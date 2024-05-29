using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AmbrellaWeb.Areas.Inspector.Controllers.AssignmentsController;
using static AmbrellaWeb.Areas.Institution.Controllers.InstitutionsController;

namespace AmbrellaWeb.Areas.Inspector.Controllers
{

    [Area("Inspector")]
    [Authorize(Roles = SD.Role_Inspector)]
    public class BuildingReportController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AssignmentNotificationService _notificationService;
        private readonly InstitutionNotificationService _institutionNotificationService;
        private readonly NotificationService _nnotificationService;




        public BuildingReportController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, AssignmentNotificationService notificationService, InstitutionNotificationService institutionNotificationService,  NotificationService nnotificationService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _notificationService = notificationService;
            _institutionNotificationService = institutionNotificationService;
            _nnotificationService = nnotificationService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SubmitApplication()
        {
            var currentUserId = _userManager.GetUserId(User);
            var buildingReports = _context.BuildingInspectionReports
                .Where(b => b.InspectorId == currentUserId)
                .ToList(); // Execute the query asynchronously

            //var institutions = _context.Universities.ToList();
            var allUsers = _userManager.Users.ToList();
            var institutionUsers = allUsers.Where(u => _userManager.IsInRoleAsync(u, SD.Role_Institution).Result).ToList();

            ViewBag.Intitutions = institutionUsers;


            // Store the buildings list in ViewBag
            ViewBag.BuildingInspectionReports = buildingReports;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitApplication(int buildingReportsId, string institutionId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var buildingReports = await _context.BuildingInspectionReports.FindAsync(buildingReportsId);

            if (buildingReports == null || buildingReports.InspectorId != currentUserId)
            {
                // Handle the case where the building is not found or does not belong to the current user
                return NotFound();
            }

            // Retrieve the entire list of users first
            var allUsers = await _userManager.Users.ToListAsync();

            // Filter the users based on their role in memory
            var institutionUsers = allUsers.Where(u => _userManager.IsInRoleAsync(u, SD.Role_Institution).Result).ToList();

            if (institutionUsers.Count == 0)
            {
                // Handle the case where no users are found with the "Institution" role
                return NotFound();
            }

            var application = new SendReportBuildingInspector
            {
                BuildingInspectionReportId = buildingReportsId,
                BuildingInspectionReport = buildingReports,
                InspectorId = currentUserId,
                InstitutionId = institutionId,
                Status = BuildingReportStatus.Pending,
                SubmittedOn = DateTime.Now
            };

            _context.SendReportBuildingInspectors.Add(application);
            await _context.SaveChangesAsync();

            var buildingName = application.BuildingInspectionReport != null ? application.BuildingInspectionReport.BuildingName : "Unknown Building";
            var notificationMessage = $"The inspection  for the building '{buildingName}' has been done successfully.";



            var notification = new InstitutionAssignNotification
            {
                Message = notificationMessage,
                CreatedAt = DateTime.Now,
            };

            _context.InstitutionAssignNotifications.Add(notification);
            await _context.SaveChangesAsync();

            var inspectionDate = DateTime.Now.AddDays(3);
            var companyName = application.BuildingInspectionReport != null ? application.BuildingInspectionReport.BuildingName : "Unknown Building";
            var nnotificationMessage = $"Your partnership application for the building '{companyName}' with Durban University Of Technology has been approved. Next step Is to await contract for signing.";




            var nnotification = new Notification
            {
                Message = notificationMessage,
                CreatedAt = DateTime.Now,
                LandlordId = application.LandlordId
            };

            _context.Notifications.Add(nnotification);
            await _context.SaveChangesAsync();

            // Optionally, you can display a success message or redirect to a different view
            return RedirectToAction("Index", "CompanyInspections");
        }
    }
}

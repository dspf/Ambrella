using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AmbrellaWeb.Areas.Inspector.Controllers.AssignmentsController;

namespace AmbrellaWeb.Areas.Inspector.Controllers
{
    [Area("Inspector")]
    [Authorize(Roles = SD.Role_Inspector)]
    public class CompanyReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AssignmentNotificationService _notificationService;


        public CompanyReportController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, AssignmentNotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _notificationService = notificationService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SubmitApplication()
        {
            var currentUserId = _userManager.GetUserId(User);
            var companyReports = _context.CompanyInspectionReports
                .Where(b => b.InspectorId == currentUserId)
                .ToList(); // Execute the query asynchronously

            //var institutions = _context.Universities.ToList();
            var allUsers = _userManager.Users.ToList();
            var adminUsers = allUsers.Where(u => _userManager.IsInRoleAsync(u, SD.Role_Admin).Result).ToList();

            ViewBag.Administrators = adminUsers;


            // Store the buildings list in ViewBag
            ViewBag.CompanyInspectionReports = companyReports;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitApplication(int companyReportsId, string adminId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var companyReports = await _context.CompanyInspectionReports.FindAsync(companyReportsId);

            if (companyReports == null || companyReports.InspectorId != currentUserId)
            {
                // Handle the case where the building is not found or does not belong to the current user
                return NotFound();
            }

            // Retrieve the entire list of users first
            var allUsers = await _userManager.Users.ToListAsync();

            // Filter the users based on their role in memory
            var adminUsers = allUsers.Where(u => _userManager.IsInRoleAsync(u, SD.Role_Admin).Result).ToList();

            if (adminUsers.Count == 0)
            {
                // Handle the case where no users are found with the "Institution" role
                return NotFound();
            }

            var application = new SendReportCompanyInspector
            {
                CompanyInspectionReportId = companyReportsId,
                CompanyInspectionReport = companyReports,
                InspectorId = currentUserId,
                AdminId = adminId,
                Status = SendReportStatus.Pending,
                SubmittedOn = DateTime.Now
            };

            _context.SendReportCompanyInspectors.Add(application);
            await _context.SaveChangesAsync();

            var companyName = application.CompanyInspectionReport != null ? application.CompanyInspectionReport.CompanyName : "Unknown Building";
            var notificationMessage = $"The inspection  for the building '{companyName}' has been done successfully.";



            var notification = new AssignmentNotification
            {
                Message = notificationMessage,
                CreatedAt = DateTime.Now,
            };

            _context.AssignmentNotifications.Add(notification);
            await _context.SaveChangesAsync();

            // Optionally, you can display a success message or redirect to a different view
            return RedirectToAction("Index", "CompanyInspections");
        }
    }
}

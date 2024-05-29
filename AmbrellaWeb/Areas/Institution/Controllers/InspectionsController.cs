using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AmbrellaWeb.Areas.Administrator.Controllers.InspectorController;

namespace AmbrellaWeb.Areas.Institution.Controllers
{
    [Area("Institution")]
    [Authorize(Roles = SD.Role_Institution)]
    public class InspectionsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly InspectorRecieveNotificationService _notificationService;
        private readonly RoleManager<IdentityRole> _roleManager;
        public InspectionsController(ApplicationDbContext db, UserManager<IdentityUser> userManager, InspectorRecieveNotificationService notificationService, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _notificationService = notificationService;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Assign(int applicationBuildingId)
        {
            var currentUserId = _userManager.GetUserId(User);

            // Get the list of all approved buildings for the logged-in institution
            var approvedBuildings = _db.ApplicationBuildings
                .Include(a => a.Building)
                .Where(a => a.InstitutionId == currentUserId && a.Status == ApplicationStatus.Approved)
                .ToList();

            if (!approvedBuildings.Any())
            {
                // Handle the case where there are no approved buildings for the institution
                return NotFound();
            }

                ViewBag.Buildings = approvedBuildings;



            var allUsers = _userManager.Users.ToList();
            var inspectorUsers = _userManager.GetUsersInRoleAsync(SD.Role_Inspector).Result.ToList();

            ViewBag.Inspector = inspectorUsers;

            // Pass the approved building to the view
            return View(approvedBuildings);
        }

      

       

        [HttpPost]
        public async Task<IActionResult> Assign(int buildingId, string inspectorId)
        {
            // Existing code to find inspector and building...
            //var currentUserId = _userManager.GetUserId(User);
            var building = await _db.Buildings.FindAsync(buildingId);



          

            var application = new InstitutionAssign
            {
                BuildingId = buildingId, // Set the BuildingId property
                Building = building,
                InspectorId = inspectorId,
                Status = InstitutionAssignmentStatus.Pending,
                SubmittedOn = DateTime.Now,
            };

            _db.InstitutionAssigns.Add(application);
            await _db.SaveChangesAsync();



            var inspectionDate = DateTime.Now.AddDays(3);
            var buildingName = application.Building != null ? application.Building.Name : "Unknown Building";
            var notificationMessage = $"Your are here by assigned to conduct inspection for the building '{buildingName}' " +
                $". An inspection has been scheduled for {inspectionDate.ToShortDateString()}.";




            var notification = new InspectorRecieveNotification
            {
                Message = notificationMessage,
                CreatedAt = DateTime.Now,
                InspectorId = application.InspectorId
            };

            _db.InspectorRecieveNotifications.Add(notification);
            await _db.SaveChangesAsync();



            // Optionally, you can display a success message or redirect to a different view
            return RedirectToAction("Index", "Institutions");
        }
    }
}

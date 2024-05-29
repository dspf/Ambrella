using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AmbrellaWeb.Areas.Institution.Controllers.InstitutionsController;

namespace AmbrellaWeb.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles = SD.Role_Admin)]
    public class InspectorController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly InspectorRecieveNotificationService _notificationService;


        public InspectorController(ApplicationDbContext db, UserManager<IdentityUser> userManager, InspectorRecieveNotificationService notificationService)
        {
            _db = db;
            _userManager = userManager;
            _notificationService = notificationService;
        }



        public IActionResult Create()
        {
            return View();
        }



        public IActionResult Assign()
        {
            var currentUserId = _userManager.GetUserId(User);
            var allUsers = _userManager.Users.ToList();
            var inspectorUsers = allUsers.Where(u => _userManager.IsInRoleAsync(u, SD.Role_Inspector).Result).ToList();

            ViewBag.Inspector = inspectorUsers;

            // Retrieve buildings from your database or any other source
            var company = _db.Companies.ToList();

            // Store the buildings list in ViewBag
            ViewBag.Companies = company;

            // Store the buildings list in ViewBag



            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Assign(int companyId, string inspectorId)
        {
            // Existing code to find inspector and building...
            var currentUserId = _userManager.GetUserId(User);
            var company = await _db.Companies.FindAsync(companyId);



            // Retrieve the entire list of users first
            var allUsers = await _userManager.Users.ToListAsync();

            // Filter the users based on their role in memory
            var inspectorUsers = allUsers.Where(u => _userManager.IsInRoleAsync(u, SD.Role_Inspector).Result).ToList();

            if (inspectorUsers.Count == 0)
            {
                // Handle the case where no users are found with the "Institution" role
                return NotFound();
            }

            var application = new InspectorAssign
            {
                CompanyId = companyId,
                Company = company,
                InspectorId = inspectorId,
                Status = AssignmentStatus.Pending,
                SubmittedOn = DateTime.Now
            };

            _db.InspectorAssigns.Add(application);
            await _db.SaveChangesAsync();

            var inspectionDate = DateTime.Now.AddDays(3);
            var companyName = application.Company != null ? application.Company.Name : "Unknown Building";
            var notificationMessage = $"Your are here by assigned to conduct inspection for the building '{companyName}' " +
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
            return RedirectToAction("Assign");
        }

        public class InspectorRecieveNotificationService
        {
            private readonly ApplicationDbContext _context;

            public InspectorRecieveNotificationService(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task SendNotificationAsync(string inspectorId, string message)
            {
                var notification = new InspectorRecieveNotification
                {
                    Message = message,
                    CreatedAt = DateTime.Now,
                    InspectorId = inspectorId
                };

                _context.InspectorRecieveNotifications.Add(notification);
                await _context.SaveChangesAsync();
            }

        }
    }
}

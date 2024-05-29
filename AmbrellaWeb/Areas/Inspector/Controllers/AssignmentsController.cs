using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AmbrellaWeb.Areas.Institution.Controllers.InstitutionsController;

namespace AmbrellaWeb.Areas.Inspector.Controllers
{
    [Area("Inspector")]
    [Authorize(Roles = SD.Role_Inspector)]
    public class AssignmentsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AssignmentNotificationService _notificationService;
        private readonly InstitutionNotificationService _institutionNotificationService;

        public AssignmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, AssignmentNotificationService notificationService, InstitutionNotificationService institutionNotificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
            _institutionNotificationService = institutionNotificationService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult create()
        {
            return View();
        }

        public IActionResult PendingApplications()
        {
            var currentAdminId = _userManager.GetUserId(User); // Get the ID of the logged-in university
            var pendingApplications = _context.InspectorAssigns
            .Where(a => a.InspectorId == currentAdminId && a.Status == AssignmentStatus.Pending)
            .Include(a => a.Company);
            //.Include(a => a.Admin);

            return View(pendingApplications);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveApplication(int id)
        {
            var application = await _context.InspectorAssigns
                .Include(a => a.Company)
                .FirstOrDefaultAsync(a => a.InspectorAssignId == id);

            if (application == null)
            {
                return NotFound();
            }

            application.Status = AssignmentStatus.Approved;
            _context.InspectorAssigns.Update(application);
            await _context.SaveChangesAsync();

            var companyName = application.Company?.Name ?? "Unknown Building";
            var location = application.Company?.Location ?? string.Empty;
            var address = application.Company?.Address ?? string.Empty;

            return RedirectToAction("Create", "CompanyInspections", new
            {
                companyName,
                location,
                address
            });
        }

        //[HttpPost]
        //public async Task<IActionResult> ApproveApplication(int id)
        //{
        //    //var application = await _context.ApplicationBuildings.FindAsync(id);
        //    var application = await _context.InspectorAssigns
        //    .Include(a => a.Company) // Include the Building navigation property
        //    .FirstOrDefaultAsync(a => a.InspectorAssignId == id);

        //    if (application == null)
        //    {
        //        return NotFound();
        //    }

        //    application.Status = AssignmentStatus.Approved;
        //    _context.InspectorAssigns.Update(application);
        //    await _context.SaveChangesAsync();



        //    var companyName = application.Company != null ? application.Company.Name : "Unknown Building";
        //    var notificationMessage = $"The inspection  for the building '{companyName}' has been accepted.";



        //    var notification = new AssignmentNotification
        //    {
        //        Message = notificationMessage,
        //        CreatedAt = DateTime.Now,
        //    };

        //    _context.AssignmentNotifications.Add(notification);
        //    await _context.SaveChangesAsync();


        //    return RedirectToAction(nameof(PendingApplications));
        //}

        public IActionResult InstitutionPendingApplications()
        {
            var currentAdminId = _userManager.GetUserId(User); // Get the ID of the logged-in university
            var pendingApplications = _context.InstitutionAssigns
            .Where(a => a.InspectorId == currentAdminId && a.Status == InstitutionAssignmentStatus.Pending)
            .Include(a => a.Building);
            //.Include(a => a.Admin);

            return View(pendingApplications);
        }


        [HttpPost]
        public async Task<IActionResult> InstitutionApproveApplication(int id)
        {
            //var application = await _context.ApplicationBuildings.FindAsync(id);
            var application = await _context.InstitutionAssigns
            .Include(a => a.Building) // Include the Building navigation property
            .FirstOrDefaultAsync(a => a.InstitutionAssignId == id);

            if (application == null)
            {
                return NotFound();
            }

            application.Status = InstitutionAssignmentStatus.Accepted;
            _context.InstitutionAssigns.Update(application);
            await _context.SaveChangesAsync();



            var buildingName = application.Building != null ? application.Building.Name : "Unknown Building";
            var notificationMessage = $"The inspection  for the building '{buildingName}' has been accepted.";



            var notification = new InstitutionAssignNotification
            {
                Message = notificationMessage,
                CreatedAt = DateTime.Now,
            };
            

            _context.InstitutionAssignNotifications.Add(notification);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(PendingApplications));
        }

        public class AssignmentNotificationService
        {
            private readonly ApplicationDbContext _context;

            public AssignmentNotificationService(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task SendNotificationAsync(string adminId, string message)
            {
                var notification = new AssignmentNotification
                {
                    Message = message,
                    CreatedAt = DateTime.Now,
                    //AdminId = adminId
                };

                _context.AssignmentNotifications.Add(notification);
                await _context.SaveChangesAsync();
            }
        }

        public class InstitutionNotificationService
        {
            private readonly ApplicationDbContext _context;

            public InstitutionNotificationService(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task SendNotificationAsync(string adminId, string message)
            {
                var notification = new InstitutionAssignNotification
                {
                    Message = message,
                    CreatedAt = DateTime.Now,
                    //AdminId = adminId
                };

                _context.InstitutionAssignNotifications.Add(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}

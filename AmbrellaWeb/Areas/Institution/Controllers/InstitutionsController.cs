using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbrellaWeb.Areas.Institution.Controllers
{
    [Area("Institution")]
    [Authorize(Roles = SD.Role_Institution)]
    public class InstitutionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly NotificationService _notificationService;
        public InstitutionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, NotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }
        public IActionResult Index()
        {
            var currentUserId = _userManager.GetUserId(User);
            //var institution = _context.Universities.Where(b=>b.InstitutionId==currentUserId).ToList();
            return View(/*institution*/);
        }

        public IActionResult create() 
        {
            return View();
        }

        public IActionResult PendingApplications()
        {
            var currentUniversityId = _userManager.GetUserId(User); // Get the ID of the logged-in university

            var pendingApplications = _context.ApplicationBuildings
                .Where(a => a.InstitutionId == currentUniversityId && a.Status == ApplicationStatus.Pending)
                .Include(a => a.Building)
                    //.ThenInclude(b => b.ExteriorImage) // Include amenities if needed
                .Include(a => a.Landlord);

            return View(pendingApplications);
        }


        [HttpPost]
        public async Task<IActionResult> ApproveApplication(int id)
        {
            //var application = await _context.ApplicationBuildings.FindAsync(id);
            var application = await _context.ApplicationBuildings
            .Include(a => a.Building) // Include the Building navigation property
            .FirstOrDefaultAsync(a => a.ApplicationBuildingId == id);

            if (application == null)
            {
                return NotFound();
            }

            application.Status = ApplicationStatus.Approved;
            _context.ApplicationBuildings.Update(application);
            await _context.SaveChangesAsync();

           


            var inspectionDate = DateTime.Now.AddDays(3);
            var buildingName = application.Building != null ? application.Building.Name : "Unknown Building";
            var notificationMessage = $"Your application was accepted. Please await inspection for the building '{buildingName}' " +
                $". An inspection has been scheduled for {inspectionDate.ToShortDateString()}.";


            var notification = new Notification
            {
                Message = notificationMessage,
                CreatedAt = DateTime.Now,
                LandlordId = application.LandlordId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(/*"Assign", "Inspections", new { applicationBuildingId = id }*/nameof(PendingApplications));
        }

        [HttpPost]
        public async Task<IActionResult> RejectApplication(int id)
        {
            var application = await _context.ApplicationBuildings.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            application.Status = ApplicationStatus.Rejected;
            _context.ApplicationBuildings.Update(application);
            await _context.SaveChangesAsync();

            // Optionally, you can send a notification to the landlord

            return RedirectToAction(nameof(PendingApplications));
        }


        public async Task<IActionResult> ApplicationsReceived()
        {
            var currentUserId = _userManager.GetUserId(User);
            var applications = _context.ApplicationInstitutions
            .Where(a => a.InstitutionId == currentUserId && a.Status == LearnerStatus.Pending)
                    .Include(ab => ab.Building)
                    .ThenInclude(ab => ab.Landlord)
                .Include(ai => ai.Student);

            return View(applications);
          
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int applicationId)
        {
            var application = await _context.ApplicationInstitutions
                 .Include(a => a.Building) // Include the Building navigation property
            .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
            {
                // Handle the case where the application is not found
                return NotFound();
            }



            application.Status = LearnerStatus.Approved;
            _context.ApplicationInstitutions.Update(application);
            await _context.SaveChangesAsync();

            var buildingName = application.Building != null ? application.Building.Name : "Unknown Building";
            var nextYear = DateTime.Now.AddYears(1);
            var notificationMessage = $"You have been allocated to the '{buildingName}' Residence for year {nextYear.ToString("MMMM dd, yyyy")} at {nextYear.ToString("h:mm tt")}.";



            var notification = new StudentNotification
            {
                Message = notificationMessage,
                CreatedAt = DateTime.Now,
                StudentId = application.StudentId
            };

            _context.StudentNotifications.Add(notification);
            await _context.SaveChangesAsync();

            // Optionally, you can send a notification to the student

            // Optionally, you can update the status of the corresponding ApplicationBuilding entity

            return RedirectToAction("ApplicationsReceived");
        }


        public class NotificationService
        {
            private readonly ApplicationDbContext _context;

            public NotificationService(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task SendNotificationAsync(string landlordId, string message)
            {
                var notification = new Notification
                {
                    Message = message,
                    CreatedAt = DateTime.Now,
                    LandlordId = landlordId
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }
        }

    }
}

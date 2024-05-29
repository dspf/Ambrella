using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static AmbrellaWeb.Areas.Institution.Controllers.InstitutionsController;

using Microsoft.EntityFrameworkCore;

namespace AmbrellaWeb.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly NotificationService _notificationService;
        public CAdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, NotificationService notificationService)
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
            var currentAdminId = _userManager.GetUserId(User); // Get the ID of the logged-in university
            var pendingApplications = _context.ApplicationCompanies
            .Where(a => a.AdminId == currentAdminId && a.Status == ApplicationStatuss.Pending)
            .Include(a => a.Company)
            .Include(a => a.Landlord);

            return View(pendingApplications);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveApplication(int id)
        {
            //var application = await _context.ApplicationBuildings.FindAsync(id);
            var application = await _context.ApplicationCompanies
            .Include(a => a.Company) // Include the Building navigation property
            .FirstOrDefaultAsync(a => a.ApplicationCompanyId == id);

            if (application == null)
            {
                return NotFound();
            }

            application.Status = ApplicationStatuss.Approved;
            _context.ApplicationCompanies.Update(application);
            await _context.SaveChangesAsync();


            var inspectionDate = DateTime.Now.AddDays(3);
            var companyName = application.Company != null ? application.Company.Name : "Unknown Building";
            var notificationMessage = $"Your application for the building '{companyName}' has been approved. An inspection has been scheduled for {inspectionDate.ToShortDateString()}.";

           


            var notification = new Notification
            {
                Message = notificationMessage,
                CreatedAt = DateTime.Now,
                LandlordId = application.LandlordId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(PendingApplications));
        }

    }
}

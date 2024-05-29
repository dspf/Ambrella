using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AmbrellaWeb.Areas.Institution.Controllers.InstitutionsController;
using static System.Net.Mime.MediaTypeNames;

namespace AmbrellaWeb.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles = SD.Role_Admin)]
    public class PendingReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly NotificationService _notificationService;
        public PendingReportsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, NotificationService notificationService)
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
            var pendingReportss = _context.SendReportCompanyInspectors
            .Where(a => a.AdminId == currentAdminId && a.Status == SendReportStatus.Pending)
            .Include(a => a.CompanyInspectionReport)
            .Include(a => a.Inspector);

            return View(pendingReportss);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveApplication(int id)
        {
            //var application = await _context.ApplicationBuildings.FindAsync(id);
            var reports = await _context.SendReportCompanyInspectors
            .Include(a => a.CompanyInspectionReport) // Include the Building navigation property
            .FirstOrDefaultAsync(a => a.CompanyInspectionReportId == id);

            if (reports == null)
            {
                return NotFound();
            }

            reports.Status = SendReportStatus.Approved;
            _context.SendReportCompanyInspectors.Update(reports);
            await _context.SaveChangesAsync();


            var companyName = reports.CompanyInspectionReport != null ? reports.CompanyInspectionReport.CompanyName : "Unknown Building";
            var notificationMessage = $"Your application for the building '{companyName}' has been inspected and approved.";



            var notification = new Notification
            {
                Message = notificationMessage,
                CreatedAt = DateTime.Now,
                LandlordId = reports.LandlordId,
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(PendingApplications));
        }
    }
}

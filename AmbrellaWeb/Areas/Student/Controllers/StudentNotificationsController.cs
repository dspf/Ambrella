using Ambrella.DataAccess;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AmbrellaWeb.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = SD.Role_Student)]
    public class StudentNotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentNotificationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var landlordId = _userManager.GetUserId(User);
            var notifications = _context.StudentNotifications
                .Where(n => n.StudentId == landlordId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return View(notifications);
        }
    }
}

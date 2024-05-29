using Ambrella.DataAccess;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AmbrellaWeb.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AssignmentNotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AssignmentNotificationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var notifications = _context.AssignmentNotifications
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return View(notifications);
        }
    }
}

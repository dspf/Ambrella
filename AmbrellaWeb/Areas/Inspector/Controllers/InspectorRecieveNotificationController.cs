using Ambrella.DataAccess;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AmbrellaWeb.Areas.Inspector.Controllers
{
    [Area("Inspector")]
    [Authorize(Roles = SD.Role_Inspector)]
    public class InspectorRecieveNotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public InspectorRecieveNotificationController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var inspectorId = _userManager.GetUserId(User);
            var notifications = _context.InspectorRecieveNotifications
                .Where(n => n.InspectorId == inspectorId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return View(notifications);
        }
    }
}

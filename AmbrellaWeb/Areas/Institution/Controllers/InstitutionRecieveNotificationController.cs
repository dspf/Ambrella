using Ambrella.DataAccess;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AmbrellaWeb.Areas.Institution.Controllers
{
    [Area("Institution")]
    [Authorize(Roles = SD.Role_Institution)]
    public class InstitutionRecieveNotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public InstitutionRecieveNotificationController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var notifications = _context.InstitutionAssignNotifications
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return View(notifications);
        }
    }
}

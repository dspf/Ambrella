using Microsoft.AspNetCore.Mvc;
using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AmbrellaWeb.Areas.Landlord.Controllers
{
    [Area("Landlord")]
    [Authorize(Roles = SD.Role_Landlord)]

    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var landlordId = _userManager.GetUserId(User);
            var notifications = _context.Notifications
                .Where(n => n.LandlordId == landlordId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
           
            return View(notifications);
        }
      
    }
}

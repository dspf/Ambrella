using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AmbrellaWeb.Areas.Landlord.Controllers
{
    [Area("Landlord")]
    [Authorize(Roles = SD.Role_Landlord)]
    public class LandlordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LandlordController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SubmitApplication()
        {
            var currentUserId = _userManager.GetUserId(User);
            var buildings = _context.Buildings
                .Where(b => b.LandlordId == currentUserId)
                .ToList(); // Execute the query asynchronously

            var allUsers = _userManager.Users.ToList();
            var institutionUsers = allUsers.Where(u => _userManager.IsInRoleAsync(u, SD.Role_Institution).Result).ToList();

            ViewBag.Universities = institutionUsers;
            ViewBag.Buildings = buildings;

            // Pass the buildings list to the view
            return View(buildings);
        }

        //[HttpPost]
        //public async Task<IActionResult> SubmitApplication(int buildingId, string institutionId)
        //{
        //    var currentUserId = _userManager.GetUserId(User);
        //    var building = await _context.Buildings.FindAsync(buildingId);

        //    if (building == null || building.LandlordId != currentUserId)
        //    {
        //        // Handle the case where the building is not found or does not belong to the current user
        //        return NotFound();
        //    }

        //    // Retrieve the entire list of users first
        //    var allUsers = await _userManager.Users.ToListAsync();

        //    // Filter the users based on their role in memory
        //    var institutionUsers = allUsers.Where(u => _userManager.IsInRoleAsync(u, SD.Role_Institution).Result).ToList();

        //    if (institutionUsers.Count == 0)
        //    {
        //        // Handle the case where no users are found with the "Institution" role
        //        return NotFound();
        //    }

        //    var application = new ApplicationBuilding
        //    {
        //        BuildingId = buildingId,
        //        Building = building,
        //        LandlordId = currentUserId,
        //        InstitutionId = institutionId,
        //        Status = ApplicationStatus.Pending,
        //        SubmittedOn = DateTime.Now
        //    };

        //    _context.ApplicationBuildings.Add(application);
        //    await _context.SaveChangesAsync();

        //    // Optionally, you can display a success message or redirect to a different view
        //    return RedirectToAction("Index", "Building");
        //}
        [HttpPost]
        public async Task<IActionResult> SubmitApplication(int buildingId, string institutionId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var building = await _context.Buildings.FindAsync(buildingId);

            if (building == null || building.LandlordId != currentUserId)
            {
                // Handle the case where the building is not found or does not belong to the current user
                return NotFound();
            }

            // Retrieve the entire list of users first
            var allUsers = await _userManager.Users.ToListAsync();

            // Filter the users based on their role in memory
            var institutionUsers = allUsers.Where(u => _userManager.IsInRoleAsync(u, SD.Role_Institution).Result).ToList();

            if (institutionUsers.Count == 0)
            {
                // Handle the case where no users are found with the "Institution" role
                return NotFound();
            }

            var application = new ApplicationBuilding
            {
                BuildingId = buildingId,
                Building = building,
                LandlordId = currentUserId,
                InstitutionId = institutionId,
                Status = ApplicationStatus.Pending,
                SubmittedOn = DateTime.Now
            };

            _context.ApplicationBuildings.Add(application);
            await _context.SaveChangesAsync();

            // Optionally, you can display a success message or redirect to a different view
            return RedirectToAction("Index", "Building");
        }

        public async Task<IActionResult> ApplicationsReceived()
        {
            var currentUserId = _userManager.GetUserId(User);
            var applications = await _context.StudentApplicationBuildings
                .Include(ab => ab.Building)
                .Include(ab => ab.Student)
                .Where(ab => ab.LandlordId == currentUserId)
                .ToListAsync();

            return View(applications);
        }
    }
}

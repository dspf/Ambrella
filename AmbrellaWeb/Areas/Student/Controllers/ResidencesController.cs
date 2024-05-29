using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbrellaWeb.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = SD.Role_Student)]
    public class ResidencesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ResidencesController(UserManager<IdentityUser> userManager, ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _roleManager = roleManager;


        }

        public IActionResult StudentRedirect()
        {
            return View();
        }
        public IActionResult Index(string searchString)
        {
            var currentUserId = _userManager.GetUserId(User);
            var buildings = _db.Buildings.Include(b => b.Landlord);

          
            return View(buildings.ToList());
        }
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitApplication(int buildingId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var building = await _db.Buildings
                .Include(b => b.Landlord)
                .FirstOrDefaultAsync(b => b.BuildingId == buildingId);

            if (building == null)
            {
                // Handle the case where the building is not found
                return NotFound();
            }

            var application = new StudentApplicationBuilding
            {
                BuildingId = buildingId,
                Building = building,
                StudentId = currentUserId,
                LandlordId = building.LandlordId,
                ApplicationDate = DateTime.Now,
                Status = StudentStatus.Pending
            };

            _db.StudentApplicationBuildings.Add(application);
            await _db.SaveChangesAsync();

            // Optionally, you can send a notification to the landlord

            // Optionally, you can display a success message or redirect to a different view
            return RedirectToAction("StudentRedirect");
        }

        public IActionResult SendApplication()
        {
            var currentUserId = _userManager.GetUserId(User);
            var buildings = _db.Buildings
               
                .ToList(); // Execute the query asynchronously

            //var institutions = _context.Universities.ToList();
            var allUsers = _userManager.Users.ToList();
            var institutionUsers = allUsers.Where(u => _userManager.IsInRoleAsync(u, SD.Role_Institution).Result).ToList();

            ViewBag.Universities = institutionUsers;


            // Store the buildings list in ViewBag
            ViewBag.Buildings = buildings;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendApplication(int buildingId, string institutionId)
        {


            var currentUserId = _userManager.GetUserId(User);
            var building = await _db.Buildings.FindAsync(buildingId);

           

            // Retrieve the entire list of users first
            var allUsers = await _userManager.Users.ToListAsync();

            // Filter the users based on their role in memory
            var institutionUsers = allUsers.Where(u => _userManager.IsInRoleAsync(u, SD.Role_Institution).Result).ToList();

            if (institutionUsers.Count == 0)
            {
                // Handle the case where no users are found with the "Institution" role
                return NotFound();
            }




            var applicationInstitution = new ApplicationInstitution
            {
                BuildingId = buildingId,
                Building = building,
                StudentId = currentUserId,
                InstitutionId = institutionId,
                SubmittedOn = DateTime.Now,
                Status = LearnerStatus.Pending
            };

            _db.ApplicationInstitutions.Add(applicationInstitution);
            await _db.SaveChangesAsync();

            // Optionally, you can send a notification to the institution

            // Optionally, you can display a success message or redirect to a different view
            return RedirectToAction("StudentRedirect");
        }
    }
}

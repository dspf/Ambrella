using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbrellaWeb.Areas.Landlord.Controllers
{

    [Area("Landlord")]
    [Authorize(Roles = SD.Role_Landlord)]
    public class CApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CApplicationController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
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
            var companies = _context.Companies
                .Where(b => b.LandlordId == currentUserId)
                .ToList(); // Execute the query asynchronously

            //var institutions = _context.Universities.ToList();
            var allUsers = _userManager.Users.ToList();
            var adminUsers = allUsers.Where(u => _userManager.IsInRoleAsync(u, SD.Role_Admin).Result).ToList();

            ViewBag.Administrator = adminUsers;


            // Store the buildings list in ViewBag
            ViewBag.Companies = companies;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitApplication(int companyId, string adminId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var company = await _context.Companies.FindAsync(companyId);

            if (company == null || company.LandlordId != currentUserId)
            {
                // Handle the case where the building is not found or does not belong to the current user
                return NotFound();
            }

            // Retrieve the entire list of users first
            var allUsers = await _userManager.Users.ToListAsync();

            // Filter the users based on their role in memory
            var adminUsers = allUsers.Where(u => _userManager.IsInRoleAsync(u, SD.Role_Admin).Result).ToList();

            if (adminUsers.Count == 0)
            {
                // Handle the case where no users are found with the "Institution" role
                return NotFound();
            }

            var application = new ApplicationCompany
            {
                CompanyId = companyId,
                Company = company,
                LandlordId = currentUserId,
                AdminId = adminId,
                Status = ApplicationStatuss.Pending,
                SubmittedOn = DateTime.Now
            };

            _context.ApplicationCompanies.Add(application);
            await _context.SaveChangesAsync();

            // Optionally, you can display a success message or redirect to a different view
            return RedirectToAction("Index", "Company");
        }
    }
}

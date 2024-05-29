using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AmbrellaWeb.Areas.Landlord.Controllers
{
    [Area("Landlord")]
    [Authorize(Roles = SD.Role_Landlord)]
    public class CompanyController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CompanyController(UserManager<IdentityUser> userManager, ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index(string searchString)
        {
            var currentUserId = _userManager.GetUserId(User);
            var companies = from b in _db.Companies.Where(b => b.LandlordId == currentUserId)
                            select b;

            if (!string.IsNullOrEmpty(searchString))
            {
                companies = companies.Where(b => b.Location.Contains(searchString) && b.LandlordId == currentUserId);
            }

            return View(companies.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Company obj, IFormFile HeadQuarterImgImage)
        {
            //if (ModelState.IsValid)
            //{
            var currentUserId = _userManager.GetUserId(User);
            obj.LandlordId = currentUserId;
            obj.HeadQuarterImg = UploadImage(HeadQuarterImgImage);
           

            _db.Companies.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");

        }

        private string UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                // Check if the directory exists, if not, create it
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return "/uploads/" + uniqueFileName; // Return the relative path to store in the database
            }
            return null;
        }
    }
}

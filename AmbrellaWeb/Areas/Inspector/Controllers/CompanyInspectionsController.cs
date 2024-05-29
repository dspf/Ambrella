using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AmbrellaWeb.Areas.Inspector.Controllers
{
    [Area("Inspector")]
    [Authorize(Roles = SD.Role_Inspector)]
    public class CompanyInspectionsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CompanyInspectionsController(UserManager<IdentityUser> userManager, ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index(string searchString)
        {
            var currentUserId = _userManager.GetUserId(User);
            var companyReports = from b in _db.CompanyInspectionReports.Where(b => b.InspectorId == currentUserId)
                            select b;

            if (!string.IsNullOrEmpty(searchString))
            {
                companyReports = companyReports.Where(b => b.CompanyName.Contains(searchString) && b.InspectorId == currentUserId);
            }

            return View(companyReports.ToList());
        }

        public IActionResult Create(string companyName, string location, string address)
        {
            // Create a new instance of your model and assign the values
            var model = new CompanyInspectionReport
            {
                CompanyName = companyName,
                CompanyLocation = location,
                CompanyAddress = address
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CompanyInspectionReport obj, IFormFile firstBuildingImage, IFormFile secondBuildingImage, IFormFile thirdBuildingImage, IFormFile fourthBuildingImage)
        {
            //if (ModelState.IsValid)
            //{
            var currentUserId = _userManager.GetUserId(User);
            obj.InspectorId = currentUserId;

            obj.InspectionDate = DateTime.Now; // Set the inspection date

            // Handle null file uploads and assign values to properties
            obj.FirstBuilding = firstBuildingImage != null ? UploadImage(firstBuildingImage) : string.Empty;
            obj.SecondBuilding = secondBuildingImage != null ? UploadImage(secondBuildingImage) : string.Empty;
            obj.ThirdBuilding = thirdBuildingImage != null ? UploadImage(thirdBuildingImage) : string.Empty;
            obj.FourthBuilding = fourthBuildingImage != null ? UploadImage(fourthBuildingImage) : string.Empty;

            if (!string.IsNullOrEmpty(obj.CompanyName) && !string.IsNullOrEmpty(obj.CompanyRegistrationNumber))
            {
                obj.IsCompanyVerified = true;
            }
            else
            {
                obj.IsCompanyVerified = false;
            }

            _db.CompanyInspectionReports.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");

        }

        private string UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "upload");

                // Check if the directory exists, if not, create it
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var uniqueFilesName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadFolder, uniqueFilesName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return "/upload/" + uniqueFilesName; // Return the relative path to store in the database
            }
            return null;
        }
        public IActionResult Search()
        {
            return View();
        }
    }
}

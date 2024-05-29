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
    public class BuildingInspectionsController : Controller
    {
      

        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BuildingInspectionsController(UserManager<IdentityUser> userManager, ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index(string searchString)
        {
            var currentUserId = _userManager.GetUserId(User);
            var buildingReports = from b in _db.BuildingInspectionReports.Where(b => b.InspectorId == currentUserId)
                                 select b;

            if (!string.IsNullOrEmpty(searchString))
            {
                buildingReports = buildingReports.Where(b => b.BuildingName.Contains(searchString) && b.InspectorId == currentUserId);
            }

            return View(buildingReports.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(BuildingInspectionReport obj, IFormFile firstBuildingImage, IFormFile secondBuildingImage, IFormFile thirdBuildingImage, IFormFile fourthBuildingImage)
        {
            //if (ModelState.IsValid)
            //{
            var currentUserId = _userManager.GetUserId(User);
            obj.InspectorId = currentUserId;

            obj.InspectionDate = DateTime.Now; // Set the inspection date

            // Handle null file uploads and assign values to properties
            obj.RoomImage = firstBuildingImage != null ? UploadImage(firstBuildingImage) : string.Empty;
            obj.Kitchen = secondBuildingImage != null ? UploadImage(secondBuildingImage) : string.Empty;
            obj.ExteriorImage = thirdBuildingImage != null ? UploadImage(thirdBuildingImage) : string.Empty;
            obj.StudyImage = fourthBuildingImage != null ? UploadImage(fourthBuildingImage) : string.Empty;

            if (!string.IsNullOrEmpty(obj.BuildingName) && !string.IsNullOrEmpty(obj.BuildingAddress))
            {
                obj.IsBuildingVerified = true;
            }
            else
            {
                obj.IsBuildingVerified = false;
            }

            _db.BuildingInspectionReports.Add(obj);
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
    }
}

using Microsoft.AspNetCore.Mvc;

namespace AmbrellaWeb.Areas.Administrator.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Hosting;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Ambrella.DataAccess;
    using global::Ambrella.Models;

    namespace Ambrella.Controllers
    {
        public class BuildingController : Controller
        {
            private readonly UserManager<IdentityUser> _userManager;
            private readonly ApplicationDbContext _db;
            private readonly IWebHostEnvironment _webHostEnvironment;

            public BuildingController(UserManager<IdentityUser> userManager, ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
            {
                _userManager = userManager;
                _db = db;
                _webHostEnvironment = webHostEnvironment;
            }

            public IActionResult Index()
            {
                var currentUserId = _userManager.GetUserId(User);
                var buildings = _db.Buildings.Where(b => b.LandlordId == currentUserId).ToList();
                return View(buildings);
            }

            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> Create(Building obj, IFormFile roomImage, IFormFile exteriorImage, IFormFile studyImage, IFormFile kitchenImage)
            {
                var currentUserId = _userManager.GetUserId(User);
                obj.LandlordId = currentUserId;
                obj.RoomImage = await UploadImage(roomImage);
                obj.ExteriorImage = await UploadImage(exteriorImage);
                obj.StudyImage = await UploadImage(studyImage);
                obj.KitchenImage = await UploadImage(kitchenImage);

                _db.Buildings.Add(obj);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            private async Task<string> UploadImage(IFormFile file)
            {
                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return "/uploads/" + uniqueFileName;
                }
                return null;
            }

            public async Task<IActionResult> Edit(int id)
            {
                var building = await _db.Buildings.FindAsync(id);
                if (building == null)
                {
                    return NotFound();
                }
                return View(building);
            }

            [HttpPost]
            public async Task<IActionResult> Edit(Building building)
            {
                if (ModelState.IsValid)
                {
                    _db.Update(building);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(building);
            }

            public async Task<IActionResult> Details(int id)
            {
                var building = await _db.Buildings.FindAsync(id);
                if (building == null)
                {
                    return NotFound();
                }
                return View(building);
            }

            public async Task<IActionResult> Delete(int id)
            {
                var building = await _db.Buildings.FindAsync(id);
                if (building == null)
                {
                    return NotFound();
                }
                return View(building);
            }

            [HttpPost, ActionName("Delete")]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var building = await _db.Buildings.FindAsync(id);
                _db.Buildings.Remove(building);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }
    }

}

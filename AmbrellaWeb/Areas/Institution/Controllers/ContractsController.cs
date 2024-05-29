using Ambrella.DataAccess;
using Ambrella.Models;
using Ambrella.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AmbrellaWeb.Areas.Institution.Controllers
{
    [Area("Institution")]

    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ContractsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = SD.Role_Institution)]
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [Authorize(Roles = SD.Role_Institution)]
        [HttpPost]
        public async Task<IActionResult> Upload(Contract contract, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file.");
                return View(contract);
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                contract.FileContent = memoryStream.ToArray();
                contract.FileName = file.FileName;
                contract.FileContentType = file.ContentType;
            }

            var currentUserId = _userManager.GetUserId(User);
            contract.InstitutionId = currentUserId;
            contract.CreatedAt = DateTime.Now;
            contract.Status = ContractStatus.Draft;

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return RedirectToAction("Upload");
        }

        [Authorize(Roles = SD.Role_Institution)]
        [HttpGet]
        public async Task<IActionResult> Send()
        {
            var currentUserId = _userManager.GetUserId(User);
            var draftContracts = await _context.Contracts
                .Where(c => c.InstitutionId == currentUserId && c.Status == ContractStatus.Draft)
                .ToListAsync();

            var landlordUsers = await _userManager.GetUsersInRoleAsync(SD.Role_Landlord);
            ViewBag.LandlordUsers = new SelectList(landlordUsers, "Id", "UserName");

            return View(draftContracts);
        }

        [Authorize(Roles = SD.Role_Institution)]
        [HttpPost]
        public async Task<IActionResult> Send(int contractId, string landlordId)
        {
            var contract = await _context.Contracts.FindAsync(contractId);
          
            var recipient = await _userManager.FindByIdAsync(landlordId);
            if (recipient == null || !await _userManager.IsInRoleAsync(recipient, SD.Role_Landlord))
            {
                ModelState.AddModelError("RecipientId", "Invalid recipient.");
                return RedirectToAction("Send");
            }

            contract.LandlordId = landlordId;
            contract.Status = ContractStatus.Sent;
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();

            // Optionally, send a notification to the recipient

            return RedirectToAction("Upload");
        }

        [Authorize(Roles = SD.Role_Landlord)]
        [HttpGet]
        public async Task<IActionResult> Received()
        {
            var receivedContracts = await _context.Contracts
                .Where(c => c.Status == ContractStatus.Draft)
                .ToListAsync();

            return View(receivedContracts.ToList());
        }

        [Authorize(Roles = SD.Role_Landlord)]
        [HttpGet]
        public async Task<IActionResult> Download(int contractId)
        {
            var contract = await _context.Contracts.FindAsync(contractId);
            if (contract == null)
            {
                return NotFound();
            }

            return File(contract.FileContent, contract.FileContentType, contract.FileName);
        }

        [HttpPost]
        public async Task<IActionResult> UploadSigned(int contractId, IFormFile file)
        {
            var contract = await _context.Contracts.FindAsync(contractId);
          

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file.");
                return RedirectToAction("Received");
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                contract.FileContent = memoryStream.ToArray();
                contract.FileName = file.FileName;
                contract.FileContentType = file.ContentType;
            }

            contract.Status = ContractStatus.Received;
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();

            // Optionally, send a notification to the institution

            return RedirectToAction("Upload");
        }

        [Authorize(Roles = SD.Role_Institution)]
        [HttpGet]
        public async Task<IActionResult> ReceivedSigned()
        {
            //var currentUserId = _userManager.GetUserId(User);
            var receivedSignedContracts = await _context.Contracts
                .Where(c => c.Status == ContractStatus.Received)
                .ToListAsync();

            return View(receivedSignedContracts.ToList());
        }

        [Authorize(Roles = SD.Role_Institution)]
        [HttpGet]
        public async Task<IActionResult> DownloadSigned(int contractId)
        {
            var contract = await _context.Contracts.FindAsync(contractId);
            if (contract == null)
            {
                return NotFound();
            }

            return File(contract.FileContent, contract.FileContentType, contract.FileName);
        }

        [Authorize(Roles = SD.Role_Institution)]
        [HttpPost]
        public async Task<IActionResult> AcceptOrReject(int contractId, ContractStatus status)
        {
            var contract = await _context.Contracts.FindAsync(contractId);
            if (contract == null)
            {
                return NotFound();
            }

            contract.Status = status;
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();

            // Optionally, send a notification to the landlord

            return RedirectToAction("ReceivedSigned");
        }
    }



}

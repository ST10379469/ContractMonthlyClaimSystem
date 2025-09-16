using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private static readonly string[] AllowedExtensions = { ".pdf", ".docx", ".xlsx", ".jpg", ".png", ".jpeg" };

        public ClaimsController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }

            var userEmail = HttpContext.Session.GetString("UserEmail");
            var claims = GetUserClaims(userEmail);
            return View(claims);
        }

        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MonthlyClaim claim, string action, List<IFormFile> supportingDocuments)
        {
            if (ModelState.IsValid)
            {
                // Set user information
                claim.UserId = HttpContext.Session.GetString("UserEmail");
                claim.CreatedDate = DateTime.Now;

                // Set status based on action
                claim.Status = action == "submit" ? ClaimStatus.PendingReview : ClaimStatus.Draft;

                if (action == "submit")
                {
                    claim.SubmittedDate = DateTime.Now;
                }

                // Calculate total amount
                claim.TotalAmount = claim.ClaimItems?.Sum(item => item.Amount) ?? 0;

                // Handle file uploads
                if (supportingDocuments != null && supportingDocuments.Count > 0)
                {
                    claim.SupportingDocuments = new List<SupportingDocument>();

                    foreach (var file in supportingDocuments)
                    {
                        if (file.Length > 0)
                        {
                            // Validate file
                            var validationResult = ValidateFile(file);
                            if (!validationResult.isValid)
                            {
                                ModelState.AddModelError("", validationResult.errorMessage);
                                return View(claim);
                            }

                            // Save file
                            var document = await SaveUploadedFile(file, claim.Id);
                            if (document != null)
                            {
                                claim.SupportingDocuments.Add(document);
                            }
                        }
                    }
                }

                // Save claim logic here (database operation)
                // _context.Claims.Add(claim);
                // await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = action == "submit"
                    ? "Claim submitted successfully!"
                    : "Claim saved as draft.";

                return RedirectToAction("Index");
            }

            return View(claim);
        }

        public IActionResult Details(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }

            var userEmail = HttpContext.Session.GetString("UserEmail");
            var claim = GetClaimById(id);

            if (claim == null || claim.UserId != userEmail)
            {
                return NotFound();
            }

            return View(claim);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, ClaimStatus status, string notes = "")
        {
            // Update claim status logic
            var claim = GetClaimById(id);
            if (claim != null)
            {
                claim.Status = status;
                // Save changes to database
            }

            TempData["SuccessMessage"] = $"Claim status updated to {status}.";
            return RedirectToAction("Index");
        }

        private (bool isValid, string errorMessage) ValidateFile(IFormFile file)
        {
            // Check file size
            if (file.Length > MaxFileSize)
            {
                return (false, $"File {file.FileName} exceeds maximum size of 5MB.");
            }

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return (false, $"File type {extension} is not supported. Allowed types: {string.Join(", ", AllowedExtensions)}");
            }

            return (true, string.Empty);
        }

        private async Task<SupportingDocument> SaveUploadedFile(IFormFile file, int claimId)
        {
            try
            {
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "claims", claimId.ToString());
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return new SupportingDocument
                {
                    FileName = file.FileName,
                    FilePath = filePath,
                    UploadDate = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                // Log error
                return null;
            }
        }

        // Helper methods - replace with actual database calls
        private List<MonthlyClaim> GetUserClaims(string userEmail)
        {
            return new List<MonthlyClaim>
            {
                new MonthlyClaim
                {
                    Id = 1,
                    Month = 1,
                    Year = 2024,
                    TotalAmount = 1500,
                    Status = ClaimStatus.Approved,
                    SubmittedDate = new DateTime(2024, 1, 15),
                    UserId = userEmail
                },
                new MonthlyClaim
                {
                    Id = 2,
                    Month = 2,
                    Year = 2024,
                    TotalAmount = 1800,
                    Status = ClaimStatus.PendingReview,
                    SubmittedDate = new DateTime(2024, 2, 10),
                    UserId = userEmail
                }
            };
        }

        private MonthlyClaim GetClaimById(int id)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            return GetUserClaims(userEmail).FirstOrDefault(c => c.Id == id);
        }
    }
}
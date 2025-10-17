using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Http;
using ContractMonthlyClaimSystem.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ClaimsController> _logger;

        public ClaimsController(IWebHostEnvironment environment, ILogger<ClaimsController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
                {
                    return RedirectToAction("Login", "Account");
                }

                var userEmail = HttpContext.Session.GetString("UserEmail");
                var claims = GetUserClaims(userEmail);

                return View(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving claims for user.");
                TempData["ErrorMessage"] = "Unable to load your claims. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Create()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
                {
                    return RedirectToAction("Login", "Account");
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading claim creation form.");
                TempData["ErrorMessage"] = "Unable to load claim form. Please try again.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MonthlyClaim claim, string action, List<IFormFile> supportingDocuments)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                    throw new ClaimValidationException("Please correct the validation errors.", errors);
                }

                // Validate user session
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    throw new UnauthorizedAccessException("User session expired. Please login again.");
                }

                claim.UserId = userEmail;
                claim.CreatedDate = DateTime.Now;
                claim.Status = action == "submit" ? ClaimStatus.PendingReview : ClaimStatus.Draft;

                if (action == "submit")
                {
                    claim.SubmittedDate = DateTime.Now;
                }

                // Validate and process files
                if (supportingDocuments != null && supportingDocuments.Count > 0)
                {
                    claim.SupportingDocuments = new List<SupportingDocument>();

                    foreach (var file in supportingDocuments)
                    {
                        if (file.Length > 0)
                        {
                            var validationResult = ValidateFile(file);
                            if (!validationResult.isValid)
                            {
                                throw new FileValidationException(validationResult.errorMessage);
                            }

                            var document = await SaveUploadedFile(file, claim.Id);
                            if (document != null)
                            {
                                claim.SupportingDocuments.Add(document);
                            }
                        }
                    }
                }

                // Save claim to database
                

                TempData["SuccessMessage"] = action == "submit"
                    ? "Claim submitted successfully!"
                    : "Claim saved as draft.";

                return RedirectToAction("Index");
            }
            catch (ClaimValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    foreach (var message in error.Value)
                    {
                        ModelState.AddModelError(error.Key, message);
                    }
                }
                return View(claim);
            }
            catch (FileValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(claim);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt.");
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating claim.");
                TempData["ErrorMessage"] = "An error occurred while creating your claim. Please try again.";
                return View(claim);
            }
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, ClaimStatus status, string notes = "")
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
                {
                    throw new UnauthorizedAccessException("User session expired.");
                }

                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "Coordinator" && userRole != "Manager")
                {
                    throw new UnauthorizedAccessException("You don't have permission to perform this action.");
                }

                var claim = GetClaimById(id);
                if (claim == null)
                {
                    throw new ArgumentException($"Claim with ID {id} not found.");
                }

                claim.Status = status;
                // Save changes to database

                TempData["SuccessMessage"] = $"Claim status updated to {status} successfully.";
                return RedirectToAction("Index");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized status update attempt.");
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Login", "Account");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Claim not found for status update.");
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating claim status.");
                TempData["ErrorMessage"] = "An error occurred while updating the claim status.";
                return RedirectToAction("Index");
            }
        }

        // Helper methods 
        private (bool isValid, string errorMessage) ValidateFile(IFormFile file)
        {
            const long maxFileSize = 5 * 1024 * 1024; // 5MB
            string[] allowedExtensions = { ".pdf", ".docx", ".xlsx", ".jpg", ".png", ".jpeg" };

            if (file.Length > maxFileSize)
            {
                return (false, $"File '{file.FileName}' exceeds maximum size of 5MB.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return (false, $"File type '{extension}' is not supported. Allowed types: {string.Join(", ", allowedExtensions)}");
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
                _logger.LogError(ex, "Error saving uploaded file.");
                throw new FileValidationException("Error saving file. Please try again.");
            }
        }

        private List<MonthlyClaim> GetUserClaims(string userEmail)
        {
            
            return new List<MonthlyClaim>
            {
                new MonthlyClaim
                {
                    Id = 1,
                    Month = 3,
                    Year = 2024,
                    TotalAmount = 1250,
                    Status = ClaimStatus.PendingReview,
                    SubmittedDate = DateTime.Now.AddDays(-2),
                    UserId = userEmail
                },
                new MonthlyClaim
                {
                    Id = 2,
                    Month = 2,
                    Year = 2024,
                    TotalAmount = 1800,
                    Status = ClaimStatus.Approved,
                    SubmittedDate = DateTime.Now.AddDays(-7),
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
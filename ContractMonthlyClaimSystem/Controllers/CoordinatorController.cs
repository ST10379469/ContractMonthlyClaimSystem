using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Http;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class CoordinatorController : Controller
    {
        public IActionResult Index()
        {
            // Check if user is logged in and is coordinator/manager
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Coordinator" && userRole != "Manager")
            {
                return RedirectToAction("Index", "Home");
            }

            // Simulate claims for review
            var claims = new List<MonthlyClaim>
            {
                new MonthlyClaim
                {
                    Id = 1,
                    UserId = "lecturer1@university.edu",
                    Month = 1,
                    Year = 2024,
                    TotalAmount = 1500,
                    Status = ClaimStatus.PendingReview
                },
                new MonthlyClaim
                {
                    Id = 2,
                    UserId = "lecturer2@university.edu",
                    Month = 2,
                    Year = 2024,
                    TotalAmount = 1800,
                    Status = ClaimStatus.PendingReview
                }
            };

            return View(claims);
        }

        public IActionResult Review(int id)
        {
            // Check if user is logged in and is coordinator/manager
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Coordinator" && userRole != "Manager")
            {
                return RedirectToAction("Index", "Home");
            }

            // Simulate getting claim by id
            var claim = new MonthlyClaim
            {
                Id = id,
                UserId = "lecturer1@university.edu",
                Month = 1,
                Year = 2024,
                TotalAmount = 1500,
                Status = ClaimStatus.PendingReview,
                ClaimItems = new List<ClaimItem>
                {
                    new ClaimItem { Date = new DateTime(2024, 1, 15), HoursWorked = 8, Module = "CS101", Description = "Lecture", Amount = 400 },
                    new ClaimItem { Date = new DateTime(2024, 1, 22), HoursWorked = 6, Module = "CS102", Description = "Tutorial", Amount = 300 }
                }
            };

            var viewModel = new ReviewViewModel { Claim = claim };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Review(int id, string action, string notes)
        {
            // Handle review actions
            return RedirectToAction("Index");
        }
    }
}
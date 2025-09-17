using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Http;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimsController : Controller
    {
        public IActionResult Index()
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }

            // Simulate claims data
            var claims = new List<MonthlyClaim>
            {
                new MonthlyClaim { Id = 1, Month = 1, Year = 2024, TotalAmount = 1500, Status = ClaimStatus.Approved },
                new MonthlyClaim { Id = 2, Month = 2, Year = 2024, TotalAmount = 1800, Status = ClaimStatus.PendingReview }
            };

            return View(claims);
        }

        public IActionResult Create()
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(MonthlyClaim claim)
        {
            if (ModelState.IsValid)
            {
                // Save claim logic here
                return RedirectToAction("Index");
            }
            return View(claim);
        }
    }
}
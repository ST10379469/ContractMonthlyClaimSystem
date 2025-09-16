using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Http;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Get user role from session
            var userRole = HttpContext.Session.GetString("UserRole") ?? "Lecturer";
            ViewBag.UserRole = userRole;
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "user@university.edu";

            return View();
        }
    }
}
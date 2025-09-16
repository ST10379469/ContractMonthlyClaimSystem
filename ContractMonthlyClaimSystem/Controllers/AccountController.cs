using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Http;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Simulate authentication - in real app, use Identity
                HttpContext.Session.SetString("UserEmail", model.Email);
                HttpContext.Session.SetString("UserRole", model.Role.ToString());

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
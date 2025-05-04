using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfServicePortal.Core.Interfaces;
using SelfServicePortal.Web.Models;

namespace SelfServicePortal.Web.Controllers
{
    public class AccountController(IAuthService authService) : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
                return View(model);

            var result = await authService.LoginUserAsync(model.Username, model.Password);
            if (result.Succeeded)
            {
                TempData["Success"] = "Welcome back!";
                return RedirectToLocal(returnUrl);
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
                return View(model);

            var (Succeeded, Errors) = await authService.RegisterUserAsync(model.Username, model.Email, model.Password);
            if (Succeeded)
            {
                TempData["Success"] = "Registration successful! You can now log in.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in Errors)
                ModelState.AddModelError(string.Empty, error);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await authService.LogoutUserAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            return Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl!)
                : RedirectToAction(nameof(HomeController.Index), nameof(HomeController));
        }
    }
}

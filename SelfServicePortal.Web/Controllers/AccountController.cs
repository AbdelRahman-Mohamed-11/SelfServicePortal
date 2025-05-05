using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfServicePortal.Core.Interfaces;
using SelfServicePortal.Web.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SelfServicePortal.Web.Controllers
{
    public class AccountController(IAuthService authService , 
        ILogger<AccountController> logger) : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            logger.LogInformation("GET Login page requested; returnUrl={ReturnUrl}", returnUrl);

            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");

            logger.LogInformation("POST Login attempt for user {Username}", model.Username);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Login model state invalid for user {Username}", model.Username);
                return View(model);
            }

            var (Succeeded, Errors) = await authService.LoginUserAsync(model.Username, model.Password);
            if (Succeeded)
            {
                logger.LogInformation("User logged in successfully");
                TempData["SuccessMessage"] = "Welcome back, " + model.Username + "!";
                return RedirectToAction("Index", "Home");
            }

            logger.LogWarning("Login failed for user");

            foreach (var error in Errors)
                ModelState.AddModelError(string.Empty, error);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            logger.LogInformation("GET Register page requested; returnUrl={ReturnUrl}", returnUrl);
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");

            logger.LogInformation("POST Register attempt for user {Username}", model.Username);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Register model state invalid for user {Username}", model.Username);
                return View(model);
            }
            var (Succeeded, Errors) = await authService.RegisterUserAsync(model.Username, model.Email, model.Password);
            if (Succeeded)
            {
                logger.LogInformation("User {Username} registered successfully", model.Username);
                TempData["Success"] = "Registration successful! You can now log in.";
                return RedirectToAction(nameof(Login));
            }

            logger.LogWarning("Registration failed for user {Username}", model.Username);

            foreach (var error in Errors)
                ModelState.AddModelError(string.Empty, error);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            logger.LogInformation("User logout requested");

            await authService.LogoutUserAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                logger.LogInformation("Redirecting to local URL {ReturnUrl}", returnUrl);
                return Redirect(returnUrl!);
            }
            logger.LogInformation("Redirecting to Home/Index");
            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController));
        }
    }
}

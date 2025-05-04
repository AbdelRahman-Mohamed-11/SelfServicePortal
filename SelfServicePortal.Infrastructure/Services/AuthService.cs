using Microsoft.AspNetCore.Identity;
using SelfServicePortal.Core.Entities.Identity;
using SelfServicePortal.Core.Entities.Identity.Enums;
using SelfServicePortal.Core.Interfaces;

namespace SelfServicePortal.Infrastructure.Services
{
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager) : IAuthService
    {
        public async Task<(bool Succeeded, string[] Errors)> RegisterUserAsync(string username, string email, string password)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                Email = email
            };

            string roleName = nameof(Role.User);
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, roleName);
                return (true, Array.Empty<string>());
            }

            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<(bool Succeeded, string[] Errors)> LoginUserAsync(string username, string password)
        {
            var result = await signInManager.PasswordSignInAsync(username, password, false, false);
            if (result.Succeeded)
            {
                return (true, Array.Empty<string>());
            }

            return (false, new[] { "Invalid login attempt." });
        }

        public async Task LogoutUserAsync()
        {
            await signInManager.SignOutAsync();
        }
    }
}
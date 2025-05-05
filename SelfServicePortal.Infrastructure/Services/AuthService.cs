using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SelfServicePortal.Core.Entities.Identity;
using SelfServicePortal.Core.Entities.Identity.Enums;
using SelfServicePortal.Core.Interfaces;

namespace SelfServicePortal.Infrastructure.Services
{
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<AuthService> logger) : IAuthService
    {
        public async Task<(bool Succeeded, string[] Errors)> RegisterUserAsync(string username, string email, string password)
        {
            logger.LogInformation("RegisterUserAsync called for {Username}", username);

            var user = new ApplicationUser
            {
                UserName = username,
                Email = email
            };

            string roleName = nameof(Role.User);
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                logger.LogInformation("Role {Role} does not exist—creating it", roleName);
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName, Description = "Normal User" });
            }
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                logger.LogInformation("User {Username} created successfully; assigning role {Role}", username, roleName);
                await userManager.AddToRoleAsync(user, roleName);
                return (true, Array.Empty<string>());
            }

            var errors = result.Errors.Select(e => e.Description).ToArray();
            logger.LogWarning("Failed to create user {Username}: {Errors}", username, string.Join("; ", errors));
            return (false, errors);
        }

        public async Task<(bool Succeeded, string[] Errors)> LoginUserAsync(string username, string password)
        {
            logger.LogInformation("LoginUserAsync attempt for {Username}", username);

            var result = await signInManager.PasswordSignInAsync(username, password, false, false);
            if (result.Succeeded)
            {
                logger.LogInformation("User {Username} logged in successfully", username);
                return (true, Array.Empty<string>());
            }

            logger.LogWarning("Invalid login attempt for {Username}", username);

            return (false, new[] { "Invalid login attempt." });
        }

        public async Task LogoutUserAsync()
        {
            logger.LogInformation("LogoutUserAsync called");
            
            await signInManager.SignOutAsync();

            logger.LogInformation("User signed out");
        }
    }
}
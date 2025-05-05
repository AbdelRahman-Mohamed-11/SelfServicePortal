using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SelfServicePortal.Core.Entities.Identity;
using SelfServicePortal.Core.Entities.Identity.Enums;
using SelfServicePortal.Core.Interfaces;

namespace SelfServicePortal.Infrastructure.Services;

public class UserService(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
    ILogger<UserService> logger) : IUserService
{
    public async Task<List<SelectListItem>> GetAllUsersAsSelectListAsync()
    {
        logger.LogInformation("Loading all users as SelectList");

        return await userManager.Users.Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = u.UserName
        })
         .ToListAsync();
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        logger.LogInformation("Loading all users");

        return await userManager.Users.AsNoTracking().ToListAsync();
    }

    public async Task<bool> IsAdminAsync(Guid userId)
    {
        logger.LogInformation("Checking if user {UserId} is in Admin role", userId);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            logger.LogWarning("User {UserId} not found", userId);
            return false;
        }

        var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

        logger.LogInformation("User {UserId} is{Not} an Admin", userId, isAdmin ? "" : " not");

        return isAdmin;
    }
}
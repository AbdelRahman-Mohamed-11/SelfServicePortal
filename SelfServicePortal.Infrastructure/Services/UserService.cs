using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SelfServicePortal.Core.Entities.Identity;
using SelfServicePortal.Core.Interfaces;

namespace SelfServicePortal.Infrastructure.Services;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    public async Task<List<SelectListItem>> GetAllUsersAsSelectListAsync()
    {
        return await userManager.Users
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.UserName
            })
            .ToListAsync();
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        return await userManager.Users.AsNoTracking().ToListAsync();
    }

    public async Task<bool> IsAdminAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        return await userManager.IsInRoleAsync(user, "Admin");
    }
}
using SelfServicePortal.Core.Entities.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SelfServicePortal.Core.Interfaces;

public interface IUserService
{
    Task<List<SelectListItem>> GetAllUsersAsSelectListAsync();
    Task<List<ApplicationUser>> GetAllUsersAsync();
    Task<bool> IsAdminAsync(Guid userId);
}
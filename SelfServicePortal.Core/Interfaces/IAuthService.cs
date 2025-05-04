
namespace SelfServicePortal.Core.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Succeeded, string[] Errors)> RegisterUserAsync(string username, string email, string password);
        Task<(bool Succeeded, string[] Errors)> LoginUserAsync(string username, string password);
        Task LogoutUserAsync();
    }
}
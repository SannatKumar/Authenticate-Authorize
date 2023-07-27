using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ServiceButtonBackend.Data
{
    public interface IAuthRepository
    {
        Task<ServiceRespone<int>> Register(User user, string password);

        Task<AuthServiceRespone<string>> Login(string username, string password);

        Task<bool> UserExists(string username);

        Task<ServiceRespone<string>> RefreshToken(string refreshToken);

        Task<AuthServiceRespone<string>> GetMe();

        IActionResult Logout();

    }
}

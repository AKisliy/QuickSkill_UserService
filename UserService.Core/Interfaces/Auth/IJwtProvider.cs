using System.Security.Claims;
using UserService.Core.Models;

namespace UserService.Core.Interfaces.Auth
{
    public interface IJwtProvider
    {
        public string GenerateToken(User user);

        public string GenerateRefreshToken();

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
    }
}
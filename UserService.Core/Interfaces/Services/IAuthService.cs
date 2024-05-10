using UserService.Core.Models;

namespace UserService.Core.Interfaces.Services
{
    public interface IAuthService
    {
        public Task Register(string firstName, string lastName, string email, string password);
        
        public Task<TokensLogin> Login(string email, string password);

        public Task Verify(string token);

        public Task ForgotPassword(string email);

        public Task ResetPassword(string password, string token);

        public Task CheckPassword(int id, string password);

        public Task ChangePassword(int id, string oldPassword, string newPassword);

        public Task<string> GetNewToken(string? accessToken, string refreshToken);

        public  Task RevokeRefreshToken(int id);
    }
}
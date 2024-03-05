using UserService.Core.Interfaces;
using UserService.Core.Interfaces.Auth;
using UserService.Core.Interfaces.Infrastructure;
using UserService.Core.Interfaces.Services;
using UserService.Core.Models;
using UserService.Infrastructure;

namespace UserService.Application.Services
{
    public class AuthService : IAuthService
    {
        private IUserRepository _repository;
        private IJwtProvider _provider;
        private IEmailSender _sender;
        private IPasswordHasher _hasher;
        public AuthService(IUserRepository repository, IPasswordHasher hasher, IJwtProvider provider, IEmailSender sender)
        {
            _hasher = hasher;
            _repository = repository;
            _provider = provider;
            _sender = sender;
        }

        public async Task<string> Login(string email, string password)
        {
            var user = await _repository.GetUserByEmail(email) ?? throw new Exception("No user with this email");
            if(user.VerifiedAt == null)
                throw new Exception("User not verified!");
            var result = _hasher.Verify(password, user.Password);
            if(!result)
                throw new Exception("Password is incorrect");
            var token = _provider.GenerateToken(user);

            return token;
        }

        public async Task<bool> Register(string firstName, string lastName, string email, string password)
        {
            try
            {
                User user = User.Create(firstName, lastName, Generator.GenerateUsername(firstName, lastName, email), email, _hasher.Generate(password));
                
                var id =  await _repository.Create(user);
                var token = Generator.GenerateVerificationToken();
                bool res = await _repository.SetVerificationToken(id, token, DateTime.UtcNow.AddDays(7));
                await _sender.SendVerificationEmailAsync(email, token);
                return res;
            }
            catch(InvalidDataException)
            {
                return false;
            }
        }

        public async Task Verify(string token)
        {
            await _repository.VerifyUser(token);
        }
    }
}
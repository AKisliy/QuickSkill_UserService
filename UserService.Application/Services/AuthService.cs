using UserService.Core.Exceptions;
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
        private readonly IUserRepository _repository;
        private readonly IJwtProvider _provider;
        private readonly IEmailSender _sender;
        private readonly IPasswordHasher _hasher;
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
            return _provider.GenerateToken(user);
        }

        public async Task Register(string firstName, string lastName, string email, string password)
        {
            User user = User.Create(firstName, lastName, Generator.GenerateUsername(firstName, lastName, email), email, _hasher.Generate(password));
            var find = await _repository.GetUserByEmail(email);
            if(find != null)
                throw new Exception("User with this email already exists");

            var token = Generator.GenerateVerificationToken();
            while(!await _repository.IsUniqueVerificationToken(token))
                token = Generator.GenerateVerificationToken();

            await _sender.SendVerificationEmailAsync(email, token);

            var id =  await _repository.Create(user);
            await _repository.SetVerificationToken(id, token, DateTime.UtcNow.AddDays(7));
        }

        public async Task ForgotPassword(string email)
        {
            var user = await _repository.GetUserByEmail(email) ?? throw new NotFoundException("User with this email not found");

            var token = Generator.GenerateVerificationToken();
            while(!await _repository.IsUniqueResetToken(token))
                token = Generator.GenerateVerificationToken();

            await _sender.SendResetEmail(email, token);
            await _repository.SetResetToken(user.Id, token, DateTime.UtcNow.AddDays(1));
        }

        public async Task ResetPassword(string password, string token)
        {
            var passwordHash = _hasher.Generate(password);
            await _repository.ResetPassword(passwordHash, token);
        }

        public async Task Verify(string token)
        {
            await _repository.VerifyUser(token);
        }

        public async Task CheckPassword(int id, string password)
        {
            var user = await _repository.GetUserById(id) ?? throw new NotFoundException($"No user with id: {id}");
            if(!_hasher.Verify(password, user.Password))
                throw new CredentialsException("Password is incorrect!");
        }

        public async Task ChangePassword(int id, string oldPassword, string newPassword)
        {
            var user = await _repository.GetUserById(id) ?? throw new NotFoundException($"No user with id: {id}");
            if(!_hasher.Verify(oldPassword, user.Password))
                throw new CredentialsException("Password is incorrect!");
            var newPasswordHash = _hasher.Generate(newPassword);
            user.Password = newPasswordHash;
            await _repository.Update(user);
        }
    }
}
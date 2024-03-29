using MediatR;
using UserService.Core.Exceptions;
using UserService.Core.Interfaces;
using UserService.Core.Interfaces.Auth;
using UserService.Core.Interfaces.Infrastructure;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Core.Models;
using UserService.Core.Notifications;
using UserService.Infrastructure;

namespace UserService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _provider;
        private readonly IEmailSender _sender;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IMediator _meadiator;
        private readonly IPasswordHasher _hasher;
        public AuthService(IUserRepository userRepository, IBadgeRepository badgeRepository, IPasswordHasher hasher, IJwtProvider provider, IEmailSender sender, IMediator mediator)
        {
            _hasher = hasher;
            _userRepository = userRepository;
            _provider = provider;
            _sender = sender;
            _badgeRepository = badgeRepository;
            _meadiator = mediator;
        }

        public async Task<string> Login(string email, string password)
        {
            var user = await _userRepository.GetUserByEmail(email) ?? throw new NotFoundException("No user with this email");
            if(user.VerifiedAt == null)
                throw new CredentialsException("User not verified!");
            var result = _hasher.Verify(password, user.Password);
            if(!result)
                throw new CredentialsException("Password is incorrect");
            return _provider.GenerateToken(user);
        }

        public async Task Register(string firstName, string lastName, string email, string password)
        {
            var username = Generator.GenerateUsername(firstName, lastName, email);
            User user = User.Create(firstName, lastName, username, email, _hasher.Generate(password));
            var find = await _userRepository.HasUserWithEmail(email);
            if(find)
                throw new ConflictException("User with this email already exists");

            var token = Generator.GenerateVerificationToken();
            while(!await _userRepository.IsUniqueVerificationToken(token))
                token = Generator.GenerateVerificationToken();

            await _sender.SendVerificationEmailAsync(email, token);

            var id =  await _userRepository.Create(user);
            await _userRepository.SetVerificationToken(id, token, DateTime.UtcNow.AddDays(7));
            await _badgeRepository.OnUserCreate(id);

            user.Id = id;
            await _meadiator.Publish(new UserCreatedNotification(user));
        }

        public async Task ForgotPassword(string email)
        {
            var user = await _userRepository.GetUserByEmail(email) ?? throw new NotFoundException("User with this email not found");

            var token = Generator.GenerateVerificationToken();
            while(!await _userRepository.IsUniqueResetToken(token))
                token = Generator.GenerateVerificationToken();

            await _sender.SendResetEmail(email, token);
            await _userRepository.SetResetToken(user.Id, token, DateTime.UtcNow.AddDays(1));
        }

        public async Task ResetPassword(string password, string token)
        {
            var passwordHash = _hasher.Generate(password);
            await _userRepository.ResetPassword(passwordHash, token);
        }

        public async Task Verify(string token)
        {
            await _userRepository.VerifyUser(token);
        }

        public async Task CheckPassword(int id, string password)
        {
            var user = await _userRepository.GetUserById(id) ?? throw new NotFoundException($"No user with id: {id}");
            if(!_hasher.Verify(password, user.Password))
                throw new CredentialsException("Password is incorrect!");
        }

        public async Task ChangePassword(int id, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetUserById(id) ?? throw new NotFoundException($"No user with id: {id}");
            if(!_hasher.Verify(oldPassword, user.Password))
                throw new CredentialsException("Password is incorrect!");
            user.Password = _hasher.Generate(newPassword);
            await _userRepository.Update(user);
        }
    }
}
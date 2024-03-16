using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using UserService.Core.Interfaces.Infrastructure;
using UserService.Infrastructure.Options;

namespace UserService.Infrastructure
{
    public class EmailSender : IEmailSender
    {
        private EmailOptions _options;

        private const string verificationSubject = "Hey, start learning with QuickSkill🔥";
        private const string verificationBody = "Welcome to QuickSkill!! To confirm you registration just follow the link below👇:\n";
        private const string resetSubject = "Reset your password on QuickSkill👀";
        private const string resetBody = "This email was sent, because you're trying to reset your password. Follow the link below to do it👇:\n";

        public EmailSender(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }
        public async Task SendVerificationEmailAsync(string userEmail, string token)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_options.Address));
            email.To.Add(MailboxAddress.Parse(userEmail));
            email.Subject = verificationSubject;
            var baseUrl = "http://localhost:5179/api/auth/verify";
            email.Body = new TextPart(TextFormat.Text) { Text = verificationBody + $"{baseUrl}?token={token}"};

            using var smtp = new SmtpClient();
            smtp.Connect(_options.EmailHost, _options.Port, SecureSocketOptions.SslOnConnect);

            smtp.Authenticate(_options.Username, _options.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task SendResetEmail(string userEmail, string token)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_options.Address));
            email.To.Add(MailboxAddress.Parse(userEmail));
            email.Subject = resetSubject;
            var baseUrl = "http://localhost:5179/api/auth/reset-password";
            email.Body = new TextPart(TextFormat.Text) { Text = resetBody + $"{baseUrl}?token={token}&password=kisliy12"};

            using var smtp = new SmtpClient();
            smtp.Connect(_options.EmailHost, _options.Port, SecureSocketOptions.StartTls);

            smtp.Authenticate(_options.Username, _options.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
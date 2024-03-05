using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using UserService.Core.Interfaces.Infrastructure;

namespace UserService.Infrastructure
{
    public class EmaiSender : IEmailSender
    {
        private IConfiguration _config;

        public EmaiSender(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendVerificationEmailAsync(string userEmail, string token)
        {
            var email = new MimeMessage();
            var options = _config.GetSection("EmailOptions");
            email.From.Add(MailboxAddress.Parse(options["EmailAddress"]));
            email.To.Add(MailboxAddress.Parse(userEmail));
            email.Subject = options["EmailSubject"];
            var baseUrl = "http://localhost:5179/api/auth/verify";
            email.Body = new TextPart(TextFormat.Text) { Text = $"{baseUrl}?token={token}"};

            using var smtp = new SmtpClient();
            smtp.Connect(options["EmailHost"], 587, SecureSocketOptions.StartTls);

            smtp.Authenticate(options["EmailUsername"], options["EmailPassword"]);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
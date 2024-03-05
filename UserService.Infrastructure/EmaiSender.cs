using System.Net;
using System.Net.Mail;
using UserService.Core.Interfaces.Infrastructure;

namespace UserService.Infrastructure
{
    public class EmaiSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string text)
        {
            var mail = "a.kisel20049192@gmail.com";
            var password = "02092004";

            var client = new SmtpClient("gmail.com")
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            return client.SendMailAsync(
                new MailMessage(from: mail,
                                to: email, 
                                subject: subject, 
                                text)
            );
        }
    }
}
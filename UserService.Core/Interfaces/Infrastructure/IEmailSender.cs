using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Core.Interfaces.Infrastructure
{
    public interface IEmailSender
    {
        public Task SendVerificationEmailAsync(string userEmail, string token);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Infrastructure.Options
{
    public class EmailOptions
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int Port { get; set; }

        public string Address { get; set; } = string.Empty;

        public string EmailHost { get; set; } = string.Empty;
    } 
}
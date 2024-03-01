using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Infrastructure
{
    public class Generator
    {
        public static string GenerateUsername(string firstName, string lastName, string email)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(firstName.Substring(0, Math.Min(3, firstName.Length)));
            sb.Append(lastName.Substring(0, Math.Min(4, lastName.Length)));
            sb.Append(DateTime.UtcNow.ToString().Substring(0, 2));
            return sb.ToString();
        }
    }
}
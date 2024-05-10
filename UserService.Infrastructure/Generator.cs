using System.Security.Cryptography;
using System.Text;

namespace UserService.Infrastructure
{
    public static class Generator
    {
        public static string GenerateUsername(string firstName, string lastName, string email)
        {
            StringBuilder sb = new();
            sb.Append(firstName, 0, Math.Min(3, firstName.Length));
            sb.Append(lastName, 0, Math.Min(4, lastName.Length));
            sb.Append(DateTime.UtcNow.ToString(), 0, 2);
            return sb.ToString();
        }

        public static string GenerateVerificationToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        }
    }
}
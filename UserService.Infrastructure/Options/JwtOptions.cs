namespace UserService.Infrastructure
{
    public class JwtOptions
    {
        public string SecretKey { get; set; } = string.Empty;
        public int ExpiresMinutes { get; set; }

        public bool ValidateAudience { get; set; }

        public bool ValidateIssuer { get; set; }

        public bool ValidateIssuerSigningKey { get; set; }

        public bool ValidateLifetime { get; set; }
    }
}
namespace UserService.Core.Models
{
    public class TokensLogin
    {
        public required string JwtToken { get; set; }

        public required string RefreshToken { get; set; }
    }
}
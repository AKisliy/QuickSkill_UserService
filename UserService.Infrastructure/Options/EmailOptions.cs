namespace UserService.Infrastructure.Options
{
    public class EmailOptions
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int Port { get; set; }

        public string Address { get; set; } = string.Empty;

        public string EmailHost { get; set; } = string.Empty;

        public string BaseVerifyUrl { get; set; } = string.Empty;

        public string BaseResetUrl { get; set; } = string.Empty;
    }
}
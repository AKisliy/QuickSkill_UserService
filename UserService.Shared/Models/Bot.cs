namespace Shared
{
    public class Bot
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string? Photo { get; set; }
    }
}
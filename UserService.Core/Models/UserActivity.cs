namespace UserService.Core.Models
{
    public class UserActivity
    {
        public int UserId { get; set; }

        public DateOnly ActivityDate { get; set; }

        public string? ActivityType { get; set; }
    }
}
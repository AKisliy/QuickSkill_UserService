namespace UserService.Core.Models
{
    public class UserBadge
    {
        public int UserId { get; set; }

        public int BadgeId { get; set; }

        public int Progress { get; set; }

        public bool Achieved { get; set; }

        public virtual Badge Badge { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
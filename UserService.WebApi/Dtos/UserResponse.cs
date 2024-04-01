namespace UserService.WebApi.Dtos
{
    public class UserResponse
    {
        public string Firstname { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string Username { get; set; } = null!;

        public int Xp { get; set; }

        public int Userlevel { get; set; }

        public int Streak { get; set; }

        public int MaxStreak { get; set; }

        public string? Description { get; set; }

        public string? Photo { get; set; }

        public string? GoalText { get; set; }

        public int? GoalDays { get; set; }

        public string Status { get; set; } = null!;

        public int Freezer { get; set; }

        public short Hearts { get; set; }

        public int Crystall { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.DataAccess.Entities
{
    public partial class UserEntity
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Username { get; set; } = null!;

        public int? Xp { get; set; }

        public int? UserLevel { get; set; }

        public string? Description { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Photo { get; set; }

        public string? GoalText { get; set; }

        public int? GoalDays { get; set; }

        public string? Status { get; set; }

        public string? VerificationToken { get; set; }

        public DateTime? VerificationTokenExpires { get; set; }

        public string? ResetToken { get; set; }

        public DateTime? ResetTokenExpires { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public virtual ICollection<UserBadgeEntity> UserBadges { get; set; } = new List<UserBadgeEntity>();

        public virtual ICollection<UserActivityEntity> UserActivities { get; set; } = new List<UserActivityEntity>();
    }   
}
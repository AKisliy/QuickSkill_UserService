using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.DataAccess.Entities
{
    public partial class BadgeEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int? Required { get; set; }

        public string TaskToAchieve { get; set; } = null!;

        public string? Photo { get; set; }

        public string? GrayPhoto { get; set; }

        public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    }
}
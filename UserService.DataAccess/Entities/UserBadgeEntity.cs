using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.DataAccess.Entities
{
    public class UserBadgeEntity
    {   
        public int UserId { get; set; }

        public int BadgeId { get; set; }

        public int Progress { get; set; }

        public bool Achieved { get; set; }

        public virtual BadgeEntity Badge { get; set; } = null!;

        public virtual UserEntity User { get; set; } = null!;
    }
}
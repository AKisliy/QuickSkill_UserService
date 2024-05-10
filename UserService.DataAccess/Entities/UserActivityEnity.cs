using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.DataAccess.Entities
{
    public class UserActivityEntity
    {
        public int UserId { get; set; }

        public DateOnly ActivityDate { get; set; }

        public string? ActivityType { get; set; }

        public virtual UserEntity User { get; set; } = null!;
    }
}
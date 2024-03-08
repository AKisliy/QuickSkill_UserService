using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Core.Models
{
    public class UserActivity
    {
        public int UserId { get; set; }

        public DateOnly ActivityDate { get; set; }

        public string? ActivityType { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.WebApi.Dtos
{
    public class UserBadgeRequest
    {
        public int UserId { get; set; }
        public int BadgeId { get; set; }
        public int Progress { get; set; }
    }
}
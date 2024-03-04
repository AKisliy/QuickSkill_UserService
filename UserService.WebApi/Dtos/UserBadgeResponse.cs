using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.WebApi.Dtos
{
    public class UserBadgeResponse
    {
        public int UserId { get; set; }

        public string Name { get; set; } = null!;

        public int Progress { get; set; }

        public int? Required { get; set; }

        public bool Achieved { get; set; }

        public string TaskToAchieve { get; set; } = null!;

        public string? Photo { get; set; }

        public string? GrayPhoto { get; set; }
    }
}
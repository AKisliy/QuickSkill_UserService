using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.WebApi.Dtos
{
    public class UserResponse
    {
        public int Id { get; set; }

        public string Firstname { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string Username { get; set; } = null!;

        public int? Xp { get; set; }

        public int? Userlevel { get; set; }

        public string? Description { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Photo { get; set; }

        public string? Goaltext { get; set; }

        public int? Goaldays { get; set; }

        public string? Status { get; set; }
    }
}
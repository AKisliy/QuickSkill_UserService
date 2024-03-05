using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.WebApi.Dtos
{
    public class UserLoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required, MinLength(8, ErrorMessage = "Password should contain at least 8 symbols!")]
        public string Password { get; set; } = null!;
    }
}
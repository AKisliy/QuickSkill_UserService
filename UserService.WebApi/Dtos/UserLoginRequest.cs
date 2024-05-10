using System.ComponentModel.DataAnnotations;

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
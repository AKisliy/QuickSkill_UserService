using System.ComponentModel.DataAnnotations;

namespace UserService.WebApi.Dtos
{
    public class UserRegisterRequest
    {
        [Required]
        public string Firstname { get; set; } = null!;
        [Required]
        public string Lastname { get; set; } = null!;
        [Required, EmailAddress(ErrorMessage = "Please, provide correct email address!")]
        public string Email { get; set; } = null!;
        [Required, MinLength(8, ErrorMessage = "Password should contain at least 8 sybmols!")]
        public string Password { get; set; } = null!;
    }
}
using System.ComponentModel.DataAnnotations;

namespace Talabat.API.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
            ErrorMessage = "password must has minimum 8 characters, At least one uppercase English letter, At least one lowercase English letter, At least one digit and At least one special character")]
        public string Password { get; set; }
    }
}

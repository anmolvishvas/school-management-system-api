using System.ComponentModel.DataAnnotations;

namespace SchoolManagementAPI.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string Role { get; set; } = "Teacher";
    }
}
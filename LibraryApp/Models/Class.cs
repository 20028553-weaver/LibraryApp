using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = "";
        [Required]
        public string Role { get; set; }
    }
}
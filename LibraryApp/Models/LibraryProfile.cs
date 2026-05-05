using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class LibraryProfile
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public string? Hours { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public string? LogoPath { get; set; }
    }
}
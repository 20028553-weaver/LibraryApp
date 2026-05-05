using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        public string? Genre { get; set; }

        [Required]
        public string ISBN { get; set; } = string.Empty;

        public string? CoverImagePath { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int TotalCopies { get; set; } = 1;
        public int AvailableCopies { get; set; } = 1;
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        public string? Publisher { get; set; }
        public int? YearPublished { get; set; }
    }
}

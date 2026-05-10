using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(150, ErrorMessage = "Author name cannot exceed 150 characters.")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN is required.")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN must be between 10 and 13 digits.")]
        public string ISBN { get; set; } = string.Empty;

        public string? CoverImagePath { get; set; }
        public bool IsAvailable { get; set; } = true;

        [Range(1, 9999, ErrorMessage = "Total copies must be at least 1.")]
        public int TotalCopies { get; set; } = 1;

        public int AvailableCopies { get; set; } = 1;
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        public string? Publisher { get; set; }

        [Range(1900, 2100, ErrorMessage = "Please enter a valid year.")]
        public int? YearPublished { get; set; }

        // Navigation properties
        public ICollection<BorrowTransaction> BorrowTransactions { get; set; } = new List<BorrowTransaction>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
    }
}

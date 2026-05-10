using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Genre name is required.")]
        [StringLength(100, ErrorMessage = "Genre name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        // Many-to-many with Book
        public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
    }
}
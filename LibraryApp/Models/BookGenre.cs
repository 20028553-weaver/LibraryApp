namespace LibraryApp.Models
{
    public class BookGenre
    {
        public int BookId { get; set; }
        public int GenreId { get; set; }

        // Navigation properties
        public Book Book { get; set; } = null!;
        public Genre Genre { get; set; } = null!;
    }
}
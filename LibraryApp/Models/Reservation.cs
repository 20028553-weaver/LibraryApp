using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int MemberId { get; set; }

        public DateTime ReservedDate { get; set; } = DateTime.Now;

        public DateTime? ExpiryDate { get; set; }

        public string Status { get; set; } = "Pending";

        // Navigation properties
        public Book Book { get; set; } = null!;
        public Member Member { get; set; } = null!;
    }
}

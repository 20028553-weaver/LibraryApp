using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Penalty
    {
        public int Id { get; set; }

        [Required]
        public int BorrowTransactionId { get; set; }

        [Required]
        public int MemberId { get; set; }

        public decimal Amount { get; set; } = 0;

        public DateTime DateIssued { get; set; } = DateTime.Now;

        public DateTime? DatePaid { get; set; }

        public string Status { get; set; } = "Unpaid";

        // Navigation properties
        public BorrowTransaction BorrowTransaction { get; set; } = null!;
        public Member Member { get; set; } = null!;
    }
}
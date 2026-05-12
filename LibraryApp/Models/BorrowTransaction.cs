using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class BorrowTransaction
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int MemberId { get; set; }

        public int? AdminId { get; set; }

        public DateTime BorrowDate { get; set; } = DateTime.Now;

        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public int RenewCount { get; set; } = 0;

        public string Status { get; set; } = "Borrowed";

        public Book? Book { get; set; }
        public Member? Member { get; set; }
        public Admin? Admin { get; set; }
        public Penalty? Penalty { get; set; }
    }
}
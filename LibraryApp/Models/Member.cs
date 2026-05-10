using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public DateTime MembershipDate { get; set; } = DateTime.Now;

        public DateTime? MembershipExpiry { get; set; }

        [Required]
        public string Status { get; set; } = "Active";

        // Navigation properties
        public ICollection<BorrowTransaction> BorrowTransactions { get; set; } = new List<BorrowTransaction>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();
    }
}
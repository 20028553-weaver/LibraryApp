using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(150, ErrorMessage = "Name cannot exceed 150 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string? Phone { get; set; }

        [StringLength(300, ErrorMessage = "Address cannot exceed 300 characters.")]
        public string? Address { get; set; }

        public DateTime MembershipDate { get; set; } = DateTime.Now;
        public DateTime? MembershipExpiry { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = "Active";

        public string PasswordHash { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<BorrowTransaction> BorrowTransactions { get; set; } = new List<BorrowTransaction>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();
    }
}
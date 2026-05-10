using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class BorrowingConfig
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Loan duration is required.")]
        [Range(1, 365, ErrorMessage = "Loan duration must be between 1 and 365 days.")]
        public int LoanDurationDays { get; set; } = 14;

        [Range(0, 20, ErrorMessage = "Max renewals cannot exceed 20.")]
        public int MaxRenewals { get; set; } = 2;

        [Range(1, 100, ErrorMessage = "Max items must be between 1 and 100.")]
        public int MaxItemsPerMember { get; set; } = 5;

        [Required(ErrorMessage = "Daily penalty is required.")]
        [Range(0, 9999, ErrorMessage = "Please enter a valid penalty amount.")]
        public decimal DailyPenalty { get; set; } = 5.00m;

        public decimal? MaxPenaltyCap { get; set; }

        [Range(0, 30, ErrorMessage = "Grace period cannot exceed 30 days.")]
        public int GracePeriodDays { get; set; } = 0;

        public bool AllowRenewals { get; set; } = true;
        public bool AllowReservations { get; set; } = true;
    }
}
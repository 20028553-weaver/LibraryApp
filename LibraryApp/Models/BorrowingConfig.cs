using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class BorrowingConfig
    {
        public int Id { get; set; }
        public int LoanDurationDays { get; set; } = 14;
        public int MaxRenewals { get; set; } = 2;
        public int MaxItemsPerMember { get; set; } = 5;
        public decimal DailyPenalty { get; set; } = 5.00m;
        public decimal? MaxPenaltyCap { get; set; }
        public int GracePeriodDays { get; set; } = 0;
        public bool AllowRenewals { get; set; } = true;
        public bool AllowReservations { get; set; } = true;
    }
}
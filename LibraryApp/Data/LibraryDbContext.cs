using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;

namespace LibraryApp.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<BorrowTransaction> BorrowTransactions { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Penalty> Penalties { get; set; }
        public DbSet<LibraryProfile> LibraryProfiles { get; set; }
        public DbSet<BorrowingConfig> BorrowingConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Book → BorrowTransaction (one to many)
            modelBuilder.Entity<BorrowTransaction>()
                .HasOne(b => b.Book)
                .WithMany(b => b.BorrowTransactions)
                .HasForeignKey(b => b.BookId);

            // Member → BorrowTransaction (one to many)
            modelBuilder.Entity<BorrowTransaction>()
                .HasOne(b => b.Member)
                .WithMany(m => m.BorrowTransactions)
                .HasForeignKey(b => b.MemberId);

            // Admin → BorrowTransaction (one to many)
            modelBuilder.Entity<BorrowTransaction>()
                .HasOne(b => b.Admin)
                .WithMany(a => a.BorrowTransactions)
                .HasForeignKey(b => b.AdminId);

            // BorrowTransaction → Penalty (one to one)
            modelBuilder.Entity<Penalty>()
                .HasOne(p => p.BorrowTransaction)
                .WithOne(b => b.Penalty)
                .HasForeignKey<Penalty>(p => p.BorrowTransactionId);

            // Member → Penalty (one to many)
            // NoAction breaks the multiple-cascade-path: Member→BorrowTransaction→Penalty and Member→Penalty
            modelBuilder.Entity<Penalty>()
                .HasOne(p => p.Member)
                .WithMany(m => m.Penalties)
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.NoAction);

            // Book → Reservation (one to many)
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reservations)
                .HasForeignKey(r => r.BookId);

            // Member → Reservation (one to many)
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Member)
                .WithMany(m => m.Reservations)
                .HasForeignKey(r => r.MemberId);

            // Decimal precision
            modelBuilder.Entity<BorrowingConfig>()
                .Property(b => b.DailyPenalty)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<BorrowingConfig>()
                .Property(b => b.MaxPenaltyCap)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Penalty>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(10,2)");
        }
    }
}

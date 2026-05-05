using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;

namespace LibraryApp.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<LibraryProfile> LibraryProfiles { get; set; }
        public DbSet<BorrowingConfig> BorrowingConfigs { get; set; }
    }
}

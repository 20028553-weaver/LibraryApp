using LibraryApp.Data;
using LibraryApp.Models;
using System.Security.Cryptography;
using System.Text;

namespace LibraryApp
{
    public static class SeedData
    {
        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public static void Initialize(LibraryDbContext context)
        {
            // Always ensure admin has the correct demo password
            var expectedAdminHash = HashPassword("admin123");
            var existingAdmin = context.Admins.FirstOrDefault();
            if (existingAdmin != null && existingAdmin.PasswordHash != expectedAdminHash)
            {
                existingAdmin.PasswordHash = expectedAdminHash;
                context.SaveChanges();
            }

            // Always ensure seeded members have a working password
            foreach (var m in context.Members.Where(m => string.IsNullOrEmpty(m.PasswordHash)))
            {
                m.PasswordHash = HashPassword("member123");
            }
            if (context.ChangeTracker.HasChanges())
                context.SaveChanges();

            if (!context.BorrowingConfigs.Any())
            {
                context.BorrowingConfigs.Add(new BorrowingConfig
                {
                    LoanDurationDays = 14,
                    MaxRenewals = 2,
                    MaxItemsPerMember = 5,
                    DailyPenalty = 5.00m,
                    GracePeriodDays = 0,
                    AllowRenewals = true,
                    AllowReservations = true
                });
                context.SaveChanges();
            }

            if (!context.LibraryProfiles.Any())
            {
                context.LibraryProfiles.Add(new LibraryProfile
                {
                    Name = "Nepal Public Library",
                    Location = "Kathmandu, Nepal",
                    Hours = "Sun–Fri 9:00 AM – 6:00 PM",
                    ContactNumber = "+977-1-4xxxxxx",
                    Email = "info@nepallibrary.gov.np",
                    Website = "https://nepallibrary.gov.np",
                    Description = "A public library serving the community of Kathmandu."
                });
                context.SaveChanges();
            }

            if (!context.Genres.Any())
            {
                context.Genres.AddRange(
                    new Genre { Name = "Fiction" },
                    new Genre { Name = "Non-Fiction" },
                    new Genre { Name = "Science" },
                    new Genre { Name = "History" }
                );
                context.SaveChanges();
            }

            if (!context.Books.Any())
            {
                context.Books.AddRange(
                    new Book
                    {
                        Title = "Harry Potter and the Philosopher's Stone",
                        Author = "J.K. Rowling",
                        ISBN = "9780439708180",
                        TotalCopies = 3,
                        AvailableCopies = 3,
                        IsAvailable = true,
                        DateAdded = DateTime.Now,
                        Publisher = "Bloomsbury",
                        YearPublished = 1997,
                        Description = "A young boy discovers he is a wizard."
                    },
                    new Book
                    {
                        Title = "The Great Gatsby",
                        Author = "F. Scott Fitzgerald",
                        ISBN = "9780743273565",
                        TotalCopies = 2,
                        AvailableCopies = 2,
                        IsAvailable = true,
                        DateAdded = DateTime.Now,
                        Publisher = "Scribner",
                        YearPublished = 1925,
                        Description = "A story of wealth and love in the 1920s."
                    },
                    new Book
                    {
                        Title = "To Kill a Mockingbird",
                        Author = "Harper Lee",
                        ISBN = "9780061935466",
                        TotalCopies = 2,
                        AvailableCopies = 1,
                        IsAvailable = true,
                        DateAdded = DateTime.Now,
                        Publisher = "HarperCollins",
                        YearPublished = 1960,
                        Description = "A story about racial injustice in the American South."
                    },
                    new Book
                    {
                        Title = "1984",
                        Author = "George Orwell",
                        ISBN = "9780451524935",
                        TotalCopies = 2,
                        AvailableCopies = 0,
                        IsAvailable = false,
                        DateAdded = DateTime.Now,
                        Publisher = "Signet Classic",
                        YearPublished = 1949,
                        Description = "A dystopian novel about totalitarianism."
                    }
                );
                context.SaveChanges();
            }

            if (!context.Admins.Any())
            {
                context.Admins.Add(new Admin
                {
                    FullName = "Library Admin",
                    Email = "admin@library.com",
                    PasswordHash = HashPassword("admin123"),
                    Role = "Admin"
                });
                context.SaveChanges();
            }

            if (!context.Members.Any())
            {
                context.Members.AddRange(
                    new Member
                    {
                        FullName = "John Smith",
                        Email = "john@email.com",
                        Phone = "0412345678",
                        Address = "123 Main St, Kathmandu",
                        MembershipDate = DateTime.Now.AddMonths(-6),
                        MembershipExpiry = DateTime.Now.AddYears(1),
                        Status = "Active",
                        PasswordHash = HashPassword("member123")
                    },
                    new Member
                    {
                        FullName = "Sarah Johnson",
                        Email = "sarah@email.com",
                        Phone = "0498765432",
                        Address = "456 Park Ave, Pokhara",
                        MembershipDate = DateTime.Now.AddMonths(-3),
                        MembershipExpiry = DateTime.Now.AddYears(1),
                        Status = "Active",
                        PasswordHash = HashPassword("member123")
                    },
                    new Member
                    {
                        FullName = "Mike Davis",
                        Email = "mike@email.com",
                        Phone = "0411111111",
                        Address = "789 Queen St, Lalitpur",
                        MembershipDate = DateTime.Now.AddMonths(-1),
                        MembershipExpiry = DateTime.Now.AddYears(1),
                        Status = "Active",
                        PasswordHash = HashPassword("member123")
                    }
                );
                context.SaveChanges();
            }

            if (!context.BorrowTransactions.Any())
            {
                var book1 = context.Books.First();
                var book4 = context.Books.FirstOrDefault(b => b.Title == "1984");
                var member1 = context.Members.First();
                var member2 = context.Members.Skip(1).First();
                var admin = context.Admins.First();

                context.BorrowTransactions.Add(new BorrowTransaction
                {
                    BookId = book1.Id,
                    MemberId = member1.Id,
                    AdminId = admin.Id,
                    BorrowDate = DateTime.Now.AddDays(-5),
                    DueDate = DateTime.Now.AddDays(9),
                    Status = "Borrowed",
                    RenewCount = 0
                });

                if (book4 != null)
                {
                    context.BorrowTransactions.Add(new BorrowTransaction
                    {
                        BookId = book4.Id,
                        MemberId = member2.Id,
                        AdminId = admin.Id,
                        BorrowDate = DateTime.Now.AddDays(-20),
                        DueDate = DateTime.Now.AddDays(-6),
                        Status = "Borrowed",
                        RenewCount = 1
                    });
                }

                context.SaveChanges();
            }
        }
    }
}

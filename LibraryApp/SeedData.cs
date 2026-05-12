using LibraryApp.Data;
using LibraryApp.Models;

namespace LibraryApp
{
    public static class SeedData
    {
        public static void Initialize(LibraryDbContext context)
        {
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
                        Title = "Harry Potter and the Philosophers Stone",
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
                        Description = "A story about racial injustice."
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
                    Email = "admin@library.com"
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
                        Address = "123 Main St Sydney",
                        MembershipDate = DateTime.Now.AddMonths(-6),
                        MembershipExpiry = DateTime.Now.AddYears(1),
                        Status = "Active"
                    },
                    new Member
                    {
                        FullName = "Sarah Johnson",
                        Email = "sarah@email.com",
                        Phone = "0498765432",
                        Address = "456 Park Ave Melbourne",
                        MembershipDate = DateTime.Now.AddMonths(-3),
                        MembershipExpiry = DateTime.Now.AddYears(1),
                        Status = "Active"
                    },
                    new Member
                    {
                        FullName = "Mike Davis",
                        Email = "mike@email.com",
                        Phone = "0411111111",
                        Address = "789 Queen St Brisbane",
                        MembershipDate = DateTime.Now.AddMonths(-1),
                        MembershipExpiry = DateTime.Now.AddYears(1),
                        Status = "Active"
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
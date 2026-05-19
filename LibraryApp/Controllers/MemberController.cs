using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;
using System.Security.Claims;

namespace LibraryApp.Controllers
{
    public class MemberController : Controller
    {
        private readonly LibraryDbContext _context;

        public MemberController(LibraryDbContext context)
        {
            _context = context;
        }

        private int? GetMemberId()
        {
            var claim = User.FindFirst("MemberId")?.Value;
            return int.TryParse(claim, out var id) ? id : (int?)null;
        }

        private IActionResult RequireLogin()
        {
            TempData["Error"] = "Please log in to access the member portal.";
            return RedirectToAction("Login", "Account");
        }

        // GET: Member/Index
        public async Task<IActionResult> Index()
        {
            var memberId = GetMemberId();
            if (memberId == null) return RequireLogin();

            var member = await _context.Members
                .Include(m => m.BorrowTransactions).ThenInclude(bt => bt.Book)
                .Include(m => m.Reservations).ThenInclude(r => r.Book)
                .Include(m => m.Penalties)
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null) return RequireLogin();

            ViewBag.Books = await _context.Books
                .OrderByDescending(b => b.DateAdded)
                .Take(8)
                .ToListAsync();

            return View(member);
        }

        // GET: Member/Browse
        public async Task<IActionResult> Browse(string? search)
        {
            var memberId = GetMemberId();
            if (memberId == null) return RequireLogin();

            var books = _context.Books.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                books = books.Where(b =>
                    b.Title.Contains(search) ||
                    b.Author.Contains(search) ||
                    b.ISBN.Contains(search));

            ViewBag.Search = search;
            return View(await books.OrderBy(b => b.Title).ToListAsync());
        }

        // POST: Member/Borrow
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrow(int bookId)
        {
            var memberId = GetMemberId();
            if (memberId == null) return RequireLogin();

            var book = await _context.Books.FindAsync(bookId);
            if (book == null || book.AvailableCopies <= 0)
            {
                TempData["Error"] = "This book is not available for borrowing.";
                return RedirectToAction(nameof(Browse));
            }

            var config = await _context.BorrowingConfigs.FirstOrDefaultAsync();
            var loanDays = config?.LoanDurationDays ?? 14;

            var transaction = new BorrowTransaction
            {
                BookId = bookId,
                MemberId = memberId.Value,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(loanDays),
                Status = "Borrowed",
                RenewCount = 0
            };

            book.AvailableCopies--;
            if (book.AvailableCopies == 0) book.IsAvailable = false;

            _context.BorrowTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"'{book.Title}' borrowed! Due: {transaction.DueDate:dd MMM yyyy}";
            return RedirectToAction(nameof(MyBorrows));
        }

        // POST: Member/Reserve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(int bookId)
        {
            var memberId = GetMemberId();
            if (memberId == null) return RequireLogin();

            var existing = await _context.Reservations.AnyAsync(r =>
                r.BookId == bookId && r.MemberId == memberId && r.Status == "Pending");

            if (existing)
            {
                TempData["Warning"] = "You already have a pending reservation for this book.";
                return RedirectToAction(nameof(Browse));
            }

            _context.Reservations.Add(new Reservation
            {
                BookId = bookId,
                MemberId = memberId.Value,
                ReservedDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(7),
                Status = "Pending"
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = "Reservation placed successfully!";
            return RedirectToAction(nameof(MyReservations));
        }

        // GET: Member/MyBorrows
        public async Task<IActionResult> MyBorrows()
        {
            var memberId = GetMemberId();
            if (memberId == null) return RequireLogin();

            var borrows = await _context.BorrowTransactions
                .Include(bt => bt.Book)
                .Where(bt => bt.MemberId == memberId)
                .OrderByDescending(bt => bt.BorrowDate)
                .ToListAsync();

            return View(borrows);
        }

        // GET: Member/MyReservations
        public async Task<IActionResult> MyReservations()
        {
            var memberId = GetMemberId();
            if (memberId == null) return RequireLogin();

            var reservations = await _context.Reservations
                .Include(r => r.Book)
                .Where(r => r.MemberId == memberId)
                .OrderByDescending(r => r.ReservedDate)
                .ToListAsync();

            return View(reservations);
        }

        // POST: Member/CancelReservation/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var memberId = GetMemberId();
            if (memberId == null) return RequireLogin();

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == id && r.MemberId == memberId);

            if (reservation == null) return NotFound();

            reservation.Status = "Cancelled";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Reservation cancelled.";
            return RedirectToAction(nameof(MyReservations));
        }

        // GET: Member/MyPenalties
        public async Task<IActionResult> MyPenalties()
        {
            var memberId = GetMemberId();
            if (memberId == null) return RequireLogin();

            var penalties = await _context.Penalties
                .Include(p => p.BorrowTransaction).ThenInclude(bt => bt.Book)
                .Where(p => p.MemberId == memberId)
                .OrderByDescending(p => p.DateIssued)
                .ToListAsync();

            ViewBag.Config = await _context.BorrowingConfigs.FirstOrDefaultAsync();
            return View(penalties);
        }
    }
}

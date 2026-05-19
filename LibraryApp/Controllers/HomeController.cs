using System.Diagnostics;
using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryDbContext _context;

        public HomeController(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "Account");

            if (role == "Member")
                return RedirectToAction("Index", "Member");

            var config = await _context.BorrowingConfigs.FirstOrDefaultAsync();
            var dailyPenalty = config?.DailyPenalty ?? 5.00m;

            ViewBag.TotalBooks = await _context.Books.CountAsync();
            ViewBag.TotalMembers = await _context.Members.CountAsync();

            var overdueList = await _context.BorrowTransactions
                .Where(b => b.DueDate < DateTime.Today && b.ReturnDate == null)
                .ToListAsync();
            ViewBag.OverdueCount = overdueList.Count;
            ViewBag.TotalFines = overdueList.Sum(b =>
                (int)(DateTime.Today - b.DueDate).TotalDays * dailyPenalty);

            ViewBag.ActiveBorrows = await _context.BorrowTransactions
                .CountAsync(b => b.Status == "Borrowed" && b.ReturnDate == null);

            ViewBag.RecentBooks = await _context.Books
                .OrderByDescending(b => b.DateAdded)
                .Take(4)
                .ToListAsync();

            ViewBag.RecentTransactions = await _context.BorrowTransactions
                .Include(b => b.Book)
                .Include(b => b.Member)
                .OrderByDescending(b => b.BorrowDate)
                .Take(6)
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

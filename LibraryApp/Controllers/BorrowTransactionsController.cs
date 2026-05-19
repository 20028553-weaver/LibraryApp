using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;

namespace LibraryApp.Controllers
{
    public class BorrowTransactionsController : Controller
    {
        private readonly LibraryDbContext _context;

        public BorrowTransactionsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: BorrowTransactions
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.BorrowTransactions
                .Include(b => b.Admin)
                .Include(b => b.Book)
                .Include(b => b.Member)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();
            return View(transactions);
        }

        // GET: BorrowTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var bt = await _context.BorrowTransactions
                .Include(b => b.Admin).Include(b => b.Book).Include(b => b.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bt == null) return NotFound();
            return View(bt);
        }

        // GET: BorrowTransactions/IssueBook
        public async Task<IActionResult> IssueBook()
        {
            var config = await _context.BorrowingConfigs.FirstOrDefaultAsync();
            ViewBag.Config = config;
            ViewBag.BookId = new SelectList(
                _context.Books.Where(b => b.AvailableCopies > 0).OrderBy(b => b.Title),
                "Id", "Title");
            ViewBag.MemberId = new SelectList(
                _context.Members.Where(m => m.Status == "Active").OrderBy(m => m.FullName),
                "Id", "FullName");
            ViewBag.AdminId = new SelectList(_context.Admins, "Id", "FullName");
            ViewBag.TodayIssued = await _context.BorrowTransactions
                .Include(b => b.Book).Include(b => b.Member)
                .Where(b => b.BorrowDate.Date == DateTime.Today)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();
            return View();
        }

        // POST: BorrowTransactions/IssueBook
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IssueBook([Bind("BookId,MemberId,AdminId")] BorrowTransaction bt)
        {
            var config = await _context.BorrowingConfigs.FirstOrDefaultAsync();
            var book = await _context.Books.FindAsync(bt.BookId);

            if (book == null || book.AvailableCopies <= 0)
            {
                TempData["Error"] = "Selected book is not available.";
                return RedirectToAction(nameof(IssueBook));
            }

            if (ModelState.IsValid)
            {
                var loanDays = config?.LoanDurationDays ?? 14;
                bt.BorrowDate = DateTime.Now;
                bt.DueDate = DateTime.Now.AddDays(loanDays);
                bt.Status = "Borrowed";
                bt.RenewCount = 0;

                book.AvailableCopies--;
                if (book.AvailableCopies == 0) book.IsAvailable = false;

                _context.Add(bt);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"'{book.Title}' issued successfully. Due: {bt.DueDate:dd MMM yyyy}";
                return RedirectToAction(nameof(IssueBook));
            }

            return RedirectToAction(nameof(IssueBook));
        }

        // GET: BorrowTransactions/Overdue
        public async Task<IActionResult> Overdue()
        {
            var config = await _context.BorrowingConfigs.FirstOrDefaultAsync();
            ViewBag.Config = config;
            var overdue = await _context.BorrowTransactions
                .Include(b => b.Book)
                .Include(b => b.Member)
                .Where(b => b.DueDate < DateTime.Today && b.ReturnDate == null)
                .OrderBy(b => b.DueDate)
                .ToListAsync();
            return View(overdue);
        }

        // POST: BorrowTransactions/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id)
        {
            var bt = await _context.BorrowTransactions
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bt == null) return NotFound();

            bt.ReturnDate = DateTime.Now;
            bt.Status = "Returned";

            if (bt.Book != null)
            {
                bt.Book.AvailableCopies++;
                if (bt.Book.AvailableCopies > 0)
                    bt.Book.IsAvailable = true;
            }

            // Create penalty if overdue and none exists
            if (bt.DueDate < DateTime.Now)
            {
                var config = await _context.BorrowingConfigs.FirstOrDefaultAsync();
                var dailyPenalty = config?.DailyPenalty ?? 5.00m;
                var graceDays = config?.GracePeriodDays ?? 0;
                var maxCap = config?.MaxPenaltyCap;

                var graceCutoff = bt.DueDate.AddDays(graceDays);
                var daysOverdue = (int)(DateTime.Now - graceCutoff).TotalDays;

                if (daysOverdue > 0)
                {
                    var exists = await _context.Penalties.AnyAsync(p => p.BorrowTransactionId == id);
                    if (!exists)
                    {
                        decimal amount = daysOverdue * dailyPenalty;
                        if (maxCap.HasValue && amount > maxCap.Value)
                            amount = maxCap.Value;

                        _context.Penalties.Add(new Penalty
                        {
                            BorrowTransactionId = id,
                            MemberId = bt.MemberId,
                            Amount = amount,
                            DateIssued = DateTime.Now,
                            Status = "Unpaid"
                        });
                        TempData["Warning"] = $"Book returned with a ${amount:F2} penalty generated.";
                    }
                }
            }

            await _context.SaveChangesAsync();
            if (TempData["Warning"] == null)
                TempData["Success"] = "Book returned successfully.";
            return RedirectToAction(nameof(Overdue));
        }

        // GET: BorrowTransactions/Create
        public IActionResult Create()
        {
            ViewData["AdminId"] = new SelectList(_context.Admins, "Id", "Email");
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title");
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName");
            return View();
        }

        // POST: BorrowTransactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,MemberId,AdminId,BorrowDate,DueDate,ReturnDate,RenewCount,Status")] BorrowTransaction bt)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminId"] = new SelectList(_context.Admins, "Id", "Email", bt.AdminId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", bt.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName", bt.MemberId);
            return View(bt);
        }

        // GET: BorrowTransactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var bt = await _context.BorrowTransactions.FindAsync(id);
            if (bt == null) return NotFound();
            ViewData["AdminId"] = new SelectList(_context.Admins, "Id", "Email", bt.AdminId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", bt.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName", bt.MemberId);
            return View(bt);
        }

        // POST: BorrowTransactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,MemberId,AdminId,BorrowDate,DueDate,ReturnDate,RenewCount,Status")] BorrowTransaction bt)
        {
            if (id != bt.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(bt); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.BorrowTransactions.Any(e => e.Id == bt.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminId"] = new SelectList(_context.Admins, "Id", "Email", bt.AdminId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", bt.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName", bt.MemberId);
            return View(bt);
        }

        // GET: BorrowTransactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var bt = await _context.BorrowTransactions
                .Include(b => b.Admin).Include(b => b.Book).Include(b => b.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bt == null) return NotFound();
            return View(bt);
        }

        // POST: BorrowTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bt = await _context.BorrowTransactions.FindAsync(id);
            if (bt != null) _context.BorrowTransactions.Remove(bt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

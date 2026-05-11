using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;

namespace LibraryApp.Controllers
{
    public class PenaltiesController : Controller
    {
        private readonly LibraryDbContext _context;

        public PenaltiesController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Penalties
        public async Task<IActionResult> Index()
        {
            var penalties = await _context.Penalties
                .Include(p => p.Member)
                .Include(p => p.BorrowTransaction)
                    .ThenInclude(b => b.Book)
                .OrderBy(p => p.Status)
                .ToListAsync();

            return View(penalties);
        }

        // GET: Penalties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var penalty = await _context.Penalties
                .Include(p => p.Member)
                .Include(p => p.BorrowTransaction)
                    .ThenInclude(b => b.Book)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (penalty == null) return NotFound();

            return View(penalty);
        }

        // POST: Penalties/GenerateAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateAll()
        {
            var config = await _context.BorrowingConfigs.FirstOrDefaultAsync()
                ?? new BorrowingConfig { DailyPenalty = 5.00m, GracePeriodDays = 0 };

            var overdueTransactions = await _context.BorrowTransactions
                .Where(b => b.ReturnDate == null
                         && b.Status == "Borrowed"
                         && b.DueDate < DateTime.Now)
                .ToListAsync();

            int generated = 0;

            foreach (var transaction in overdueTransactions)
            {
                bool exists = await _context.Penalties
                    .AnyAsync(p => p.BorrowTransactionId == transaction.Id);

                if (!exists)
                {
                    var graceCutoff = transaction.DueDate.AddDays(config.GracePeriodDays);
                    int daysOverdue = (int)(DateTime.Now - graceCutoff).TotalDays;

                    if (daysOverdue <= 0) continue;

                    decimal amount = daysOverdue * config.DailyPenalty;

                    if (config.MaxPenaltyCap.HasValue && amount > config.MaxPenaltyCap.Value)
                        amount = config.MaxPenaltyCap.Value;

                    _context.Penalties.Add(new Penalty
                    {
                        BorrowTransactionId = transaction.Id,
                        MemberId = transaction.MemberId,
                        Amount = amount,
                        DateIssued = DateTime.Now,
                        Status = "Unpaid"
                    });
                    generated++;
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"{generated} penalty record(s) generated.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Penalties/MarkPaid/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id)
        {
            var penalty = await _context.Penalties.FindAsync(id);
            if (penalty == null) return NotFound();

            penalty.Status = "Paid";
            penalty.DatePaid = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Penalty of ${penalty.Amount:F2} marked as paid.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Penalties/Create
        public IActionResult Create()
        {
            ViewData["BorrowTransactionId"] = new SelectList(_context.BorrowTransactions, "Id", "Id");
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName");
            return View();
        }

        // POST: Penalties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BorrowTransactionId,MemberId,Amount,DateIssued,DatePaid,Status")] Penalty penalty)
        {
            if (ModelState.IsValid)
            {
                _context.Add(penalty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BorrowTransactionId"] = new SelectList(_context.BorrowTransactions, "Id", "Id", penalty.BorrowTransactionId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName", penalty.MemberId);
            return View(penalty);
        }

        // GET: Penalties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var penalty = await _context.Penalties.FindAsync(id);
            if (penalty == null) return NotFound();
            ViewData["BorrowTransactionId"] = new SelectList(_context.BorrowTransactions, "Id", "Id", penalty.BorrowTransactionId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName", penalty.MemberId);
            return View(penalty);
        }

        // POST: Penalties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BorrowTransactionId,MemberId,Amount,DateIssued,DatePaid,Status")] Penalty penalty)
        {
            if (id != penalty.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(penalty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Penalties.Any(e => e.Id == penalty.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BorrowTransactionId"] = new SelectList(_context.BorrowTransactions, "Id", "Id", penalty.BorrowTransactionId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName", penalty.MemberId);
            return View(penalty);
        }

        // GET: Penalties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var penalty = await _context.Penalties
                .Include(p => p.Member)
                .Include(p => p.BorrowTransaction)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (penalty == null) return NotFound();
            return View(penalty);
        }

        // POST: Penalties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var penalty = await _context.Penalties.FindAsync(id);
            if (penalty != null) _context.Penalties.Remove(penalty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
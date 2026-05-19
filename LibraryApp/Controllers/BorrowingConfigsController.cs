using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;

namespace LibraryApp.Controllers
{
    public class BorrowingConfigsController : Controller
    {
        private readonly LibraryDbContext _context;

        public BorrowingConfigsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: BorrowingConfigs
        public async Task<IActionResult> Index()
        {
            return View(await _context.BorrowingConfigs.ToListAsync());
        }

        // GET: BorrowingConfigs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var config = await _context.BorrowingConfigs.FirstOrDefaultAsync(m => m.Id == id);
            if (config == null) return NotFound();
            return View(config);
        }

        // GET: BorrowingConfigs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BorrowingConfigs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LoanDurationDays,MaxRenewals,MaxItemsPerMember,DailyPenalty,MaxPenaltyCap,GracePeriodDays,AllowRenewals,AllowReservations")] BorrowingConfig config)
        {
            if (ModelState.IsValid)
            {
                _context.Add(config);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Borrowing config created.";
                return RedirectToAction("Edit", "LibraryProfiles");
            }
            return View(config);
        }

        // GET: BorrowingConfigs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var config = await _context.BorrowingConfigs.FindAsync(id);
            if (config == null) return NotFound();
            return View(config);
        }

        // POST: BorrowingConfigs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LoanDurationDays,MaxRenewals,MaxItemsPerMember,DailyPenalty,MaxPenaltyCap,GracePeriodDays,AllowRenewals,AllowReservations")] BorrowingConfig config)
        {
            if (id != config.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(config); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.BorrowingConfigs.Any(e => e.Id == config.Id)) return NotFound();
                    throw;
                }
                TempData["Success"] = "Borrowing configuration saved.";
                return RedirectToAction("Edit", "LibraryProfiles");
            }
            return View(config);
        }

        // GET: BorrowingConfigs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var config = await _context.BorrowingConfigs.FirstOrDefaultAsync(m => m.Id == id);
            if (config == null) return NotFound();
            return View(config);
        }

        // POST: BorrowingConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var config = await _context.BorrowingConfigs.FindAsync(id);
            if (config != null) _context.BorrowingConfigs.Remove(config);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

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
            var libraryDbContext = _context.Penalties.Include(p => p.BorrowTransaction).Include(p => p.Member);
            return View(await libraryDbContext.ToListAsync());
        }

        // GET: Penalties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penalty = await _context.Penalties
                .Include(p => p.BorrowTransaction)
                .Include(p => p.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (penalty == null)
            {
                return NotFound();
            }

            return View(penalty);
        }

        // GET: Penalties/Create
        public IActionResult Create()
        {
            ViewData["BorrowTransactionId"] = new SelectList(_context.BorrowTransactions, "Id", "Id");
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Email");
            return View();
        }

        // POST: Penalties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Email", penalty.MemberId);
            return View(penalty);
        }

        // GET: Penalties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penalty = await _context.Penalties.FindAsync(id);
            if (penalty == null)
            {
                return NotFound();
            }
            ViewData["BorrowTransactionId"] = new SelectList(_context.BorrowTransactions, "Id", "Id", penalty.BorrowTransactionId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Email", penalty.MemberId);
            return View(penalty);
        }

        // POST: Penalties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BorrowTransactionId,MemberId,Amount,DateIssued,DatePaid,Status")] Penalty penalty)
        {
            if (id != penalty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(penalty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PenaltyExists(penalty.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BorrowTransactionId"] = new SelectList(_context.BorrowTransactions, "Id", "Id", penalty.BorrowTransactionId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Email", penalty.MemberId);
            return View(penalty);
        }

        // GET: Penalties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penalty = await _context.Penalties
                .Include(p => p.BorrowTransaction)
                .Include(p => p.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (penalty == null)
            {
                return NotFound();
            }

            return View(penalty);
        }

        // POST: Penalties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var penalty = await _context.Penalties.FindAsync(id);
            if (penalty != null)
            {
                _context.Penalties.Remove(penalty);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PenaltyExists(int id)
        {
            return _context.Penalties.Any(e => e.Id == id);
        }
    }
}

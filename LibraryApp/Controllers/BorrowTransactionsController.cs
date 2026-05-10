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
            var libraryDbContext = _context.BorrowTransactions.Include(b => b.Admin).Include(b => b.Book).Include(b => b.Member);
            return View(await libraryDbContext.ToListAsync());
        }

        // GET: BorrowTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowTransaction = await _context.BorrowTransactions
                .Include(b => b.Admin)
                .Include(b => b.Book)
                .Include(b => b.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (borrowTransaction == null)
            {
                return NotFound();
            }

            return View(borrowTransaction);
        }

        // GET: BorrowTransactions/Create
        public IActionResult Create()
        {
            ViewData["AdminId"] = new SelectList(_context.Admins, "Id", "Email");
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Author");
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Email");
            return View();
        }

        // POST: BorrowTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,MemberId,AdminId,BorrowDate,DueDate,ReturnDate,RenewCount,Status")] BorrowTransaction borrowTransaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(borrowTransaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminId"] = new SelectList(_context.Admins, "Id", "Email", borrowTransaction.AdminId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Author", borrowTransaction.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Email", borrowTransaction.MemberId);
            return View(borrowTransaction);
        }

        // GET: BorrowTransactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowTransaction = await _context.BorrowTransactions.FindAsync(id);
            if (borrowTransaction == null)
            {
                return NotFound();
            }
            ViewData["AdminId"] = new SelectList(_context.Admins, "Id", "Email", borrowTransaction.AdminId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Author", borrowTransaction.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Email", borrowTransaction.MemberId);
            return View(borrowTransaction);
        }

        // POST: BorrowTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,MemberId,AdminId,BorrowDate,DueDate,ReturnDate,RenewCount,Status")] BorrowTransaction borrowTransaction)
        {
            if (id != borrowTransaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(borrowTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowTransactionExists(borrowTransaction.Id))
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
            ViewData["AdminId"] = new SelectList(_context.Admins, "Id", "Email", borrowTransaction.AdminId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Author", borrowTransaction.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Email", borrowTransaction.MemberId);
            return View(borrowTransaction);
        }

        // GET: BorrowTransactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowTransaction = await _context.BorrowTransactions
                .Include(b => b.Admin)
                .Include(b => b.Book)
                .Include(b => b.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (borrowTransaction == null)
            {
                return NotFound();
            }

            return View(borrowTransaction);
        }

        // POST: BorrowTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrowTransaction = await _context.BorrowTransactions.FindAsync(id);
            if (borrowTransaction != null)
            {
                _context.BorrowTransactions.Remove(borrowTransaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BorrowTransactionExists(int id)
        {
            return _context.BorrowTransactions.Any(e => e.Id == id);
        }
    }
}

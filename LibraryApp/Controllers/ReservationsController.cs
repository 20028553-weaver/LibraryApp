using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;

namespace LibraryApp.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly LibraryDbContext _context;

        public ReservationsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.Member)
                .OrderByDescending(r => r.ReservedDate)
                .ToListAsync();
            return View(reservations);
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var r = await _context.Reservations
                .Include(r => r.Book).Include(r => r.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (r == null) return NotFound();
            return View(r);
        }

        // POST: Reservations/Fulfill/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Fulfill(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();
            reservation.Status = "Fulfilled";
            await _context.SaveChangesAsync();
            TempData["Success"] = "Reservation fulfilled successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Reservations/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();
            reservation.Status = "Cancelled";
            await _context.SaveChangesAsync();
            TempData["Success"] = "Reservation cancelled.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title");
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName");
            return View();
        }

        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,MemberId,ReservedDate,ExpiryDate,Status")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Reservation created.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", reservation.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName", reservation.MemberId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", reservation.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName", reservation.MemberId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,MemberId,ReservedDate,ExpiryDate,Status")] Reservation reservation)
        {
            if (id != reservation.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(reservation); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Reservations.Any(e => e.Id == reservation.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", reservation.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName", reservation.MemberId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var r = await _context.Reservations
                .Include(r => r.Book).Include(r => r.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (r == null) return NotFound();
            return View(r);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null) _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

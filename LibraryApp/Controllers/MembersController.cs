using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;

namespace LibraryApp.Controllers
{
    public class MembersController : Controller
    {
        private readonly LibraryDbContext _context;

        public MembersController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            var members = await _context.Members
                .Include(m => m.BorrowTransactions)
                .OrderBy(m => m.FullName)
                .ToListAsync();
            return View(members);
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var member = await _context.Members
                .Include(m => m.BorrowTransactions).ThenInclude(bt => bt.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null) return NotFound();
            return View(member);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Email,Phone,Address,MembershipDate,MembershipExpiry,Status")] Member member)
        {
            if (ModelState.IsValid)
            {
                member.PasswordHash = "";
                _context.Add(member);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Member '{member.FullName}' added successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var member = await _context.Members.FindAsync(id);
            if (member == null) return NotFound();
            return View(member);
        }

        // POST: Members/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email,Phone,Address,MembershipDate,MembershipExpiry,Status")] Member formMember)
        {
            if (id != formMember.Id) return NotFound();
            if (ModelState.IsValid)
            {
                var existing = await _context.Members.FindAsync(id);
                if (existing == null) return NotFound();
                existing.FullName = formMember.FullName;
                existing.Email = formMember.Email;
                existing.Phone = formMember.Phone;
                existing.Address = formMember.Address;
                existing.MembershipDate = formMember.MembershipDate;
                existing.MembershipExpiry = formMember.MembershipExpiry;
                existing.Status = formMember.Status;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Member '{existing.FullName}' updated.";
                return RedirectToAction(nameof(Index));
            }
            return View(formMember);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id);
            if (member == null) return NotFound();
            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null) _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Member deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}

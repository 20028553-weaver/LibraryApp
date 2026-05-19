using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;

namespace LibraryApp.Controllers
{
    public class LibraryProfilesController : Controller
    {
        private readonly LibraryDbContext _context;

        public LibraryProfilesController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: LibraryProfiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.LibraryProfiles.ToListAsync());
        }

        // GET: LibraryProfiles/Edit — loads first or creates default, combined settings page
        public async Task<IActionResult> Edit(int? id = null)
        {
            LibraryProfile? profile;
            if (id == null)
            {
                profile = await _context.LibraryProfiles.FirstOrDefaultAsync();
                if (profile == null)
                {
                    profile = new LibraryProfile
                    {
                        Name = "Nepal Public Library",
                        Location = "Kathmandu, Nepal"
                    };
                    _context.LibraryProfiles.Add(profile);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                profile = await _context.LibraryProfiles.FindAsync(id);
                if (profile == null) return NotFound();
            }

            ViewBag.Config = await _context.BorrowingConfigs.FirstOrDefaultAsync();
            return View(profile);
        }

        // POST: LibraryProfiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location,Hours,ContactNumber,Email,Website,Description")] LibraryProfile formProfile)
        {
            if (id != formProfile.Id) return NotFound();
            if (ModelState.IsValid)
            {
                var existing = await _context.LibraryProfiles.FindAsync(id);
                if (existing == null) return NotFound();
                existing.Name = formProfile.Name;
                existing.Location = formProfile.Location;
                existing.Hours = formProfile.Hours;
                existing.ContactNumber = formProfile.ContactNumber;
                existing.Email = formProfile.Email;
                existing.Website = formProfile.Website;
                existing.Description = formProfile.Description;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Library profile saved.";
                return RedirectToAction(nameof(Edit));
            }
            ViewBag.Config = await _context.BorrowingConfigs.FirstOrDefaultAsync();
            return View(formProfile);
        }

        // GET: LibraryProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var profile = await _context.LibraryProfiles.FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null) return NotFound();
            return View(profile);
        }

        // GET: LibraryProfiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LibraryProfiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,Hours,ContactNumber,Email,Website,Description,LogoPath")] LibraryProfile profile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(profile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit));
            }
            return View(profile);
        }

        // GET: LibraryProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var profile = await _context.LibraryProfiles.FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null) return NotFound();
            return View(profile);
        }

        // POST: LibraryProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profile = await _context.LibraryProfiles.FindAsync(id);
            if (profile != null) _context.LibraryProfiles.Remove(profile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

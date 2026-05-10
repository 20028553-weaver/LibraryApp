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

        // GET: LibraryProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryProfile = await _context.LibraryProfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libraryProfile == null)
            {
                return NotFound();
            }

            return View(libraryProfile);
        }

        // GET: LibraryProfiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LibraryProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,Hours,ContactNumber,Email,Website,Description,LogoPath")] LibraryProfile libraryProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(libraryProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(libraryProfile);
        }

        // GET: LibraryProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryProfile = await _context.LibraryProfiles.FindAsync(id);
            if (libraryProfile == null)
            {
                return NotFound();
            }
            return View(libraryProfile);
        }

        // POST: LibraryProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location,Hours,ContactNumber,Email,Website,Description,LogoPath")] LibraryProfile libraryProfile)
        {
            if (id != libraryProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(libraryProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibraryProfileExists(libraryProfile.Id))
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
            return View(libraryProfile);
        }

        // GET: LibraryProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryProfile = await _context.LibraryProfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libraryProfile == null)
            {
                return NotFound();
            }

            return View(libraryProfile);
        }

        // POST: LibraryProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libraryProfile = await _context.LibraryProfiles.FindAsync(id);
            if (libraryProfile != null)
            {
                _context.LibraryProfiles.Remove(libraryProfile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibraryProfileExists(int id)
        {
            return _context.LibraryProfiles.Any(e => e.Id == id);
        }
    }
}

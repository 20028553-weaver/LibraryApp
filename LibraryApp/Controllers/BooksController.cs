using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;

namespace LibraryApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BooksController(LibraryDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Books
        public async Task<IActionResult> Index(string? search)
        {
            var books = _context.Books.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                books = books.Where(b =>
                    b.Title.Contains(search) ||
                    b.Author.Contains(search) ||
                    b.ISBN.Contains(search));

            ViewBag.Search = search;
            return View(await books.OrderByDescending(b => b.DateAdded).ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();
            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,Author,ISBN,TotalCopies,Description,Publisher,YearPublished")] Book book,
            IFormFile? coverImage)
        {
            if (ModelState.IsValid)
            {
                book.AvailableCopies = book.TotalCopies;
                book.IsAvailable = book.TotalCopies > 0;
                book.DateAdded = DateTime.Now;

                if (coverImage != null && coverImage.Length > 0)
                    book.CoverImagePath = await SaveCoverImage(coverImage);

                _context.Add(book);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"'{book.Title}' added successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Title,Author,ISBN,IsAvailable,TotalCopies,AvailableCopies,DateAdded,Description,Publisher,YearPublished,CoverImagePath")] Book book,
            IFormFile? coverImage)
        {
            if (id != book.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (coverImage != null && coverImage.Length > 0)
                    {
                        DeleteCoverImage(book.CoverImagePath);
                        book.CoverImagePath = await SaveCoverImage(coverImage);
                    }

                    _context.Update(book);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"'{book.Title}' updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // POST: Books/UploadCover/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCover(int id, IFormFile? coverImage)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            if (coverImage != null && coverImage.Length > 0)
            {
                DeleteCoverImage(book.CoverImagePath);
                book.CoverImagePath = await SaveCoverImage(coverImage);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Cover image updated for '{book.Title}'.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                DeleteCoverImage(book.CoverImagePath);
                _context.Books.Remove(book);
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Book deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id) => _context.Books.Any(e => e.Id == id);

        private async Task<string> SaveCoverImage(IFormFile file)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "images", "covers");
            Directory.CreateDirectory(uploadsDir);
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return $"/images/covers/{fileName}";
        }

        private void DeleteCoverImage(string? path)
        {
            if (string.IsNullOrEmpty(path)) return;
            var fullPath = Path.Combine(_env.WebRootPath, path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly LibraryDbContext _context;

        public AccountController(LibraryDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == email);

            if (admin != null && password == "admin123")
            {
                HttpContext.Session.SetString("UserEmail", email);
                HttpContext.Session.SetString("UserRole", "Admin");
                TempData["Success"] = "Welcome back Admin!";
                return RedirectToAction("Index", "Home");
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Email == email);

            if (member != null && password == "member123")
            {
                HttpContext.Session.SetString("UserEmail", email);
                HttpContext.Session.SetString("UserRole", "Member");
                HttpContext.Session.SetInt32("MemberId", member.Id);
                TempData["Success"] = $"Welcome {member.FullName}!";
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Invalid email or password.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Logged out successfully.";
            return RedirectToAction("Login");
        }
    }
}
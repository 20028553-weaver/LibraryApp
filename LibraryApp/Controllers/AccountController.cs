using Microsoft.AspNetCore.Mvc;
using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace LibraryApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly LibraryDbContext _context;

        public AccountController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            email = email?.Trim() ?? "";
            password = password ?? "";

            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == email);

            if (admin != null && admin.PasswordHash == HashPassword(password))
            {
                HttpContext.Session.SetString("UserEmail", email);
                HttpContext.Session.SetString("UserRole", "Admin");
                TempData["Success"] = $"Welcome back, {admin.FullName}!";
                return RedirectToAction("Index", "Home");
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Email == email);

            if (member != null && member.PasswordHash == HashPassword(password))
            {
                HttpContext.Session.SetString("UserEmail", email);
                HttpContext.Session.SetString("UserRole", "Member");
                HttpContext.Session.SetInt32("MemberId", member.Id);
                TempData["Success"] = $"Welcome, {member.FullName}!";
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Invalid email or password.";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var emailTaken = await _context.Members.AnyAsync(m => m.Email == model.Email)
                          || await _context.Admins.AnyAsync(a => a.Email == model.Email);

            if (emailTaken)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

            var member = new Member
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                MembershipDate = DateTime.Now,
                Status = "Active",
                PasswordHash = HashPassword(model.Password)
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Account created successfully! Please log in.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Logged out successfully.";
            return RedirectToAction("Login");
        }

        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}

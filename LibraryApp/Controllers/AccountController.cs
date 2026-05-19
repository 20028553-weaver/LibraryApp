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
        // GET: /Account/Register
        public IActionResult Register()
        {
            ViewBag.ShowRegister = true;
            return View("Login");
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string fullName, string email,
            string phone, string password)
        {
            // Check if email already exists
            bool emailExists = await _context.Members
                .AnyAsync(m => m.Email == email);

            if (emailExists)
            {
                TempData["Error"] = "An account with this email already exists.";
                ViewBag.ShowRegister = true;
                return View("Login");
            }

            // Create new member account
            var newMember = new Member
            {
                FullName = fullName,
                Email = email,
                Phone = phone,
                Address = "",
                MembershipDate = DateTime.Now,
                MembershipExpiry = DateTime.Now.AddYears(1),
                Status = "Active"
            };

            _context.Members.Add(newMember);
            await _context.SaveChangesAsync();

            // Auto login after registration
            HttpContext.Session.SetString("UserEmail", email);
            HttpContext.Session.SetString("UserRole", "Member");
            HttpContext.Session.SetInt32("MemberId", newMember.Id);

            TempData["Success"] = $"Welcome {fullName}! Your account has been created.";
            return RedirectToAction("Index", "Home");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LibraryApp.Controllers
{
    [AllowAnonymous]
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
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            email = email?.Trim() ?? "";
            password = password ?? "";

            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);
            if (admin != null && admin.PasswordHash == HashPassword(password))
            {
                await SignIn(email, "Admin", admin.FullName, null);
                TempData["Success"] = $"Welcome back, {admin.FullName}!";
                return RedirectToAction("Index", "Home");
            }

            var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == email);
            if (member != null && member.PasswordHash == HashPassword(password))
            {
                await SignIn(email, "Member", member.FullName, member.Id);
                TempData["Success"] = $"Welcome, {member.FullName}!";
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Invalid email or password.";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = model.Email?.Trim() ?? "";

            var emailTaken = await _context.Members.AnyAsync(m => m.Email == email)
                          || await _context.Admins.AnyAsync(a => a.Email == email);

            if (emailTaken)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

            var member = new Member
            {
                FullName = model.FullName,
                Email = email,
                Phone = model.Phone,
                MembershipDate = DateTime.Now,
                MembershipExpiry = DateTime.Now.AddYears(1),
                Status = "Active",
                PasswordHash = HashPassword(model.Password)
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            await SignIn(email, "Member", member.FullName, member.Id);
            TempData["Success"] = $"Welcome, {member.FullName}! Your account has been created.";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Logged out successfully.";
            return RedirectToAction("Login");
        }

        private async Task SignIn(string email, string role, string fullName, int? memberId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role),
                new Claim("FullName", fullName)
            };
            if (memberId.HasValue)
                claims.Add(new Claim("MemberId", memberId.Value.ToString()));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}

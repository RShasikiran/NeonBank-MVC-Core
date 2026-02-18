using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeonBank.Data;
using NeonBank.Models;

namespace NeonBank.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Account");

            var userId = int.Parse(userIdStr);
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            // ✅ FIX: Account could be null if something went wrong during registration
            if (account == null)
            {
                ViewBag.Error = "No account found linked to your profile. Please contact support.";
            }

            // Fetch announcements for the dashboard news section
            ViewBag.News = await _context.Announcements
                .OrderByDescending(a => a.PostedDate)
                .Take(3)
                .ToListAsync();

            return View(account);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}

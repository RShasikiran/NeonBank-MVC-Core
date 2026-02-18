using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NeonBank.Data;
using NeonBank.Models;
using System.Security.Claims;

namespace NeonBank.Controllers
{
    [Authorize]
    public class BankingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BankingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ─── HELPER ────────────────────────────────────────────────────────
        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // ─── 1. USER DASHBOARD ─────────────────────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            var userId = GetUserId();
            var account = await _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
            {
                TempData["Error"] = "No account found. Please contact support.";
                return RedirectToAction("Index", "Home");
            }

            return View(account);
        }

        // ─── 2. FUND TRANSFER ──────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(string receiverAccNo, decimal amount)
        {
            var senderId = GetUserId();
            var senderAcc = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == senderId);

            if (senderAcc == null)
            {
                TempData["Error"] = "Sender account not found.";
                return RedirectToAction("Dashboard");
            }

            // ✅ FIX: Validate amount
            if (amount <= 0)
            {
                TempData["Error"] = "Transfer amount must be greater than zero.";
                return RedirectToAction("Dashboard");
            }

            // ✅ FIX: Prevent self-transfer
            if (senderAcc.AccountNumber == receiverAccNo)
            {
                TempData["Error"] = "You cannot transfer to your own account.";
                return RedirectToAction("Dashboard");
            }

            var receiverAcc = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == receiverAccNo);

            if (receiverAcc == null)
            {
                TempData["Error"] = "Recipient account number not found. Please check and try again.";
                return RedirectToAction("Dashboard");
            }

            // ✅ FIX: Check sufficient funds
            if (senderAcc.Balance < amount)
            {
                TempData["Error"] = $"Insufficient funds. Your balance is ${senderAcc.Balance:N2}.";
                return RedirectToAction("Dashboard");
            }

            // Perform transfer
            senderAcc.Balance -= amount;
            receiverAcc.Balance += amount;

            // ✅ FIX: Create debit transaction for sender
            _context.Transactions.Add(new Transaction
            {
                AccountId = senderAcc.AccountId,
                Amount = -amount,
                Type = "Transfer",
                Description = $"Transfer to {receiverAccNo}",
                TransactionDate = DateTime.Now
            });

            // ✅ FIX: Create credit transaction for receiver (was missing!)
            _context.Transactions.Add(new Transaction
            {
                AccountId = receiverAcc.AccountId,
                Amount = amount,
                Type = "Transfer",
                Description = $"Transfer from {senderAcc.AccountNumber}",
                TransactionDate = DateTime.Now
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Successfully transferred ${amount:N2} to account {receiverAccNo}.";
            return RedirectToAction("Dashboard");
        }

        // ─── 3. ADMIN DASHBOARD ────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            // ✅ FIX: Use left join so admins with no account don't crash the page
            var allClients = await _context.Users
                .Include(u => u.Account)
                .Where(u => u.Role != "Admin")
                .ToListAsync();

            ViewBag.TotalDeposits = allClients
                .Where(c => c.Account != null)
                .Sum(c => c.Account!.Balance);

            ViewBag.TotalClients = allClients.Count;

            return View(allClients);
        }

        // ─── 4. LOAN REQUEST PAGE ──────────────────────────────────────────
        public IActionResult RequestLoan() => View();

        // ─── 5. SUBMIT LOAN ────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitLoan(decimal amount)
        {
            if (amount <= 0)
            {
                TempData["Error"] = "Loan amount must be greater than zero.";
                return RedirectToAction("RequestLoan");
            }

            var userId = GetUserId();
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
            {
                TempData["Error"] = "Account not found.";
                return RedirectToAction("RequestLoan");
            }

            _context.Transactions.Add(new Transaction
            {
                AccountId = account.AccountId,
                Amount = amount,
                Type = "Loan Pending",
                Description = $"Loan request of ${amount:N2} — Awaiting Admin Approval",
                TransactionDate = DateTime.Now
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Your loan request of ${amount:N2} has been submitted and is under review.";
            return RedirectToAction("Dashboard");
        }

        // ─── 6. ADMIN APPROVE LOAN ─────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveLoan(int accountId, decimal approvedAmount)
        {
            if (approvedAmount <= 0)
            {
                TempData["Error"] = "Approved amount must be greater than zero.";
                return RedirectToAction("AdminDashboard");
            }

            var account = await _context.Accounts.FindAsync(accountId);

            if (account != null)
            {
                account.Balance += approvedAmount;

                _context.Transactions.Add(new Transaction
                {
                    AccountId = accountId,
                    Amount = approvedAmount,
                    Type = "Loan Approved",
                    Description = $"Institutional Loan Credit of ${approvedAmount:N2}",
                    TransactionDate = DateTime.Now
                });

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Successfully credited ${approvedAmount:N2} to account {account.AccountNumber}.";
            }
            else
            {
                TempData["Error"] = "Account not found.";
            }

            return RedirectToAction("AdminDashboard");
        }

        // ─── 7. TRANSACTION HISTORY ────────────────────────────────────────
        public async Task<IActionResult> TransactionHistory()
        {
            var userId = GetUserId();
            var account = await _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
                return RedirectToAction("Index", "Home");

            var transactions = account.Transactions
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            return View(transactions);
        }

        // ─── 8. PROFILE ────────────────────────────────────────────────────
        public async Task<IActionResult> Profile()
        {
            var userId = GetUserId();
            var user = await _context.Users
                .Include(u => u.Account)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return RedirectToAction("Login", "Account");

            return View(user);
        }

        // ─── 9. CONTACT SUPPORT ────────────────────────────────────────────
        public IActionResult ContactSupport() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Subject and message are required.";
                return RedirectToAction("ContactSupport");
            }

            var msg = new SupportMessage
            {
                SenderName = User.Identity?.Name ?? "Unknown",
                Subject = subject,
                MessageContent = message,
                SentDate = DateTime.Now
            };

            _context.SupportMessages.Add(msg);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Your message has been sent to the support team.";
            return RedirectToAction("Index", "Home");
        }

        // ─── 10. ADMIN SUPPORT INBOX ───────────────────────────────────────
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SupportInbox()
        {
            var messages = await _context.SupportMessages
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();
            return View(messages);
        }

        // ─── 11. ADMIN POST NEWS ───────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostNews(string title, string content, string priority)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Title and content are required.";
                return RedirectToAction("AdminDashboard");
            }

            var news = new Announcement
            {
                Title = title,
                Content = content,
                Priority = priority ?? "Normal",
                PostedDate = DateTime.Now
            };

            _context.Announcements.Add(news);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Announcement broadcast to all clients.";
            return RedirectToAction("AdminDashboard");
        }
    }
}

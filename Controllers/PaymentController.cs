using Microsoft.AspNetCore.Mvc;
using Radiocab.Models;
using Radiocab.Models.SqlDb;

namespace Radiocab.Controllers
{
    public class PaymentController : Controller
    {
        private readonly SqlDbContext _context;

        public PaymentController(SqlDbContext context)
        {
            _context = context;
        }

        private bool IsAuthenticated()
        {
            return HttpContext.Session.GetString("UserEmail") != null;
        }

        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        // Payment page for making payments
        public IActionResult MakePayment(string entityType, int entityId)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "MyController1");

            var model = new Payment
            {
                EntityType = entityType,
                EntityId = entityId,
                PaymentDate = DateTime.UtcNow
            };

            // Get entity details to determine amount
            decimal amount = 0;
            string paymentType = PaymentCatalog.Monthly;

            switch (entityType.ToLower())
            {
                case "listing":
                    var listing = _context.Listings.FirstOrDefault(l => l.ListingId == entityId);
                    if (listing != null)
                    {
                        amount = listing.PaymentAmount;
                        paymentType = listing.PaymentType;
                        model.DueDate = listing.CreatedAt.AddMonths(1);
                    }
                    break;
                case "driver":
                    var driver = _context.Drivers
                        .FirstOrDefault(d => d.DriverId == entityId);

                    if (driver != null)
                    {
                        Console.WriteLine($"Driver Found");
                        Console.WriteLine($"DriverId = {driver.DriverId}");
                        Console.WriteLine($"PaymentAmount = {driver.PaymentAmount}");

                        amount = driver.PaymentAmount;
                        paymentType = driver.PaymentType;
                        model.DueDate = driver.CreatedAt.AddMonths(1);
                    }
                    else
                    {
                        Console.WriteLine("Driver NOT FOUND");
                    }
                    break;
                case "advertisement":
                    var ad = _context.Advertisements.FirstOrDefault(a => a.AdvertiseId == entityId);
                    if (ad != null)
                    {
                        amount = ad.PaymentAmount;
                        paymentType = ad.PaymentType;
                        model.DueDate = ad.CreatedAt.AddMonths(1);
                    }
                    break;
            }

            model.Amount = amount;
            model.PaymentType = paymentType;
            model.TransactionId = GenerateTransactionId();

            ViewBag.EntityType = entityType;
            ViewBag.EntityId = entityId;
            ViewBag.Amount = amount;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessPayment(Payment model)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "MyController1");

            if (!ModelState.IsValid)
            {
                ViewBag.EntityType = model.EntityType;
                ViewBag.EntityId = model.EntityId;
                ViewBag.Amount = model.Amount;
                return View("MakePayment", model);
            }

            // Generate receipt number
            model.ReceiptNumber = "RC-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + new Random().Next(1000, 9999);
            model.Status = "Paid";
            model.PaymentDate = DateTime.UtcNow;

            _context.Payments.Add(model);
            _context.SaveChanges();

            // Update entity payment status
            switch (model.EntityType.ToLower())
            {
                case "listing":
                    var listing = _context.Listings.FirstOrDefault(l => l.ListingId == model.EntityId);
                    if (listing != null) listing.PaymentStatus = "Paid";
                    break;
                case "driver":
                    var driver = _context.Drivers.FirstOrDefault(d => d.DriverId == model.EntityId);
                    if (driver != null) driver.PaymentStatus = "Paid";
                    break;
                case "advertisement":
                    var ad = _context.Advertisements.FirstOrDefault(a => a.AdvertiseId == model.EntityId);
                    if (ad != null) ad.PaymentStatus = "Paid";
                    break;
            }

            _context.SaveChanges();

            // Log activity
            LogActivity("Payment", model.EntityType, model.EntityId, $"Payment of {model.Amount:C} processed via {model.PaymentMethod}");

            TempData["Success"] = "Payment processed successfully!";
            return RedirectToAction("Receipt", new { id = model.PaymentId });
        }

        // Payment receipt
        public IActionResult Receipt(int id)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "MyController1");

            var payment = _context.Payments.FirstOrDefault(p => p.PaymentId == id);
            if (payment == null) return NotFound();

            return View(payment);
        }

        // Payment history for current user
        public IActionResult PaymentHistory()
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "MyController1");

            var userEmail = HttpContext.Session.GetString("UserEmail");
            var payments = _context.Payments
                .Where(p => 
                    (p.EntityType == "Listing" && _context.Listings.Any(l => l.ListingId == p.EntityId && l.Email == userEmail)) ||
                    (p.EntityType == "Driver" && _context.Drivers.Any(d => d.DriverId == p.EntityId && d.Email == userEmail)) ||
                    (p.EntityType == "Advertisement" && _context.Advertisements.Any(a => a.AdvertiseId == p.EntityId && a.Email == userEmail)))
                .OrderByDescending(p => p.PaymentDate)
                .ToList();

            return View(payments);
        }

        // Admin: All payments
        public IActionResult AdminPayments(string status = "All", string search = "", int page = 1, int pageSize = 10)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");

            var query = _context.Payments.AsQueryable();

            if (!string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(p => p.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.TransactionId.Contains(search) ||
                    p.ReceiptNumber.Contains(search));
            }

            var totalCount = query.Count();
            var items = query.OrderByDescending(p => p.PaymentDate)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

            ViewBag.Status = status;
            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(items);
        }

        // Admin: Payment details
        public IActionResult PaymentDetails(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");

            var payment = _context.Payments.FirstOrDefault(p => p.PaymentId == id);
            if (payment == null) return NotFound();

            return View(payment);
        }

        // Admin: Delete payment
        public IActionResult DeletePayment(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");

            var payment = _context.Payments.FirstOrDefault(p => p.PaymentId == id);
            if (payment == null) return NotFound();

            return View(payment);
        }

        [HttpPost, ActionName("DeletePayment")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePaymentConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");

            var payment = _context.Payments.FirstOrDefault(p => p.PaymentId == id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                _context.SaveChanges();
                TempData["Success"] = "Payment deleted successfully.";
            }

            return RedirectToAction(nameof(AdminPayments));
        }

        // Helper: Generate transaction ID
        private string GenerateTransactionId()
        {
            return "TXN" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999);
        }

        // Helper: Log activity
        private void LogActivity(string action, string? entityType, int? entityId, string description)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userName = HttpContext.Session.GetString("UserName");

            var log = new ActivityLog
            {
                UserId = userEmail ?? "Unknown",
                UserName = userName,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Description = description,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            _context.ActivityLogs.Add(log);
            _context.SaveChanges();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Radiocab.Models;
using Radiocab.Models.SqlDb;
using Radiocab.Filters;

namespace Radiocab.Controllers
{
    [SessionAuthorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly SqlDbContext _context;

        public AdminController(SqlDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        public IActionResult Index()
        {
            ViewBag.TotalDrivers = _context.Drivers.Count();
            ViewBag.TotalCompanies = _context.Listings.Count();
            ViewBag.TotalAdvertisements = _context.Advertisements.Count();
            ViewBag.TotalFeedbacks = _context.Feedbacks.Count();
            
            ViewBag.PendingCompanies = _context.Listings.Count(x => x.Status == "Pending");
            ViewBag.ApprovedCompanies = _context.Listings.Count(x => x.Status == "Approved");
            ViewBag.RejectedCompanies = _context.Listings.Count(x => x.Status == "Rejected");
            
            ViewBag.PendingDrivers = _context.Drivers.Count(x => x.Status == "Pending");
            ViewBag.ApprovedDrivers = _context.Drivers.Count(x => x.Status == "Approved");
            ViewBag.RejectedDrivers = _context.Drivers.Count(x => x.Status == "Rejected");
            
            ViewBag.TotalPendingPayments =
                _context.Listings.Count(x => x.PaymentStatus == "Pending") +
                _context.Drivers.Count(x => x.PaymentStatus == "Pending") +
                _context.Advertisements.Count(x => x.PaymentStatus == "Pending");

            ViewBag.TotalPaidUsers = 
                _context.Listings.Count(x => x.PaymentStatus == "Paid") + 
                _context.Drivers.Count(x => x.PaymentStatus == "Paid") + 
                _context.Advertisements.Count(x => x.PaymentStatus == "Paid");

            // Plan distributions
            ViewBag.FreeCompanies = _context.Listings.Count(x => x.MembershipType == "Free");
            ViewBag.BasicCompanies = _context.Listings.Count(x => x.MembershipType == "Basic");
            ViewBag.PremiumCompanies = _context.Listings.Count(x => x.MembershipType == "Premium");

            // Revenue calculation
            decimal totalRevenue = 
                _context.Listings.Where(x => x.PaymentStatus == "Paid").Sum(x => x.PaymentAmount) +
                _context.Drivers.Where(x => x.PaymentStatus == "Paid").Sum(x => x.PaymentAmount) +
                _context.Advertisements.Where(x => x.PaymentStatus == "Paid").Sum(x => x.PaymentAmount);
            ViewBag.TotalRevenue = totalRevenue;

            // Recent activity feed
            ViewBag.RecentLogs = _context.ActivityLogs.OrderByDescending(x => x.CreatedAt).Take(5).ToList();

            return View();
        }

        public IActionResult AddDriver() => RedirectToAction("Drivers", "MyController1");
        public IActionResult AddCompany() => RedirectToAction("Listing", "MyController1");

        public IActionResult Driverlist() => RedirectToAction(nameof(Drivers));
        public IActionResult Companylist() => RedirectToAction(nameof(Listings));
        public IActionResult Advertise() => RedirectToAction(nameof(Advertisements));

        // LISTINGS CRUD
        public IActionResult Listings(string status = "All", string search = "", int page = 1, int pageSize = 10)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var query = _context.Listings.AsQueryable();
            if (!string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    x.City.Contains(search));
            }

            var totalCount = query.Count();
            var items = query.OrderByDescending(l => l.ListingId)
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

        public IActionResult ListingDetails(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Listings.FirstOrDefault(x => x.ListingId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult EditListing(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Listings.FirstOrDefault(x => x.ListingId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditListing(Listing model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            if (!ModelState.IsValid) return View(model);

            _context.Listings.Update(model);
            _context.SaveChanges();
            TempData["Success"] = "Listing updated successfully.";
            return RedirectToAction(nameof(Listings));
        }

        public IActionResult DeleteListing(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Listings.FirstOrDefault(x => x.ListingId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("DeleteListing")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteListingConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Listings.FirstOrDefault(x => x.ListingId == id);
            if (item != null)
            {
                _context.Listings.Remove(item);
                _context.SaveChanges();
                TempData["Success"] = "Listing deleted.";
            }
            return RedirectToAction(nameof(Listings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveListing(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Listings.FirstOrDefault(x => x.ListingId == id);
            if (item != null)
            {
                item.Status = "Approved";
                _context.SaveChanges();
                TempData["Success"] = "Company Approved Successfully";
            }
            return RedirectToAction(nameof(Listings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectListing(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Listings.FirstOrDefault(x => x.ListingId == id);
            if (item != null)
            {
                item.Status = "Rejected";
                _context.SaveChanges();
                TempData["Success"] = "Company Rejected Successfully";
            }
            return RedirectToAction(nameof(Listings));
        }

        // DRIVERS CRUD
        public IActionResult Drivers(string status = "All", string search = "", int page = 1, int pageSize = 10)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var query = _context.Drivers.AsQueryable();
            if (!string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    x.City.Contains(search));
            }

            var totalCount = query.Count();
            var items = query.OrderByDescending(d => d.DriverId)
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

        public IActionResult DriverDetails(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Drivers.FirstOrDefault(x => x.DriverId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult EditDriver(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Drivers.FirstOrDefault(x => x.DriverId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDriver(Driver model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            if (!ModelState.IsValid) return View(model);

            _context.Drivers.Update(model);
            _context.SaveChanges();
            TempData["Success"] = "Driver updated successfully.";
            return RedirectToAction(nameof(Drivers));
        }

        public IActionResult DeleteDriver(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Drivers.FirstOrDefault(x => x.DriverId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("DeleteDriver")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDriverConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Drivers.FirstOrDefault(x => x.DriverId == id);
            if (item != null)
            {
                _context.Drivers.Remove(item);
                _context.SaveChanges();
                TempData["Success"] = "Driver deleted.";
            }
            return RedirectToAction(nameof(Drivers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveDriver(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Drivers.FirstOrDefault(x => x.DriverId == id);
            if (item != null)
            {
                item.Status = "Approved";
                _context.SaveChanges();
                TempData["Success"] = "Driver Approved Successfully";
            }
            return RedirectToAction(nameof(Drivers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectDriver(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Drivers.FirstOrDefault(x => x.DriverId == id);
            if (item != null)
            {
                item.Status = "Rejected";
                _context.SaveChanges();
                TempData["Success"] = "Driver Rejected Successfully";
            }
            return RedirectToAction(nameof(Drivers));
        }

        // ADVERTISEMENTS CRUD
        public IActionResult Advertisements(string status = "All", string search = "", int page = 1, int pageSize = 10)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var query = _context.Advertisements.AsQueryable();
            if (!string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    x.City.Contains(search));
            }

            var totalCount = query.Count();
            var items = query.OrderByDescending(a => a.AdvertiseId)
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

        public IActionResult AdvertisementDetails(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Advertisements.FirstOrDefault(x => x.AdvertiseId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult EditAdvertisement(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Advertisements.FirstOrDefault(x => x.AdvertiseId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAdvertisement(Advertise model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            if (!ModelState.IsValid) return View(model);

            _context.Advertisements.Update(model);
            _context.SaveChanges();
            TempData["Success"] = "Advertisement updated successfully.";
            return RedirectToAction(nameof(Advertisements));
        }

        public IActionResult DeleteAdvertisement(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Advertisements.FirstOrDefault(x => x.AdvertiseId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("DeleteAdvertisement")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAdvertisementConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Advertisements.FirstOrDefault(x => x.AdvertiseId == id);
            if (item != null)
            {
                _context.Advertisements.Remove(item);
                _context.SaveChanges();
                TempData["Success"] = "Advertisement deleted.";
            }
            return RedirectToAction(nameof(Advertisements));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveAdvertisement(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Advertisements.FirstOrDefault(x => x.AdvertiseId == id);
            if (item != null)
            {
                item.Status = "Approved";
                _context.SaveChanges();
                TempData["Success"] = "Advertisement Approved Successfully";
            }
            return RedirectToAction(nameof(Advertisements));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectAdvertisement(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Advertisements.FirstOrDefault(x => x.AdvertiseId == id);
            if (item != null)
            {
                item.Status = "Rejected";
                _context.SaveChanges();
                TempData["Success"] = "Advertisement Rejected Successfully";
            }
            return RedirectToAction(nameof(Advertisements));
        }

        // FEEDBACK CRUD
        public IActionResult Feedbacks(string status = "All", string search = "", int page = 1, int pageSize = 10)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var query = _context.Feedbacks.AsQueryable();
            if (!string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    x.Email.Contains(search) ||
                    x.City.Contains(search));
            }

            var totalCount = query.Count();
            var items = query.OrderByDescending(f => f.FeedbackId)
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

        public IActionResult FeedbackDetails(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Feedbacks.FirstOrDefault(x => x.FeedbackId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult EditFeedback(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Feedbacks.FirstOrDefault(x => x.FeedbackId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditFeedback(Feedback model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            if (!ModelState.IsValid) return View(model);

            _context.Feedbacks.Update(model);
            _context.SaveChanges();
            TempData["Success"] = "Feedback updated successfully.";
            return RedirectToAction(nameof(Feedbacks));
        }

        public IActionResult DeleteFeedback(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Feedbacks.FirstOrDefault(x => x.FeedbackId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("DeleteFeedback")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFeedbackConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Feedbacks.FirstOrDefault(x => x.FeedbackId == id);
            if (item != null)
            {
                _context.Feedbacks.Remove(item);
                _context.SaveChanges();
                TempData["Success"] = "Feedback deleted.";
            }
            return RedirectToAction(nameof(Feedbacks));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveFeedback(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Feedbacks.FirstOrDefault(x => x.FeedbackId == id);
            if (item != null)
            {
                item.Status = "Approved";
                _context.SaveChanges();
                TempData["Success"] = "Feedback Approved Successfully";
            }
            return RedirectToAction(nameof(Feedbacks));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HideFeedback(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "MyController1");
            var item = _context.Feedbacks.FirstOrDefault(x => x.FeedbackId == id);
            if (item != null)
            {
                item.Status = "Hidden";
                _context.SaveChanges();
                TempData["Success"] = "Feedback Hidden Successfully";
            }
            return RedirectToAction(nameof(Feedbacks));
        }
    }
}

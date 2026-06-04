using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Radiocab.Models;
using Radiocab.Models.SqlDb;

namespace Radiocab.Controllers
{

    public class MyController1 : Controller
    {
        private readonly SqlDbContext _context;

        public MyController1(SqlDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.FeaturedCompanies = _context.Listings.Where(l => l.Status == "Approved").OrderByDescending(l => l.MembershipType == "Premium").ThenByDescending(l => l.ListingId).Take(6).ToList();
            ViewBag.FeaturedDrivers = _context.Drivers.Where(d => d.Status == "Approved").OrderByDescending(d => d.Experience).Take(6).ToList();
            ViewBag.Advertisements = _context.Advertisements.Where(a => a.Status == "Approved" || a.PaymentStatus == "Paid").OrderByDescending(a => a.AdvertiseId).Take(3).ToList();
            ViewBag.Feedbacks = _context.Feedbacks.Where(f => f.Status == "Approved").OrderByDescending(f => f.FeedbackId).Take(5).ToList();
            return View();
        }

        public IActionResult Drivers()
        {
            var viewModel = new DriverViewModel
            {
                NewDriver = new Driver(),
                AllDrivers = _context.Drivers.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Drivers(DriverViewModel model, IFormFile? PhotoFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill all required fields correctly!";
            }
            else
            {
                var exists = _context.Drivers.Any(d => d.DriverCode == model.NewDriver.DriverCode);
                var emailExists = !string.IsNullOrEmpty(model.NewDriver.Email) && _context.Users.Any(u => u.Email == model.NewDriver.Email);
                
                if (exists)
                {
                    TempData["Error"] = "Driver ID already exists. Please use a unique Driver ID.";
                }
                else if (emailExists)
                {
                    TempData["Error"] = "Email address already registered. Please use a unique Email.";
                }
                else
                {
                    // Handle photo upload
                    if (PhotoFile != null && PhotoFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/drivers");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(PhotoFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using var fileStream = new FileStream(filePath, FileMode.Create);
                        PhotoFile.CopyTo(fileStream);

                        model.NewDriver.PhotoPath = "/images/drivers/" + uniqueFileName;
                    }

                    // Hash password
                    model.NewDriver.Password = PasswordHelper.HashPassword(model.NewDriver.Password);
                    model.NewDriver.PaymentAmount = PaymentCatalog.GetAmount("Driver", model.NewDriver.PaymentType);
                    model.NewDriver.PaymentStatus = "Pending";
                    model.NewDriver.Status = "Pending";

                    _context.Drivers.Add(model.NewDriver);

                    // Add to central Users table
                    var driverUser = new Users
                    {
                        Name = model.NewDriver.Name,
                        Email = string.IsNullOrEmpty(model.NewDriver.Email) ? $"{model.NewDriver.DriverCode}@radiocabs.in" : model.NewDriver.Email,
                        Password = model.NewDriver.Password,
                        Role = "RegisteredUnit",
                        UnitType = "Driver"
                    };
                    _context.Users.Add(driverUser);

                    _context.SaveChanges();
                    TempData["Success"] = "Driver registered successfully.";
                }
            }

            var viewModel = new DriverViewModel
            {
                NewDriver = new Driver(),
                AllDrivers = _context.Drivers.ToList()
            };
            return View(viewModel);
        }

        public IActionResult Listing()
        {
            var viewModel = new ListingViewModel
            {
                NewListing = new Listing(),
                AllListings = _context.Listings.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddListing(ListingViewModel model, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill required fields correctly!";
            }
            else
            {
                var exists = _context.Listings.Any(l => l.CompanyCode == model.NewListing.CompanyCode);
                var emailExists = !string.IsNullOrEmpty(model.NewListing.Email) && _context.Users.Any(u => u.Email == model.NewListing.Email);

                if (exists)
                {
                    TempData["Error"] = "Company ID already exists. Please use a unique Company ID.";
                }
                else if (emailExists)
                {
                    TempData["Error"] = "Email address already registered. Please use a unique Email.";
                }
                else
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/listings");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using var fileStream = new FileStream(filePath, FileMode.Create);
                        ImageFile.CopyTo(fileStream);

                        model.NewListing.ImagePath = "/images/listings/" + uniqueFileName;
                    }

                    // Hash password
                    model.NewListing.Password = PasswordHelper.HashPassword(model.NewListing.Password);
                    model.NewListing.PaymentAmount = PaymentCatalog.GetAmount("Registration", model.NewListing.PaymentType, model.NewListing.MembershipType);
                    model.NewListing.PaymentStatus = "Pending";
                    model.NewListing.Status = "Pending";

                    _context.Listings.Add(model.NewListing);

                    // Add to central Users table
                    var listingUser = new Users
                    {
                        Name = model.NewListing.Name,
                        Email = string.IsNullOrEmpty(model.NewListing.Email) ? $"{model.NewListing.CompanyCode}@radiocabs.in" : model.NewListing.Email,
                        Password = model.NewListing.Password,
                        Role = "RegisteredUnit",
                        UnitType = "Listing"
                    };
                    _context.Users.Add(listingUser);

                    _context.SaveChanges();
                    TempData["Success"] = "Listing registered successfully.";
                }
            }

            var viewModel = new ListingViewModel
            {
                NewListing = new Listing(),
                AllListings = _context.Listings.ToList()
            };
            return View("Listing", viewModel);
        }

        public IActionResult Advertise() => View(new Advertise());

        [HttpPost]
        public IActionResult AddAdvertise(Advertise advertise, IFormFile? BannerFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill all required fields correctly!";
            }
            else
            {
                // Handle banner upload
                if (BannerFile != null && BannerFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/advertisements");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(BannerFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    BannerFile.CopyTo(fileStream);

                    advertise.BannerPath = "/images/advertisements/" + uniqueFileName;
                }

                advertise.PaymentAmount = PaymentCatalog.GetAmount("Advertisement", advertise.PaymentType);
                advertise.PaymentStatus = "Pending";

                _context.Advertisements.Add(advertise);
                _context.SaveChanges();
                TempData["Success"] = "Advertisement submitted successfully.";
            }

            return View("Advertise", new Advertise());
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string Email, string Password, string LoginType)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                return Content("Invalid");
            }

            Users? user = null;

            if (LoginType == "Admin")
            {
                user = _context.Users.FirstOrDefault(u => (u.Email == Email || u.Name == Email) && u.Role == "Admin");
            }
            else
            {
                // Try searching in the central Users table first
                user = _context.Users.FirstOrDefault(u => u.Email == Email);

                if (user == null)
                {
                    // Search in Listings by Company ID
                    var listing = _context.Listings.FirstOrDefault(l => l.CompanyCode == Email);
                    if (listing != null && PasswordHelper.VerifyPassword(Password, listing.Password))
                    {
                        HttpContext.Session.SetString("UserEmail", listing.Email ?? $"{listing.CompanyCode}@radiocabs.in");
                        HttpContext.Session.SetString("UserRole", "RegisteredUnit");
                        HttpContext.Session.SetString("UserName", listing.Name);
                        HttpContext.Session.SetString("UnitType", "Listing");
                        return Content("UnitSuccess");
                    }

                    // Search in Drivers by Driver ID
                    var driver = _context.Drivers.FirstOrDefault(d => d.DriverCode == Email);
                    if (driver != null && PasswordHelper.VerifyPassword(Password, driver.Password))
                    {
                        HttpContext.Session.SetString("UserEmail", driver.Email ?? $"{driver.DriverCode}@radiocabs.in");
                        HttpContext.Session.SetString("UserRole", "RegisteredUnit");
                        HttpContext.Session.SetString("UserName", driver.Name);
                        HttpContext.Session.SetString("UnitType", "Driver");
                        return Content("UnitSuccess");
                    }
                }
            }

            if (user == null || !PasswordHelper.VerifyPassword(Password, user.Password))
            {
                return Content("Invalid");
            }

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UnitType", user.UnitType ?? "Listing");

            if (user.Role == "Admin")
            {
                return Content("AdminSuccess");
            }

            return Content("UnitSuccess");
        }

        [HttpPost]
        public IActionResult Register(string Name, string Email, string Password, string UnitType)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == Email);
            if (existingUser != null)
            {
                return Content("This email is already registered");
            }

            var newUser = new Users
            {
                Name = Name,
                Email = Email,
                Password = PasswordHelper.HashPassword(Password),
                Role = "RegisteredUnit",
                UnitType = string.IsNullOrWhiteSpace(UnitType) ? "Listing" : UnitType
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return Content("Registration successful");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFeedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                _context.Feedbacks.Add(feedback);
                _context.SaveChanges();
                TempData["Success"] = "Feedback submitted!";
            }
            else
            {
                TempData["Error"] = "Please fill all required fields correctly!";
            }

            return View("Feedback", new Feedback());
        }

        public IActionResult UnitDashboard()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrWhiteSpace(email) || role != "RegisteredUnit")
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var viewModel = new RegisteredUnitDashboardViewModel
            {
                UserName = user.Name,
                UserEmail = user.Email,
                UnitType = user.UnitType,
                Listings = _context.Listings.Where(l => l.Email == email).ToList(),
                Drivers = _context.Drivers.Where(d => d.Email == email).ToList(),
                Advertisements = _context.Advertisements.Where(a => a.Email == email).ToList()
            };

            if (user.UnitType == "Listing")
            {
                var latestListing = viewModel.Listings.OrderByDescending(x => x.ListingId).FirstOrDefault();
                if (latestListing != null)
                {
                    viewModel.ApprovalStatus = latestListing.Status;
                    if (latestListing.Status == "Pending")
                    {
                        viewModel.HasFullAccess = false;
                        viewModel.ApprovalMessage = "Your company profile is waiting for admin approval.";
                    }
                    else if (latestListing.Status == "Rejected")
                    {
                        viewModel.HasFullAccess = false;
                        viewModel.ApprovalMessage = "Your company profile was rejected. Please contact admin support.";
                    }
                }
            }

            if (user.UnitType == "Driver")
            {
                var latestDriver = viewModel.Drivers.OrderByDescending(x => x.DriverId).FirstOrDefault();
                if (latestDriver != null)
                {
                    viewModel.ApprovalStatus = latestDriver.Status;
                    if (latestDriver.Status == "Pending")
                    {
                        viewModel.HasFullAccess = false;
                        viewModel.ApprovalMessage = "Waiting for Admin Approval.";
                    }
                    else if (latestDriver.Status == "Rejected")
                    {
                        viewModel.HasFullAccess = false;
                        viewModel.ApprovalMessage = "Your driver profile was rejected. Please contact admin support.";
                    }
                }
            }

            // Get payment history for this user
            var payments = _context.Payments
                .Where(p =>
                    (user.UnitType == "Listing" && p.EntityType == "Listing" && _context.Listings.Any(l => l.ListingId == p.EntityId && l.Email == email)) ||
                    (user.UnitType == "Driver" && p.EntityType == "Driver" && _context.Drivers.Any(d => d.DriverId == p.EntityId && d.Email == email)) ||
                    (user.UnitType == "Advertise" && p.EntityType == "Advertisement" && _context.Advertisements.Any(a => a.AdvertiseId == p.EntityId && a.Email == email)))
                .OrderByDescending(p => p.PaymentDate)
                .Take(5)
                .ToList();

            viewModel.RecentPayments = payments;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult MarkListingPaid(int id)
        {
            var item = _context.Listings.FirstOrDefault(l => l.ListingId == id);
            if (item != null)
            {
                item.PaymentStatus = "Paid";
                _context.SaveChanges();
            }

            return RedirectToAction("UnitDashboard");
        }

        [HttpPost]
        public IActionResult MarkDriverPaid(int id)
        {
            var item = _context.Drivers.FirstOrDefault(d => d.DriverId == id);
            if (item != null)
            {
                item.PaymentStatus = "Paid";
                _context.SaveChanges();
            }

            return RedirectToAction("UnitDashboard");
        }

        [HttpPost]
        public IActionResult MarkAdvertisePaid(int id)
        {
            var item = _context.Advertisements.FirstOrDefault(a => a.AdvertiseId == id);
            if (item != null)
            {
                item.PaymentStatus = "Paid";
                _context.SaveChanges();
            }

            return RedirectToAction("UnitDashboard");
        }

        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                // Generate reset token
                var resetToken = Guid.NewGuid().ToString();
                user.ResetToken = resetToken;
                user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour
                _context.SaveChanges();

                // In a real application, you would send an email with the reset link
                // For now, we'll just show the token in TempData for testing
                TempData["Success"] = $"Password reset link sent. (Token: {resetToken})";
            }
            else
            {
                // Don't reveal if email exists for security
                TempData["Success"] = "If an account exists with this email, a password reset link has been sent.";
            }

            return View();
        }

        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Invalid reset token.";
                return RedirectToAction("ForgotPassword");
            }

            var user = _context.Users.FirstOrDefault(u => u.ResetToken == token && u.ResetTokenExpiry > DateTime.UtcNow);
            if (user == null)
            {
                TempData["Error"] = "Invalid or expired reset token.";
                return RedirectToAction("ForgotPassword");
            }

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string token, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.ResetToken == token && u.ResetTokenExpiry > DateTime.UtcNow);
            if (user == null)
            {
                TempData["Error"] = "Invalid or expired reset token.";
                return RedirectToAction("ForgotPassword");
            }

            user.Password = newPassword;
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            _context.SaveChanges();

            TempData["Success"] = "Password has been reset successfully. Please login with your new password.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Services() => View();

        public IActionResult Feedback() => View();

    }
}


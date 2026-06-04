using Microsoft.AspNetCore.Mvc;
using Radiocab.Models;
using Radiocab.Models.SqlDb;

namespace Radiocab.Controllers
{
    public class UserController : Controller
    {
        private readonly SqlDbContext _context;

        public UserController(SqlDbContext context)
        {
            _context = context;
        }


        public IActionResult Listing()
        {
            return View(new Listing());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Listing listing, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                // ✅ Handle image upload safely
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/NewFolder");
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(directory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }

                    listing.ImagePath = "/images/NewFolder/" + fileName;
                }

                // ✅ Ensure no nulls in required fields
                listing.Name ??= "";
                listing.Telephone ??= "";
                listing.Email ??= "";
                listing.CompanyId = null; // 👈 added to prevent foreign key error

                try
                {
                    _context.Listings.Add(listing);
                    _context.SaveChanges();

                    TempData["Success"] = "Listing registered successfully!";
                    return RedirectToAction("Listing");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Database Error: " + (ex.InnerException?.Message ?? ex.Message);
                    return View("Listing", listing);
                }
            }

            TempData["Error"] = "Please fill all required fields!";
            return View("Listing", listing);
        }
    }
}
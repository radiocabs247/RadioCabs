using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Radiocab.Models
{
    [Table("Advertisements")] // 👈 match the SQL table name exactly
    public class Advertise
    {
        [Key]
        public int AdvertiseId { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty; // Company Name

        [StringLength(100, ErrorMessage = "Designation cannot exceed 100 characters")]
        public string? Designation { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address { get; set; }

        [Phone(ErrorMessage = "Invalid mobile number")]
        [StringLength(20, ErrorMessage = "Mobile number cannot exceed 20 characters")]
        public string? Mobile { get; set; }

        [Phone(ErrorMessage = "Invalid telephone number")]
        [StringLength(20, ErrorMessage = "Telephone number cannot exceed 20 characters")]
        public string? Telephone { get; set; }

        [StringLength(20, ErrorMessage = "Fax number cannot exceed 20 characters")]
        public string? FaxNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public string PaymentType { get; set; } = PaymentCatalog.Monthly;
        public decimal PaymentAmount { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public string? BannerPath { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

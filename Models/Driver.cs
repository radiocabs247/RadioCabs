using System.ComponentModel.DataAnnotations;

namespace Radiocab.Models
{
    public class Driver
    {
        [Key]
        public int DriverId { get; set; }

        [Required(ErrorMessage = "Driver name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Driver ID is required")]
        [StringLength(50, ErrorMessage = "Driver ID cannot exceed 50 characters")]
        public string DriverCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Contact person cannot exceed 100 characters")]
        public string ContactPerson { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid mobile number")]
        [StringLength(20, ErrorMessage = "Mobile number cannot exceed 20 characters")]
        public string Mobile { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid telephone number")]
        [StringLength(20, ErrorMessage = "Telephone number cannot exceed 20 characters")]
        public string Telephone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years")]
        public int Experience { get; set; } = 0;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";
        public string PaymentType { get; set; } = PaymentCatalog.Monthly;
        public decimal PaymentAmount { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public string? PhotoPath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

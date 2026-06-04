using System.ComponentModel.DataAnnotations;

namespace Radiocab.Models
{
    public class Users
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = "RegisteredUnit"; // Admin or RegisteredUnit

        [Required(ErrorMessage = "Unit type is required")]
        public string UnitType { get; set; } = "Listing"; // Listing, Driver, Advertise

        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
    }

}


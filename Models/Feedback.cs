using System.ComponentModel.DataAnnotations;

namespace Radiocab.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid mobile number")]
        [StringLength(20, ErrorMessage = "Mobile number cannot exceed 20 characters")]
        public string? MobileNo { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }

        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string? Type { get; set; } // Complaint, Suggestion, Compliment

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Approved, Hidden
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

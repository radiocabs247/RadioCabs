using System.ComponentModel.DataAnnotations;

namespace Radiocab.Models
{
    public class ActivityLog
    {
        [Key]
        public int ActivityLogId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public string? UserName { get; set; }

        [Required]
        public string Action { get; set; } = string.Empty; // Login, Logout, Create, Update, Delete, Approve, Reject, Payment

        public string? EntityType { get; set; } // Listing, Driver, Advertisement, Feedback, Payment

        public int? EntityId { get; set; }

        public string? Description { get; set; }

        public string? IpAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

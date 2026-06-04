using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Radiocab.Models
{
    public class Listing
    {
        [Key]
        public int ListingId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string CompanyCode { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? Designation { get; set; }

        [Required]
        public string City { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Fax { get; set; }
        public string? Telephone { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public string? Address { get; set; }

        public string? ImagePath { get; set; }

        // ✅ MAIN FIX: use only this
        [Required]
        public string MembershipType { get; set; } = "Free";

        public string Status { get; set; } = "Pending";

        public string PaymentType { get; set; } = PaymentCatalog.Monthly;

        public decimal PaymentAmount { get; set; }

        public string PaymentStatus { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("Company")]
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
    }
}
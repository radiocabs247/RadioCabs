using System.ComponentModel.DataAnnotations;

namespace Radiocab.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? City { get; set; }
        public string? ContactNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Description { get; set; }
        public string? LogoPath { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    }
}

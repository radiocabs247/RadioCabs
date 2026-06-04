using System.ComponentModel.DataAnnotations;

namespace Radiocab.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public string TransactionId { get; set; } = string.Empty;

        [Required]
        public string EntityType { get; set; } = string.Empty;

        [Required]
        public int EntityId { get; set; }

        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        public string PaymentType { get; set; } = PaymentCatalog.Monthly;

        public string Status { get; set; } = "Pending";

        public string? Description { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        public string? ReceiptNumber { get; set; }

        public string? Notes { get; set; }
    }
}
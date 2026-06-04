namespace Radiocab.Models
{
    public class RegisteredUnitDashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UnitType { get; set; } = string.Empty;
        public string? ApprovalStatus { get; set; }
        public string? ApprovalMessage { get; set; }
        public bool HasFullAccess { get; set; } = true;
        public List<Listing> Listings { get; set; } = new();
        public List<Driver> Drivers { get; set; } = new();
        public List<Advertise> Advertisements { get; set; } = new();
        public List<Payment> RecentPayments { get; set; } = new();
    }
}

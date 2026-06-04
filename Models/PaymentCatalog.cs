namespace Radiocab.Models
{
    public static class PaymentCatalog
    {
        public const string Monthly = "Monthly";
        public const string Quarterly = "Quarterly";

        public static decimal GetAmount(string section, string paymentType, string membershipType = "")
        {
            var normalizedPayment = paymentType?.Trim().ToLowerInvariant() ?? string.Empty;
            var normalizedSection = section?.Trim().ToLowerInvariant() ?? string.Empty;
            var normalizedMember = membershipType?.Trim().ToLowerInvariant() ?? "free";

            if (normalizedSection == "registration" || normalizedSection == "company" || normalizedSection == "listing")
            {
                return (normalizedMember, normalizedPayment) switch
                {
                    ("premium", "monthly") => 25m,
                    ("premium", "quarterly") => 70m,
                    ("basic", "monthly") => 15m,
                    ("basic", "quarterly") => 40m,
                    _ => 0m
                };
            }

            return (normalizedSection, normalizedPayment) switch
            {
                ("driver", "monthly") => 10m,
                ("driver", "quarterly") => 25m,
                ("advertisement", "monthly") => 15m,
                ("advertisement", "quarterly") => 40m,
                _ => 0m
            };
        }
    }
}

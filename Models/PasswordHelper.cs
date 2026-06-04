using Microsoft.AspNetCore.Identity;

namespace Radiocab.Models
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<object> Hasher = new PasswordHasher<object>();

        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;
            return Hasher.HashPassword(new object(), password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(password))
                return false;

            // Handle legacy plain text passwords (e.g. seeded admin)
            if (!hashedPassword.Contains("AQAAAAIAAYagAAAA") && !hashedPassword.StartsWith("AQAAAA") && hashedPassword == password)
            {
                return true;
            }

            try
            {
                var result = Hasher.VerifyHashedPassword(new object(), hashedPassword, password);
                return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
            }
            catch
            {
                // Fallback to plain text matching in case of legacy hash comparison issues
                return hashedPassword == password;
            }
        }
    }
}

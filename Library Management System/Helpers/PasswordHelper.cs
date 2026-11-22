using System.Security.Cryptography;
using System.Text;

namespace Library_Management_System.Helpers;

public static class PasswordHelper
{
        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        public static bool VerifyPassword(string password, string hash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hash;
        }
    
}
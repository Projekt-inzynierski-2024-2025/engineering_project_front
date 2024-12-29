using System.Security.Cryptography;
using System.Text;

namespace engineering_project_front.Services
{
    public static class EncryptionHelper
    {
        private const string SecretKey = "TwójSekretnyKlucz"; // Stały klucz szyfrowania

        // Kodowanie ID z URL-safe Base64
        public static string Encrypt(string id)
        {
            var keyBytes = Encoding.UTF8.GetBytes(SecretKey);
            using var hmac = new HMACSHA256(keyBytes);

            var dataBytes = Encoding.UTF8.GetBytes(id);
            var hashBytes = hmac.ComputeHash(dataBytes);

            // Dodajemy oryginalne dane na końcu (potrzebne do późniejszego dekodowania)
            var combinedBytes = new byte[hashBytes.Length + dataBytes.Length];
            Buffer.BlockCopy(hashBytes, 0, combinedBytes, 0, hashBytes.Length);
            Buffer.BlockCopy(dataBytes, 0, combinedBytes, hashBytes.Length, dataBytes.Length);

            // Zamieniamy Base64 na URL-safe Base64
            var base64 = Convert.ToBase64String(combinedBytes);
            return base64.Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        // Dekodowanie ID z URL-safe Base64
        public static string Decrypt(string encryptedId)
        {
            // Przywracamy Base64 do oryginalnej postaci
            var base64 = encryptedId.Replace("-", "+").Replace("_", "/");
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            var combinedBytes = Convert.FromBase64String(base64);
            var keyBytes = Encoding.UTF8.GetBytes(SecretKey);
            using var hmac = new HMACSHA256(keyBytes);

            // Wyciągamy oryginalne dane (część hashowana nie jest nam potrzebna do odzyskania ID)
            var originalDataLength = combinedBytes.Length - hmac.HashSize / 8;
            var originalData = new byte[originalDataLength];
            Buffer.BlockCopy(combinedBytes, hmac.HashSize / 8, originalData, 0, originalDataLength);

            return Encoding.UTF8.GetString(originalData);
        }
    }
}

using System;
using System.Security.Cryptography;
using System.Text;

namespace HotelApp.Helpers
{
    public class GenerateHashPassword
    {
        // Метод для генерации случайной соли
        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16]; // 16 байт - достаточно для хорошей соли
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        // Метод для хеширования пароля с солью
        public static string GenerateMD5Hash(string password, byte[] salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword = new byte[salt.Length + passwordBytes.Length];
            Array.Copy(salt, 0, saltedPassword, 0, salt.Length);
            Array.Copy(passwordBytes, 0, saltedPassword, salt.Length, passwordBytes.Length);

            using (MD5CryptoServiceProvider hash = new MD5CryptoServiceProvider())
            {
                byte[] hashBytes = hash.ComputeHash(saltedPassword);
                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}

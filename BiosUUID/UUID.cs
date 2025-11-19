using System;
using System.Management;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace HotelApp.BiosUUID
{
    public class UUID
    {
        public static string GetBiosUUID()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj["SerialNumber"]?.ToString(); // Или используйте obj["UUID"]
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex}", "Error", MessageBoxButton.OK);
            }
            return null;
        }

        public static string GenerateEncryptionKey(string biosUUID)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(biosUUID));
                return BitConverter.ToString(hashBytes); // Преобразование в hex-строку
            }
        }
    }
}

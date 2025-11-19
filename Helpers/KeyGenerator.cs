using System;
using System.Linq;
using System.Security.Cryptography;

namespace HotelApp.Helpers
{
    public class PasswordGenerator
    {
        // Все допустимые наборы символов
        private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "0123456789";
        private const string SpecialChars = "!\"#$%&'()*+,-./:;<=>?@[$$^_`{|}~";

        // Объединённая строка всех разрешённых символов
        private const string AllChars = LowerCase + UpperCase + Digits + SpecialChars;

        // Метод генерации сложного пароля
        public static string GenerateSecurePassword(int length = 16)
        {
            if (length < 12)
                throw new ArgumentException("Длина пароля должна быть не менее 12 символов.");

            using (var rng = RandomNumberGenerator.Create())
            {
                var password = new char[length];
                var byteBuffer = new byte[length];

                rng.GetBytes(byteBuffer);

                // Всегда добавляем хотя бы по одному символу из каждой группы
                password[0] = GetRandomChar(LowerCase, rng);
                password[1] = GetRandomChar(UpperCase, rng);
                password[2] = GetRandomChar(Digits, rng);
                password[3] = GetRandomChar(SpecialChars, rng);

                // Остальные символы выбираются случайно из всех категорий
                for (int i = 4; i < length; i++)
                {
                    password[i] = GetRandomChar(AllChars, rng);
                }

                // Перемешиваем массив, чтобы гарантировать случайность позиций обязательных символов
                return new string(Shuffle(password, rng).ToArray());
            }
        }

        // Получает один случайный символ из заданной строки
        private static char GetRandomChar(string charSet, RandomNumberGenerator rng)
        {
            var buffer = new byte[1];
            rng.GetBytes(buffer);
            int index = buffer[0] % charSet.Length;
            return charSet[index];
        }

        // Перемешивает массив с помощью RNG
        private static T[] Shuffle<T>(T[] array, RandomNumberGenerator rng)
        {
            var copy = array.ToArray();
            for (int i = 0; i < copy.Length; i++)
            {
                var buffer = new byte[1];
                rng.GetBytes(buffer);
                int j = buffer[0] % copy.Length;
                (copy[i], copy[j]) = (copy[j], copy[i]);
            }
            return copy;
        }

        // Точка входа для тестирования
        public static string generateKey()
        { 
            return GenerateSecurePassword();
        }
    }
}

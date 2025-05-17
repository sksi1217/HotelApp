using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using HotelApp.Models;

namespace HotelApp.Helpers
{
    public static class TelegramBotHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string BotToken = "7439658865:AAGoxFJ7Gw77--gKLskMuRSRhSgr5yhaFFQ";
        private const long AdminChatId = 1277061126;

        public static async Task SendKey(string key)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("chat_id", AdminChatId.ToString()),
                    new KeyValuePair<string, string>("text", key),
                    new KeyValuePair<string, string>("parse_mode", "Markdown")
                });

                var response = await _httpClient.PostAsync($"https://api.telegram.org/bot{BotToken}/sendMessage", content);

                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Ошибка Telegram API: {response.StatusCode}, Ответ: {responseBody}");
                    MessageBox.Show($"Ошибка отправки ключа: {response.StatusCode}\n{responseBody}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке уведомления в Telegram: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public static async Task SendMessageSession()
        {
            try
            {
                string message = $"⛔ *Сессия закончалась*";

                string url = $"https://api.telegram.org/bot{BotToken}/sendMessage?" +
                            $"chat_id={AdminChatId}&" +
                            $"text={Uri.EscapeDataString(message)}&" +
                            $"parse_mode=Markdown";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Ошибка при отправке сообщения: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке уведомления в Telegram: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public static async Task SendAccountConfirmationAsync(User newUser)
        {
            try
            {
                string userTypeEmoji = GetUserTypeEmoji(newUser.TypeUser);

                string message = $"🎉 *Новая учетная запись создана!* 🎉\n\n" +
                                $"👤 *Логин:* {newUser.Login}\n" +
                                $"🛠 *Тип пользователя:* {userTypeEmoji} {newUser.TypeUser}\n" +
                                $"📅 *Дата создания:* {DateTime.Now:dd.MM.yyyy HH:mm}\n\n" +
                                $"✅ Учетная запись успешно добавлена в систему!";

                string url = $"https://api.telegram.org/bot{BotToken}/sendMessage?" +
                            $"chat_id={AdminChatId}&" +
                            $"text={Uri.EscapeDataString(message)}&" +
                            $"parse_mode=Markdown";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Ошибка при отправке сообщения: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке уведомления в Telegram: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private static string GetUserTypeEmoji(User.UserType userType)
        {
            switch (userType)
            {
                case User.UserType.Administrator:
                    return "👑";
                case User.UserType.Worker:
                    return "👨‍💼";
                default:
                    return "👤";
            }
        }

        public static async Task SendWelcomeStickerAsync()
        {
            try
            {
                string stickerId = "CAACAgIAAxkBAAEL3hVmE6Jq2z1P8wABHX7vZfI5Qe0Ue_UAAl4BAAJWnb0K9JxVKkQN3z80BA";

                string url = $"https://api.telegram.org/bot{BotToken}/sendSticker?" +
                             $"chat_id={AdminChatId}&" +
                             $"sticker={stickerId}";

                await _httpClient.GetAsync(url);
            }
            catch
            {
                // Игнорируем ошибки со стикерами
            }
        }
    }
}
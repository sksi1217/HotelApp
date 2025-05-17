using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using HotelApp.Helpers;
using HotelApp.Models;
using HotelApp.Views;

namespace HotelApp
{
    public partial class App : Application
    {
        // private const string AdminKey = "123"; // Секретный ключ администратора
        private const string DbPath = "hotel.db"; // Путь к базе данных

        public static User CurrentUser;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeDatabase();

            var mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;

            if (!DatabaseHelper.CheckAdminExists())
            {
                string key = PasswordGenerator.generateKey();
                await TelegramBotHelper.SendKey(key);

                var adminKeyWindow = new AdminKeyWindow();
                bool? dialogResult = false;
                bool isTimedOut = false;

                // Создаем таймер на 10 секунд
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(10);
                timer.Tick += async (s, args) =>
                {
                    timer.Stop();
                    adminKeyWindow.Close(); // Закрываем окно ввода
                    isTimedOut = true;
                    await TelegramBotHelper.SendMessageSession();
                    Shutdown();
                };

                // Подписываемся на закрытие окна, чтобы остановить таймер
                EventHandler windowClosedHandler = null;
                windowClosedHandler = (sender, args) =>
                {
                    timer.Stop();
                    adminKeyWindow.Closed -= windowClosedHandler;
                };
                adminKeyWindow.Closed += windowClosedHandler;

                timer.Start(); // Запускаем таймер

                dialogResult = adminKeyWindow.ShowDialog();

                // Если время истекло — выходим
                if (isTimedOut || dialogResult != true)
                {
                    return;
                }

                if (adminKeyWindow.EnteredKey == key)
                {
                    string login = adminKeyWindow.LoginTextBox.Text;
                    string password = adminKeyWindow.PasswordTextBox.Password;

                    if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                    {
                        MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    byte[] salt = GenerateHashPassword.GenerateSalt();
                    string hashedPassword = GenerateHashPassword.GenerateMD5Hash(password, salt);

                    User newUser = new User
                    {
                        Login = login,
                        Password = hashedPassword,
                        Salt = salt,
                        TypeUser = User.UserType.Administrator,
                        TypeWorkForWorker = User.WorkForWorkerType.None
                    };

                    DatabaseHelper.AddUser(newUser);
                    await TelegramBotHelper.SendAccountConfirmationAsync(newUser);
                    await TelegramBotHelper.SendWelcomeStickerAsync();
                }
                else
                {
                    MessageBox.Show("Неверный ключ администратора. Приложение будет закрыто.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                    return;
                }
            }

            mainWindow.Show();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(DbPath))
            {
                // Создаем базу данных, если она не существует
                DatabaseHelper.InitializeDatabase();
            }
        }
    }
}
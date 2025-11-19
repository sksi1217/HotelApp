using System;
using System.IO;
using System.Windows;
using HotelApp.Helpers;
using HotelApp.Models;
using HotelApp.Views;

namespace HotelApp
{
    public partial class App : Application
    {
        private const string DbPath = "hotel.db"; // Путь к базе данных

        public static User CurrentUser;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeDatabase();

            // Всегда запускаем главное окно
            var mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(DbPath))
            {
                // Создаём базу данных, если она не существует
                DatabaseHelper.InitializeDatabase();
            }
        }
    }
}
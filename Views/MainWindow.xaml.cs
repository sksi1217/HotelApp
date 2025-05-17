using System;
using System.Windows;
using HotelApp.ViewModels;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Input;
using HotelApp.Views.UserControl;
using HotelApp.Views.Groups;
using HotelApp.Helpers.Animations;
using HotelApp.BiosUUID;
using HotelApp.Properties;
using System.Linq;

namespace HotelApp
{
    public partial class MainWindow : Window
    {
        private bool isWindowOpen = false; // Флаг для отслеживания открытого окна
        public MainWindow()
        {
            InitializeComponent();

            AutoLogin();
        }

        #region AutoLogin
        private void AutoLogin()
        {
            if (Settings.Default.LastLogin == null)
            {
                MessageBox.Show("Вы не вошли в аккаунт!", "Инфо!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var user = DatabaseHelper.GetAllUsers().FirstOrDefault(u => u.Login == Settings.Default.LastLogin);

            if (user == null)
            {
                return;
            }

            string key = UUID.GenerateEncryptionKey(UUID.GetBiosUUID());

            if (user.Cookie != key)
            {
                user.Cookie = null;
                DatabaseHelper.UpdateCookieUserInDatabase(user);

                MessageBox.Show("Ошибка Cookie!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Settings.Default.LastLogin = null;
                Settings.Default.RememberMe = false;
                Settings.Default.Save();
                return;
            }
            Settings.Default.LastLogin = user.Login;
            Settings.Default.RememberMe = true;
            Settings.Default.Save();
            MessageBox.Show($"Добро пожаловать, {user.Login}! Тип пользователя: {user.TypeUser}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            App.CurrentUser = user;
        }
        #endregion

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (isWindowOpen) return;

            var gameWindow = new LoginControl()
            {
                Width = 400,
                Height = 400,
                Opacity = 0
            };

            WindowContainer.Children.Add(gameWindow);

            // Явно задаем начальные координаты
            Canvas.SetLeft(gameWindow, (WindowContainer.ActualWidth - gameWindow.Width) / 2);
            Canvas.SetTop(gameWindow, (WindowContainer.ActualHeight - gameWindow.Height) / 2);

            AnimateWindowAppear(gameWindow);

            isWindowOpen = true;

            gameWindow.Closed += (s, args) =>
            {
                isWindowOpen = false;
            };
        }

        private void Panel_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("Сначала войдите в систему!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                switch (App.CurrentUser.TypeUser)
                {
                    case Models.User.UserType.Administrator:
                        // Логика для администратора
                        AdminPanel();
                        break;

                    case Models.User.UserType.Guest:
                        // Логика для гостя
                        GuestPanel();
                        break;

                    case Models.User.UserType.Worker:
                        // Логика для работника
                        WorkerPanel();
                        break;

                    default:
                        MessageBox.Show("Неизвестная роль пользователя");
                        break;
                }
            }

        }

        private void AdminPanel()
        {
            if (isWindowOpen) return;

            var gameWindow = new AdminControl()
            {
                Width = 800,
                Height = 600,
                Opacity = 0
            };

            WindowContainer.Children.Add(gameWindow);

            // Явно задаем начальные координаты
            Canvas.SetLeft(gameWindow, (WindowContainer.ActualWidth - gameWindow.Width) / 2);
            Canvas.SetTop(gameWindow, (WindowContainer.ActualHeight - gameWindow.Height) / 2);

            AnimateWindowAppear(gameWindow);

            isWindowOpen = true;

            gameWindow.Closed += (s, args) =>
            {
                isWindowOpen = false;
            };
        }

        private void WorkerPanel()
        {
            if (isWindowOpen) return;

            var gameWindow = new WorkerControl()
            {
                Width = 800,
                Height = 600,
                Opacity = 0
            };

            WindowContainer.Children.Add(gameWindow);

            // Явно задаем начальные координаты
            Canvas.SetLeft(gameWindow, (WindowContainer.ActualWidth - gameWindow.Width) / 2);
            Canvas.SetTop(gameWindow, (WindowContainer.ActualHeight - gameWindow.Height) / 2);

            AnimateWindowAppear(gameWindow);

            isWindowOpen = true;

            gameWindow.Closed += (s, args) =>
            {
                isWindowOpen = false;
            };
        }

        private void GuestPanel()
        {
            if (isWindowOpen) return;

            var gameWindow = new GuestControl()
            {
                Width = 800,
                Height = 600,
                Opacity = 0
            };

            WindowContainer.Children.Add(gameWindow);

            // Явно задаем начальные координаты
            Canvas.SetLeft(gameWindow, (WindowContainer.ActualWidth - gameWindow.Width) / 2);
            Canvas.SetTop(gameWindow, (WindowContainer.ActualHeight - gameWindow.Height) / 2);

            AnimateWindowAppear(gameWindow);

            isWindowOpen = true;

            gameWindow.Closed += (s, args) =>
            {
                isWindowOpen = false;
            };
        }

        private void AnimateWindowAppear(UIElement gameWindow)
        {
            // Создаем TranslateTransform для анимации
            TranslateTransform transform = new TranslateTransform();
            gameWindow.RenderTransform = transform;

            // Анимация прозрачности
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            // Анимация перемещения
            DoubleAnimation moveAnimation = new DoubleAnimation
            {
                From = 50, // Небольшое смещение вниз
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            // Применяем анимации
            gameWindow.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
            transform.BeginAnimation(TranslateTransform.YProperty, moveAnimation);
        }

        private void DragWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove(); // <-- Это стандартный способ перетаскивания окна
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
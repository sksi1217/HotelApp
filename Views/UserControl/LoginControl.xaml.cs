using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using HotelApp.BiosUUID;
using HotelApp.Helpers;
using HotelApp.Helpers.Animations;
using HotelApp.Models;
using HotelApp.Properties;
using HotelApp.Views.Groups;

namespace HotelApp.Views.UserControl
{
    /// <summary>
    /// Логика взаимодействия для LoginControl.xaml
    /// </summary>
    public partial class LoginControl : System.Windows.Controls.UserControl
    {
        private bool isWindowOpen = false;
        public event EventHandler Closed;

        public LoginControl()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем родительский контейнер
            var parent = this.Parent as Panel;
            if (parent == null)
            {
                MessageBox.Show("Невозможно закрыть окно: родительский контейнер не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Запускаем анимацию закрытия
            Animations.AnimateWindowClose(this, parent, () =>
            {
                // Вызываем событие "Closed" после завершения анимации
                Closed?.Invoke(this, EventArgs.Empty);
            });
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User user = DatabaseHelper.GetUserByLogin(login);

            if (App.CurrentUser != null && App.CurrentUser.Login == login)
            {
                LoginTextBox.Text = null;
                PasswordTextBox.Password = null;
                MessageBox.Show("Вы уже вошли в этого пользователя!", "Info!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (user != null)
            {
                string hashedPassword = GenerateHashPassword.GenerateMD5Hash(password, user.Salt);
                if (hashedPassword == user.Password)
                {
                    // Сохранение логина, если "запомнить меня" отмечено
                    if (RememberMeCheckBox.IsChecked == true)
                    {
                        Settings.Default.LastLogin = login;
                        Settings.Default.RememberMe = true;


                        string biosUUID = UUID.GetBiosUUID();

                        user.Cookie = UUID.GenerateEncryptionKey(UUID.GetBiosUUID());
                        // user.OldPassword = hashedPassword;

                        DatabaseHelper.UpdateCookieUserInDatabase(user);

                        // UpdateOldPasswordUser(user);
                    }
                    else
                    {
                        Settings.Default.LastLogin = null;
                        Settings.Default.RememberMe = false;
                        user.Cookie = null;
                        // user.OldPassword = null;
                    }
                    Settings.Default.Save();

                    App.CurrentUser = user;
                    CloseButton_Click(sender, e);

                    return;
                }
            }
            else
            {
                MessageBox.Show("Такого пользователя не существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (isWindowOpen) return;

            var gameWindow = new RegistrationControl()
            {
                Width = 400,
                Height = 400,
                Opacity = 0
            };

            WindowContainer.Children.Add(gameWindow);

            // Явно задаем начальные координаты
            Canvas.SetLeft(gameWindow, (WindowContainer.ActualWidth - gameWindow.Width) / 2);
            Canvas.SetTop(gameWindow, (WindowContainer.ActualHeight - gameWindow.Height) / 2);

            Animations.AnimateWindowAppear(gameWindow);

            isWindowOpen = true;

            gameWindow.Closed += (s, args) =>
            {
                isWindowOpen = false;
            };
        }

        private void DragWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.CaptureMouse();

                TranslateTransform transform = new TranslateTransform();

                double initialX = Canvas.GetLeft(this);
                double initialY = Canvas.GetTop(this);

                if (double.IsNaN(initialX)) initialX = 0;
                if (double.IsNaN(initialY)) initialY = 0;

                Point offset = e.GetPosition(this);
                double offsetX = offset.X;
                double offsetY = offset.Y;

                MouseEventHandler moveHandler = null;
                moveHandler = (moveSender, moveArgs) =>
                {
                    if (moveArgs.LeftButton == MouseButtonState.Pressed)
                    {
                        var parent = this.Parent as FrameworkElement;
                        if (parent != null)
                        {
                            Point currentPosition = moveArgs.GetPosition(parent);

                            double newX = currentPosition.X - offsetX;
                            double newY = currentPosition.Y - offsetY;

                            newX = Math.Max(0, Math.Min(newX, parent.ActualWidth - this.ActualWidth));
                            newY = Math.Max(0, Math.Min(newY, parent.ActualHeight - this.ActualHeight));

                            transform.X = newX;
                            transform.Y = newY;

                            Canvas.SetLeft(this, newX);
                            Canvas.SetTop(this, newY);
                        }
                    }
                };

                MouseButtonEventHandler upHandler = null;
                upHandler = (upSender, upArgs) =>
                {
                    this.ReleaseMouseCapture();
                    this.MouseMove -= moveHandler;
                    this.MouseUp -= upHandler;
                };

                this.MouseMove += moveHandler;
                this.MouseUp += upHandler;
            }
        }
    }
}

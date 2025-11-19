using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
using HotelApp.Helpers;
using HotelApp.Helpers.Animations;
using HotelApp.Models;

namespace HotelApp.Views.UserControl
{
    /// <summary>
    /// Логика взаимодействия для RegistrationControl.xaml
    /// </summary>
    public partial class RegistrationControl : System.Windows.Controls.UserControl
    {

        public event EventHandler Closed;
        public RegistrationControl()
        {
            InitializeComponent();
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

        private void RegisterLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;
            User.UserType selectedType = (User.UserType)TypeUserComboBox.SelectedIndex; // Получаем тип пользователя из ComboBox


            // Получаем тип работы
            User.WorkForWorkerType workType;

            if (selectedType == User.UserType.Worker)
            {
                workType = (User.WorkForWorkerType)TypeWorkForWokerComboBox.SelectedIndex; 
            }
            else
            {
                workType = User.WorkForWorkerType.None;
            }

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            byte[] salt = GenerateHashPassword.GenerateSalt(); // Generate salt
            string hashedPassword = GenerateHashPassword.GenerateMD5Hash(password, salt); // Hash password with salt

            // Генерируем ключ шифрования
            // string encryptionKey = UUID.GenerateEncryptionKey(UUID.GetBiosUUID());

            User newUser = new User
            {
                Login = login,
                Password = hashedPassword,
                Salt = salt,
                TypeUser = selectedType,
                TypeWorkForWorker = workType + 1
            };
            try
            {
                DatabaseHelper.AddUser(newUser);
                MessageBox.Show("Пользователь успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoginTextBox.Text = null;
                PasswordTextBox.Password = null;
                // this.Close();
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE constraint failed: Users.Login"))
            {
                MessageBox.Show($"Ошибка регистрации: Такой пользователь уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void TypeUserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeUserComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem.Content?.ToString() == "Worker")
                {
                    // Показываем поле для выбора типа работы
                    TypeWorkForWokerComboBox.Visibility = Visibility.Visible;
                    WorkTypeLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    // Скрываем
                    TypeWorkForWokerComboBox.Visibility = Visibility.Collapsed;
                    WorkTypeLabel.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                // Если ничего не выбрано — скрываем
                TypeWorkForWokerComboBox.Visibility = Visibility.Collapsed;
                WorkTypeLabel.Visibility = Visibility.Collapsed;
            }
        }
    }
}

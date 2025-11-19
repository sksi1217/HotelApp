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
using HotelApp.Helpers;
using HotelApp.Helpers.Animations;
using HotelApp.Models;

namespace HotelApp.Views.UserControl
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWork.xaml
    /// </summary>
    public partial class RegistrationWork : System.Windows.Controls.UserControl
    {
        public event EventHandler Closed;
        public RegistrationWork()
        {
            InitializeComponent();
        }

        private void RegisterWorker_Click(object sender, RoutedEventArgs e)
        {
            string firstname = FirstNameTextBox.Text?.Trim();
            string lastname = LastNameTextBox.Text?.Trim();
            string phone = PhoneTextBox.Text?.Trim();

            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(firstname) || string.IsNullOrWhiteSpace(lastname))
            {
                MessageBox.Show("Пожалуйста, заполните имя и фамилию.", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка телефона
            if (!string.IsNullOrWhiteSpace(phone) && !IsValidPhone(phone))
            {
                MessageBox.Show("Введите корректный номер телефона.", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка выбора типа работника
            if (!(TypeWorkComboBox.SelectedItem is ComboBoxItem selectedItem) || selectedItem.Tag == null)
            {
                MessageBox.Show("Выберите тип работника из списка.", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(selectedItem.Tag.ToString(), out int workerTypeId))
            {
                MessageBox.Show("Неверное значение типа работника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Worker.WorkerType workerType = (Worker.WorkerType)workerTypeId;

            Worker newWorker = new Worker
            {
                FirstName = firstname,
                LastName = lastname,
                Phone = phone,
                TypeWorker = workerType
            };

            try
            {
                DatabaseHelper.AddWorker(newWorker);
                MessageBox.Show("Работник успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Очистка полей
                FirstNameTextBox.Clear();
                LastNameTextBox.Clear();
                PhoneTextBox.Clear();
                TypeWorkComboBox.SelectedIndex = -1; // Сброс выбора
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Удаляем все символы, кроме цифр
            string digits = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");

            // Проверяем длину после очистки
            if (digits.Length != 11)
                return false;

            // Проверяем, начинается ли номер с 7 или 8
            return digits.StartsWith("7") || digits.StartsWith("8");
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

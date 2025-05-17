using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using HotelApp.Helpers.Animations;
using HotelApp.Models;

namespace HotelApp.Views.Groups.WorkPanel.Concierge
{
    /// <summary>
    /// Логика взаимодействия для BookingConciergeControl.xaml
    /// </summary>
    public partial class BookingConciergeControl : System.Windows.Controls.UserControl
    {
        public event EventHandler Closed;
        public Room selectedRoom { get; set; }
        public BookingConciergeControl(Room room)
        {
            InitializeComponent();
            selectedRoom = room;
        }

        private decimal PriceCalculateTotal(DateTime checkInDate, DateTime checkOutDate)
        {
            if (selectedRoom == null)
            {
                MessageBox.Show("Выберите комнату!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return 0;
            }

            // Расчет общей стоимости
            int nights = (checkOutDate - checkInDate).Days;
            decimal totalPrice = nights * selectedRoom.PriceRoom;
            return totalPrice;
        }

        private void Book_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRoom == null)
            {
                MessageBox.Show("Выберите комнату!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedRoom.StatusRoom != Status.Free)
            {
                MessageBox.Show("Номер уже заронирован...", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!ValidateFields(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int guestCount = int.Parse(((ComboBoxItem)GuestCountComboBox.SelectedItem).Tag.ToString());
            PaymentMethod paymentMethod = (PaymentMethod)int.Parse(((ComboBoxItem)PaymentMethodComboBox.SelectedItem).Tag.ToString());
            Status statusRoom = (Status)int.Parse(((ComboBoxItem)PaymentMethodComboBox.SelectedItem).Tag.ToString());

            // Автоматический расчет времени заезда/выезда
            DateTime checkInDate = (StartDatePicker.SelectedDate.Value).Date.AddHours(14); // 14:00
            DateTime checkOutDate = (EndDatePicker.SelectedDate.Value).Date.AddHours(12); // 12:00

            // Здесь можно вызвать метод сохранения бронирования в БД
            var booking = new Room
            {
                Id = selectedRoom.Id,
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Email = EmailTextBox.Text,
                Phone = PhoneTextBox.Text,
                GuestCount = guestCount,
                TypeRoom = selectedRoom.TypeRoom,
                Payment = paymentMethod,
                IsPaid = true,
                StatusRoom = statusRoom,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                TotalAmount = PriceCalculateTotal(checkInDate, checkOutDate),
            };

            DatabaseHelper.UpdateRoomInDatabase(booking);

            MessageBox.Show("Бронирование успешно оформлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CalculateTotalPrice_Click(object sender, RoutedEventArgs e)
        {
            // Автоматический расчет времени заезда/выезда
            DateTime checkInDate = (StartDatePicker.SelectedDate.Value).Date.AddHours(14); // 14:00
            DateTime checkOutDate = (EndDatePicker.SelectedDate.Value).Date.AddHours(12); // 12:00

            totalCash.Text = PriceCalculateTotal(checkInDate, checkOutDate).ToString();
        }

        private bool ValidateFields(out string errorMessage)
        {
            errorMessage = "";

            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
                errorMessage += "Имя не может быть пустым.\n";

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
                errorMessage += "Фамилия не может быть пустой.\n";

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || !IsValidEmail(EmailTextBox.Text))
                errorMessage += "Введите корректный адрес электронной почты.\n";

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text) || !IsValidPhone(PhoneTextBox.Text))
                errorMessage += "Введите корректный номер телефона.\n";

            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
                errorMessage += "Выберите даты заезда и выезда.\n";
            else if (StartDatePicker.SelectedDate >= EndDatePicker.SelectedDate)
                errorMessage += "Дата заезда должна быть раньше даты выезда.\n";

            return string.IsNullOrEmpty(errorMessage);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
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

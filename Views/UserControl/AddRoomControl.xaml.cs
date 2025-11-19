using HotelApp.Helpers.Animations;
using HotelApp.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HotelApp.Views.UserControl
{
    public partial class AddRoomControl : System.Windows.Controls.UserControl
    {
        public event EventHandler Closed;
        public event Action OnRoomAdded; // Событие для обновления списка

        public AddRoomControl()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var room = new Room
                {
                    NameRoom = NameRoomBox.Text,
                    DescriptionRoom = DescriptionBox.Text,
                    PriceRoom = decimal.TryParse(PriceBox.Text, out var price) ? price : 0m,
                    NumberRoom = int.TryParse(NumberBox.Text, out var num) ? num : 0,
                    FloorRoom = FloorBox.Text,
                    TypeRoom = (RoomType)(TypeRoomCombo.SelectedIndex),
                    StatusRoom = (Status)(StatusRoomCombo.SelectedIndex),
                    // Остальные поля могут остаться null/по умолчанию
                };

                DatabaseHelper.AddRoom(room);
                MessageBox.Show("Номер успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                OnRoomAdded?.Invoke(); // Уведомить основное окно обновить список
                // Если это UserControl в модальном окне — закройте его
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении номера: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
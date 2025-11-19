using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HotelApp.Helpers.Animations;
using HotelApp.Models;
using HotelApp.ViewModels;
using HotelApp.Views.Groups;

namespace HotelApp.Views.UserControl
{
    /// <summary>
    /// Логика взаимодействия для BookRoomWindow.xaml
    /// </summary>
    public partial class BookRoomWindow : System.Windows.Controls.UserControl
    {
        public event EventHandler Closed;

        public BookRoomWindow()
        {
            InitializeComponent();

            // Инициализация ViewModel
            var viewModel = new ViewModel();
            DataContext = viewModel;

            // Загрузка данных из базы данных
            viewModel.LoadRoomsFromDatabase();
        }

        private void FindAvailableRooms_Click(object sender, RoutedEventArgs e)
        {
            /*
            // Получение выбранных дат
            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите даты заезда и выезда.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var startDate = StartDatePicker.SelectedDate.Value;
            var endDate = EndDatePicker.SelectedDate.Value;

            // Проверка корректности дат
            if (startDate >= endDate)
            {
                MessageBox.Show("Дата заезда должна быть раньше даты выезда.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получение списка всех комнат из ViewModel
            var viewModel = DataContext as ViewModel;
            if (viewModel == null || viewModel.Rooms == null)
            {
                MessageBox.Show("Нет данных о комнатах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Фильтрация комнат
            var availableRooms = viewModel.Rooms.Where(room =>
                room.StatusRoom == Status.Free || // Комната свободна
                (
                    // Комната занята, но даты не пересекаются
                    room.CheckInDate.HasValue && room.CheckOutDate.HasValue &&
                    (endDate <= room.CheckInDate.Value || startDate >= room.CheckOutDate.Value)
                )
            ).ToList();

            // Проверка результата
            if (availableRooms.Count == 0)
            {
                MessageBox.Show("Свободные номера не найдены.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                // Обновляем коллекцию Rooms в ViewModel
                viewModel.Rooms = new ObservableCollection<Room>(availableRooms);
                return;
            }

            // Отображение результатов поиска
            MessageBox.Show($"Найдено свободных номеров: {availableRooms.Count}", "Результат поиска", MessageBoxButton.OK, MessageBoxImage.Information);

            // Обновляем коллекцию Rooms в ViewModel
            viewModel.Rooms = new ObservableCollection<Room>(availableRooms);
            */
        }


        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Book_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную комнату из DataGrid
            var selectedRoom = RoomsDataGrid.SelectedItem as Room;
            if (selectedRoom == null)
            {
                MessageBox.Show("Выберите комнату для бронирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверяем, что даты заезда и выезда выбраны
            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите даты заезда и выезда.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var startDate = StartDatePicker.SelectedDate.Value;
            var endDate = EndDatePicker.SelectedDate.Value;

            // Проверяем корректность дат
            if (startDate >= endDate)
            {
                MessageBox.Show("Дата заезда должна быть раньше даты выезда.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Бронируем комнату
            selectedRoom.StatusRoom = Status.Pending; // Устанавливаем статус "Забронирован"
            selectedRoom.CheckInDate = startDate;   // Сохраняем дату заезда
            selectedRoom.CheckOutDate = endDate;    // Сохраняем дату выезда

            // Обновляем коллекцию данных в ViewModel
            var viewModel = DataContext as ViewModel;
            if (viewModel != null)
            {
                viewModel.Rooms = new ObservableCollection<Room>(viewModel.Rooms); // Принудительное обновление коллекции
            }

            MessageBox.Show($"Комната {selectedRoom.NumberRoom} успешно забронирована.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
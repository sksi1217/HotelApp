using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using HotelApp.Helpers.Animations;
using HotelApp.Models;
using HotelApp.ViewModels;
using HotelApp.Views.Groups.WorkPanel.Concierge;
using HotelApp.Views.UserControl;

namespace HotelApp.Views.Groups.WorkPanel
{
    /// <summary>
    /// Логика взаимодействия для ConciergeWorkPanel.xaml
    /// </summary>
    public partial class ConciergeWorkPanel : System.Windows.Controls.UserControl
    {
        private bool isWindowOpen = false;
        public event EventHandler Closed;

        public Room SelectedRoom { get; set; }

        public ConciergeWorkPanel()
        {
            InitializeComponent();
            DataContext = this;

            LoadedRoom();
        }

        private void LoadedRoom()
        {
            // Инициализация ViewModel
            var viewModel = new ViewModel();

            // Загрузка данных из базы данных
            viewModel.LoadRoomsFromDatabase();

            // Привязываем отфильтрованный список к ItemsSource
            RoomsDataGrid.ItemsSource = viewModel.Rooms;
        }

        private void CreateBooking_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRoom.StatusRoom != Status.Free)
            {
                MessageBox.Show("Эта комната уже забронированая или что-то другое!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedRoom == null)
            {
                MessageBox.Show("Сначало выберите комнату!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (isWindowOpen) return;

            var window = new BookingConciergeControl(SelectedRoom)
            {
                Width = 500,
                Height = 400,
                Opacity = 0
            };

            WindowContainer.Children.Add(window);

            // Явно задаем начальные координаты
            Canvas.SetLeft(window, (WindowContainer.ActualWidth - window.Width) / 2);
            Canvas.SetTop(window, (WindowContainer.ActualHeight - window.Height) / 2);

            Animations.AnimateWindowAppear(window);

            isWindowOpen = true;

            window.Closed += (s, args) =>
            {
                isWindowOpen = false;
            };
        }

        private void EditBooking_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRoom == null)
            {
                MessageBox.Show("Сначало выберите комнату!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (isWindowOpen) return;

            var window = new EditingControl(SelectedRoom)
            {
                Width = 400,
                Height = 200,
                Opacity = 0
            };

            WindowContainer.Children.Add(window);

            // Явно задаем начальные координаты
            Canvas.SetLeft(window, (WindowContainer.ActualWidth - window.Width) / 2);
            Canvas.SetTop(window, (WindowContainer.ActualHeight - window.Height) / 2);

            Animations.AnimateWindowAppear(window);

            isWindowOpen = true;

            window.Closed += (s, args) =>
            {
                isWindowOpen = false;
            };
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

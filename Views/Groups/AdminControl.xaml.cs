using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HotelApp.Helpers.Animations;
using HotelApp.Models;
using HotelApp.ViewModels;
using HotelApp.Views.UserControl;

namespace HotelApp.Views.Groups
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminControl : System.Windows.Controls.UserControl
    {

        public event EventHandler Closed;
        private readonly ViewModel _viewModel = new ViewModel();

        private bool isWindowOpen = false; // Флаг для отслеживания открытого окна
        public AdminControl()
        {
            InitializeComponent();
            DataContext = _viewModel;

            LoadedRoom();
        }

        private void LoadedRoom()
        {
            try
            {
                // Проверка, есть ли данные в таблице Rooms
                if (DatabaseHelper.IsRoomsTableEmpty())
                {
                    // Загрузка данных из Excel
                    var rooms = ExcelLoader.LoadRoomsFromExcel();

                    // Сохранение данных в базу данных
                    DatabaseHelper.SaveRoomsToDatabase(rooms);

                    // Преобразование List<Room> в ObservableCollection<Room>
                    _viewModel.Rooms = new ObservableCollection<Room>(rooms);
                }
                else
                {
                    // Если данные уже есть, загружаем их из базы данных
                    _viewModel.Rooms = new ObservableCollection<Room>(DatabaseHelper.LoadRoomsFromDatabase());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Инициализация ViewModel
            var viewModel = new ViewModel();

            // Загрузка данных из базы данных
            viewModel.LoadRoomsFromDatabase();

            // Привязываем отфильтрованный список к ItemsSource
            RoomsDataGrid.ItemsSource = viewModel.Rooms;
        }

        private void ShowDataGrid(DataGrid dataGridToShow)
        {
            RoomsDataGrid.Visibility = Visibility.Collapsed;
            UserDataGrid.Visibility = Visibility.Collapsed;
            WorkersDataGrid.Visibility = Visibility.Collapsed;

            if (dataGridToShow != null)
            {
                dataGridToShow.Visibility = Visibility.Visible;
            }
        }

        private void AllRooms_Click(object sender, RoutedEventArgs e)
        {
            ShowDataGrid(RoomsDataGrid);
            // Инициализация ViewModel
            var viewModel = new ViewModel();

            // Загрузка данных из базы данных
            viewModel.LoadRoomsFromDatabase();

            // Привязываем отфильтрованный список к ItemsSource
            RoomsDataGrid.ItemsSource = viewModel.Rooms;
        }

        private void AllWorkers_Click(object sender, RoutedEventArgs e)
        {
            ShowDataGrid(WorkersDataGrid);
            // Инициализация ViewModel
            var viewModel = new ViewModel();

            // Загрузка данных из базы данных
            viewModel.LoadWorkerFromDatabase();

            // Привязываем отфильтрованный список к ItemsSource
            WorkersDataGrid.ItemsSource = viewModel.Workers;
        }

        private void AllUsers_Click(object sender, RoutedEventArgs e)
        {
            ShowDataGrid(UserDataGrid);
            // Инициализация ViewModel
            var viewModel = new ViewModel();

            // Загрузка данных из базы данных
            viewModel.LoadUsersFromDatabase();

            // Привязываем отфильтрованный список к ItemsSource
            UserDataGrid.ItemsSource = viewModel.Users;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (isWindowOpen) return;

            var loginWindow = new LoginControl()
            {
                Width = 400,
                Height = 450,
                Opacity = 0
            };

            WindowContainer.Children.Add(loginWindow);

            // Явно задаем начальные координаты
            Canvas.SetLeft(loginWindow, (WindowContainer.ActualWidth - loginWindow.Width) / 2);
            Canvas.SetTop(loginWindow, (WindowContainer.ActualHeight - loginWindow.Height) / 2);

            Animations.AnimateWindowAppear(loginWindow);

            isWindowOpen = true;

            loginWindow.Closed += (s, args) =>
            {
                isWindowOpen = false;
            };
        }

        private void BookRoom_Click(object sender, RoutedEventArgs e)
        {
            if (isWindowOpen) return;

            var gameWindow = new BookRoomWindow()
            {
                Width = 700,
                Height = 550,
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

        private void PriceRooms_Click(object sender, RoutedEventArgs e)
        {
            if (isWindowOpen) return;

            var gameWindow = new SpecifyPriceControl()
            {
                Width = 600,
                Height = 500,
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

        private void RegisterWorker_Click(object sender, RoutedEventArgs e)
        {
            if (isWindowOpen) return;

            var gameWindow = new RegistrationWork()
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

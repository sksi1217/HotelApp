using HotelApp.Models;
using System.Collections.ObjectModel;

namespace HotelApp.ViewModels
{
    public class ViewModel : NotifyPropertyChanged
    {
        private ObservableCollection<Room> _rooms = new ObservableCollection<Room>();
        
        public ObservableCollection<Room> Rooms
        {
            get => _rooms;
            set => SetField(ref _rooms, value);
        }

        private ObservableCollection<User> _users = new ObservableCollection<User>();

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetField(ref _users, value);
        }

        private ObservableCollection<Worker> _workers = new ObservableCollection<Worker>();

        public ObservableCollection<Worker> Workers
        {
            get => _workers;
            set => SetField(ref _workers, value);
        }

        // Метод для загрузки данных комнаты из базы данных
        public void LoadRoomsFromDatabase()
        {
            // Загрузка данных из базы данных
            var rooms = DatabaseHelper.LoadRoomsFromDatabase();

            // Преобразование данных в коллекцию Room
            var updatedRooms = new ObservableCollection<Room>();
            foreach (var room in rooms)
            {
                updatedRooms.Add(new Room
                {
                    Id = room.Id,
                    FirstName = room.FirstName,
                    LastName = room.LastName,
                    Email = room.Email,
                    Phone = room.Phone,

                    NameRoom = room.NameRoom,
                    DescriptionRoom = room.DescriptionRoom,
                    PriceRoom = room.PriceRoom,
                    NumberRoom = room.NumberRoom,
                    FloorRoom = room.FloorRoom,
                    TypeRoom = room.TypeRoom,
                    StatusRoom = room.StatusRoom,
                    Client = room.Client,

                    Payment = room.Payment,
                    IsPaid = room.IsPaid,
                    TotalAmount = room.TotalAmount,

                    CheckInDate = room.CheckInDate,
                    CheckOutDate = room.CheckOutDate,

                    GuestCount = room.GuestCount
                });
            }

            // Обновление коллекции Rooms
            Rooms = updatedRooms;
        }

        // Метод для загрузки user из базы данных
        public void LoadUsersFromDatabase()
        {
            // Загрузка данных из базы данных
            var users = DatabaseHelper.LoadUserFromDatabase();

            // Преобразование данных в коллекцию Room
            var updatedUsers = new ObservableCollection<User>();
            foreach (var user in users)
            {
                updatedUsers.Add(new User
                {
                    Id = user.Id,
                    Login = user.Login,
                    Password = user.Password,
                    Salt = user.Salt,
                    TypeUser = user.TypeUser
                });
            }

            // Обновление коллекции Users
            Users = updatedUsers;
        }

        // Метод для загрузки рабочего из базы данных
        public void LoadWorkerFromDatabase()
        {
            // Загрузка данных из базы данных
            var workers = DatabaseHelper.LoadWorkerFromDatabase();

            // Преобразование данных в коллекцию Room
            var updatedWorkers = new ObservableCollection<Worker>();
            foreach (var worker in workers)
            {
                updatedWorkers.Add(new Worker
                {
                    Id = worker.Id,
                    FirstName = worker.FirstName,
                    LastName = worker.LastName,
                    Phone = worker.Phone,
                    TypeWorker = worker.TypeWorker,


                });
            }

            // Обновление коллекции Workers
            Workers = updatedWorkers;
        }
    }
}
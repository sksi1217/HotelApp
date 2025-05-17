using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using HotelApp.Models;

namespace HotelApp
{
    public static class DatabaseHelper
    {
        #region Пути и строка подключения

        // Путь к файлу базы данных в папке приложения
        private static string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hotel.db");

        // Строка подключения к SQLite-базе данных
        private static string ConnectionString = $"Data Source={DbPath};Version=3;";

        #endregion

        public static bool CheckAdminExists()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM Users WHERE TypeUser = @type;";
                command.Parameters.AddWithValue("@type", (int)User.UserType.Administrator);

                long count = (long)command.ExecuteScalar();
                return count > 0;
            }
        }
        public static void AddUser(User user)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO Users (Login, Password, Cookie, Salt, TypeUser, TypeWorkForWorker) VALUES (@Login, @Password, @Cookie, @Salt, @TypeUser, @TypeWorkForWorker)";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Login", user.Login);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Cookie", user.Cookie);
                    command.Parameters.AddWithValue("@Salt", user.Salt);
                    command.Parameters.AddWithValue("@TypeUser", (int)user.TypeUser);
                    command.Parameters.AddWithValue("@TypeUser", (int)user.TypeUser);
                    command.Parameters.AddWithValue("@TypeWorkForWorker", (int)user.TypeWorkForWorker);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void AddWorker(Worker worker)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO Workers (FirstName, LastName, Phone, TypeWorker) VALUES (@FirstName, @LastName, @Phone, @TypeWorker)";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", worker.FirstName);
                    command.Parameters.AddWithValue("@LastName", worker.LastName);
                    command.Parameters.AddWithValue("@Phone", worker.Phone);
                    command.Parameters.AddWithValue("@TypeWorker", (int)worker.TypeWorker);
                    command.ExecuteNonQuery();
                }
            }
        }


        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string selectQuery = "SELECT Id, Login, Password, Cookie, Salt, TypeUser, TypeWorkForWorker FROM Users";
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                Id = reader.GetInt32(0),
                                Login = reader.GetString(1),
                                Password = reader.GetString(2),
                                Cookie = !reader.IsDBNull(3) ? reader.GetString(3) : null,
                                Salt = (byte[])reader["Salt"],
                                TypeUser = (User.UserType)reader.GetInt32(5),
                                TypeWorkForWorker = (User.WorkForWorkerType)reader.GetInt32(6)
                            });
                        }
                    }
                }
                return users;
            }
        }

        #region Инициализация БД (создание файла и таблицы)

        /// <summary>
        /// Инициализация базы данных: создание файла БД и таблиц, если они не существуют.
        /// </summary>
        public static void InitializeDatabase()
        {
            try
            {
                // Проверка существования файла базы данных
                if (!File.Exists(DbPath))
                {
                    Console.WriteLine("База данных не найдена. Создаём новый файл...");

                    // Создание файла базы данных
                    SQLiteConnection.CreateFile(DbPath);
                    Console.WriteLine($"Файл базы данных создан: {DbPath}");
                }

                // Открываем соединение с базой данных
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine("Подключение к базе данных установлено.");

                    // 1. Создание таблицы пользователей
                    string createUsersTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Login TEXT NOT NULL UNIQUE,
                            Password TEXT NOT NULL,
                            Cookie TEXT,
                            Salt BLOB NOT NULL,
                            TypeUser INTEGER NOT NULL,
                            TypeWorkForWorker INTEGER NOT NULL
                        )";

                    ExecuteNonQuery(connection, createUsersTableQuery, "Таблица Users создана или уже существует.");

                    // 2. Создание таблицы номеров
                    string createRoomsTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Rooms (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            NameRoom TEXT,
                            DescriptionRoom TEXT,
                            PriceRoom REAL,
                            NumberRoom INTEGER,
                            FloorRoom TEXT,
                            TypeRoom INTEGER,
                            StatusRoom INTEGER,
                            Client TEXT,
                            FirstName TEXT,
                            LastName TEXT,
                            Email TEXT,
                            Phone TEXT,
                            PaymentMethod INTEGER,
                            IsPaid INTEGER,
                            TotalAmount REAL,
                            CheckInDate TEXT,
                            CheckOutDate TEXT,
                            GuestCount INTEGER
                        )";

                    ExecuteNonQuery(connection, createRoomsTableQuery, "Таблица Rooms создана или уже существует.");

                    string createClientsTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Clients (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            FirstName TEXT,
                            LastName TEXT,
                            Email TEXT,
                            Phone TEXT,
                            FloorRoom TEXT,
                            NumberRoom INTEGER
                        )";

                    ExecuteNonQuery(connection, createClientsTableQuery, "Таблица Clients создана или уже существует.");

                    string createWorkerTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Workers (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            FirstName TEXT,
                            LastName TEXT,
                            Phone TEXT,
                            TypeWorker INTEGER
                        )";

                    ExecuteNonQuery(connection, createWorkerTableQuery, "Таблица Workers создана или уже существует.");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при инициализации базы данных: {ex.Message}");
                throw; // Переброс исключения для дальнейшей обработки
            }
        }

        /// <summary>
        /// Выполнение SQL-запроса без возврата результата.
        /// </summary>
        /// <param name="connection">Открытое соединение с базой данных.</param>
        /// <param name="query">SQL-запрос.</param>
        /// <param name="successMessage">Сообщение о успешном выполнении запроса.</param>
        private static void ExecuteNonQuery(SQLiteConnection connection, string query, string successMessage)
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine(successMessage);
            }
        }

        #endregion

        #region Загрузка комнаты из БД

        /// <summary>
        /// Загружает список комнат (номеров отеля) из базы данных.
        /// </summary>
        /// <returns>Список объектов Room</returns>
        public static ObservableCollection<Room> LoadRoomsFromDatabase()
        {
            var rooms = new ObservableCollection<Room>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // SQL-запрос для получения всех записей из таблицы Rooms
                string query = "SELECT * FROM Rooms";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rooms.Add(new Room
                        {
                            Id = reader["Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Id"]),
                            NameRoom = reader["NameRoom"] as string,
                            DescriptionRoom = reader["DescriptionRoom"] as string,
                            PriceRoom = reader["PriceRoom"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["PriceRoom"]),
                            NumberRoom = reader["NumberRoom"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NumberRoom"]),
                            FloorRoom = reader["FloorRoom"] as string,
                            TypeRoom = reader["TypeRoom"] == DBNull.Value
                                ? default(RoomType)
                                : (RoomType)Convert.ToInt32(reader["TypeRoom"]),
                            StatusRoom = reader["StatusRoom"] == DBNull.Value
                                ? default(Status)
                                : (Status)Convert.ToInt32(reader["StatusRoom"]),
                            Client = reader["Client"] as string,
                            FirstName = reader["FirstName"] as string,
                            LastName = reader["LastName"] as string,
                            Email = reader["Email"] as string,
                            Phone = reader["Phone"] as string,
                            Payment = reader["PaymentMethod"] == DBNull.Value
                                ? default(PaymentMethod)
                                : (PaymentMethod)Convert.ToInt32(reader["PaymentMethod"]),
                            IsPaid = reader["IsPaid"] == DBNull.Value
                                ? false
                                : Convert.ToInt32(reader["IsPaid"]) == 1,
                            TotalAmount = reader["TotalAmount"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["TotalAmount"]),
                            CheckInDate = reader.IsDBNull(reader.GetOrdinal("CheckInDate"))
                                ? null
                                : (DateTime?)DateTime.Parse(reader["CheckInDate"].ToString()),
                            CheckOutDate = reader.IsDBNull(reader.GetOrdinal("CheckOutDate"))
                                ? null
                                : (DateTime?)DateTime.Parse(reader["CheckOutDate"].ToString()),
                            GuestCount = reader["GuestCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GuestCount"])
                        });
                    }
                }
            }

            return rooms;
        }

        /// <summary>
        /// Загружает список пользователей из базы данных.
        /// </summary>
        /// <returns>Список объектов User</returns>
        public static ObservableCollection<User> LoadUserFromDatabase()
        {
            var users = new ObservableCollection<User>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Users";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader["Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Id"]),
                            Login = reader["Login"] as string,
                            Password = reader["Password"] as string,
                            Cookie = reader["Cookie"] as string,
                            Salt = reader["Salt"] is byte[] salt ? salt : null,
                            TypeUser = reader["TypeUser"] == DBNull.Value
                                ? default(User.UserType)
                                : (User.UserType)Convert.ToInt32(reader["TypeUser"]),
                            TypeWorkForWorker = reader["TypeWorkForWorker"] == DBNull.Value
                                ? default(User.WorkForWorkerType)
                                : (User.WorkForWorkerType)Convert.ToInt32(reader["TypeWorkForWorker"])
                        });
                    }
                }
            }

            return users;
        }

        /// <summary>
        /// Загружает список работников отеля из базы данных.
        /// </summary>
        /// <returns>Список объектов Worker</returns>
        public static ObservableCollection<Worker> LoadWorkerFromDatabase()
        {
            var workers = new ObservableCollection<Worker>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Workers";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        workers.Add(new Worker
                        {
                            Id = reader["Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Id"]),
                            FirstName = reader["FirstName"] as string,
                            LastName = reader["LastName"] as string,
                            Phone = reader["Phone"] as string,
                            TypeWorker = reader["TypeWorker"] == DBNull.Value
                                ? default(Worker.WorkerType)
                                : (Worker.WorkerType)Convert.ToInt32(reader["TypeWorker"])
                        });
                    }
                }
            }

            return workers;
        }
        #endregion


        /// <summary>
        /// Проверяет, пуста ли таблица Rooms.
        /// </summary>
        /// <returns>True, если таблица пустая; иначе False.</returns>
        public static bool IsRoomsTableEmpty()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // SQL-запрос для подсчета строк в таблице Rooms
                string query = "SELECT COUNT(*) FROM Rooms";

                using (var command = new SQLiteCommand(query, connection))
                {
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count == 0;
                }
            }
        }

        #region Сохранение гостей в БД

        /// <summary>
        /// Сохраняет список гостей (номеров) в базу данных.
        /// ВНИМАНИЕ: При повторном вызове данные будут дублироваться!
        /// Для корректной работы лучше использовать UPDATE или UPSERT.
        /// </summary>
        /// <param name="rooms">Список гостей</param>
        public static void SaveRoomsToDatabase(List<Room> rooms)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                foreach (var guest in rooms)
                {
                    // SQL-запрос на добавление новой записи
                    string insertQuery = @"
                        INSERT INTO Rooms (NameRoom, FloorRoom, NumberRoom, TypeRoom, Client, StatusRoom, CheckInDate, CheckOutDate)
                        VALUES (@nameroom, @floor, @number, @category, @client, @status, @checkin, @checkout)";

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        // Привязываем параметры, чтобы избежать SQL-инъекций
                        command.Parameters.AddWithValue("@nameroom", guest.NameRoom);
                        command.Parameters.AddWithValue("@floor", guest.FloorRoom);
                        command.Parameters.AddWithValue("@number", guest.NumberRoom);
                        command.Parameters.AddWithValue("@category", guest.TypeRoom);
                        command.Parameters.AddWithValue("@client", guest.Client);
                        command.Parameters.AddWithValue("@status", guest.StatusRoom);
                        command.Parameters.AddWithValue("@checkin", guest.CheckInDate);
                        command.Parameters.AddWithValue("@checkout", guest.CheckOutDate);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion


        public static User GetUserByLogin(string login)
        {
            using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                connection.Open();
                string selectQuery = "SELECT Id, Login, Password, Cookie, Salt, TypeUser, TypeWorkForWorker " +
                                     "FROM Users WHERE Login = @Login";
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string dbLogin = reader.GetString(reader.GetOrdinal("Login"));
                            string password = reader.GetString(reader.GetOrdinal("Password"));
                            object cookieObj = reader["Cookie"];
                            string cookie = cookieObj is DBNull ? null : cookieObj as string;
                            byte[] salt = reader["Salt"] as byte[];
                            User.UserType typeUser = (User.UserType)reader.GetInt32(reader.GetOrdinal("TypeUser"));
                            User.WorkForWorkerType workType = (User.WorkForWorkerType)reader.GetInt32(reader.GetOrdinal("TypeWorkForWorker"));

                            return new User
                            {
                                Id = id,
                                Login = dbLogin,
                                Password = password,
                                Cookie = cookie,
                                Salt = salt,
                                TypeUser = typeUser,
                                TypeWorkForWorker = workType
                            };
                        }
                        return null; // Пользователь не найден
                    }
                }
            }
        }

        public static void UpdateRoomInDatabase(Room room)
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
                {
                    connection.Open();

                    string query = @"
                        UPDATE Rooms SET 
                            FirstName = @FirstName,
                            LastName = @LastName,
                            Email = @Email,
                            Phone = @Phone,
                            CheckInDate = @CheckIn,
                            CheckOutDate = @CheckOut,
                            GuestCount = @GuestCount,
                            StatusRoom = @StatusRoom,
                            Client = @Client,
                            TotalAmount = @TotalAmount,
                            IsPaid = @IsPaid,
                            PaymentMethod = @PaymentMethod
                        WHERE Id = @Id";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        // Обязательные поля
                        command.Parameters.AddWithValue("@FirstName", room.FirstName);
                        command.Parameters.AddWithValue("@LastName", room.LastName);
                        command.Parameters.AddWithValue("@Email", room.Email);
                        command.Parameters.AddWithValue("@Phone", room.Phone);

                        // Nullable даты
                        command.Parameters.AddWithValue(
                            "@CheckIn",
                            room.CheckInDate.HasValue
                                ? (object)room.CheckInDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                                : DBNull.Value);

                        command.Parameters.AddWithValue(
                            "@CheckOut",
                            room.CheckOutDate.HasValue
                                ? (object)room.CheckOutDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                                : DBNull.Value);

                        // Дополнительные поля
                        command.Parameters.AddWithValue("@GuestCount", room.GuestCount);
                        command.Parameters.AddWithValue("@StatusRoom", (int)room.StatusRoom);
                        command.Parameters.AddWithValue("@Client", $"{room.FirstName} {room.LastName}");
                        command.Parameters.AddWithValue("@TotalAmount", room.TotalAmount);
                        command.Parameters.AddWithValue("@IsPaid", room.IsPaid ? 1 : 0);
                        command.Parameters.AddWithValue("@PaymentMethod", (int)room.Payment);

                        // Условие WHERE — по ID
                        command.Parameters.AddWithValue("@Id", room.Id);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            MessageBox.Show("Не удалось обновить запись: комната не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void UpdatePriceRoomInDatabase(Room room)
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
                {
                    connection.Open();
                    string updateQuery = "UPDATE Rooms SET PriceRoom = @Price WHERE Id = @Id;";

                    using (var command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Price", room.PriceRoom);
                        command.Parameters.AddWithValue("@Id", room.Id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}");
            }
        }

        public static void UpdateCookieUserInDatabase(User user)
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
                {
                    connection.Open();
                    string updateQuery = "UPDATE Users SET Cookie = @Cookie WHERE Id = @Id;";

                    using (var command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Cookie", user.Cookie);
                        command.Parameters.AddWithValue("@Id", user.Id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}");
            }
        }
    }
}
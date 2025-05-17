using System;
using System.ComponentModel;

namespace HotelApp.Models
{
    public class Room
    {
        public int Id { get; set; }

        // Личная информация
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string NameRoom { get; set; } = "";
        public string DescriptionRoom { get; set; } = "";
        public decimal PriceRoom { get; set; }
        public int NumberRoom { get; set; } // Номер комнат
        public string FloorRoom { get; set; } // Этаж комнаты        
        public RoomType TypeRoom { get; set; } // Тип комнаты
        public Status StatusRoom { get; set; } = Status.Free;
        public string Client { get; set; } = "";

        // Оплата
        public PaymentMethod Payment { get; set; }
        public bool IsPaid { get; set; }
        public decimal TotalAmount { get; set; }

        // Информация о бронировании
        public DateTime? CheckInDate { get; set; } = null; // Добавлено свойство CheckInDate
        public DateTime? CheckOutDate { get; set; } = null; // Добавлено свойство CheckOutDate
        public int GuestCount { get; set; } // Сколько людей
    }


    public enum PaymentMethod
    {
        CreditCard, // Кредитная карта
        Cash, // Наличные
        OnlineBanking, // Онлайн банк
    }
    public enum RoomType
    {
        [Description("Одноместный стандарт")]
        Single,

        [Description("Одноместный эконом")]
        SingleBudget,

        [Description("Стандарт двухместный с 2 раздельными кроватями")]
        DoubleStandard,

        [Description("Эконом двухместный с 2 раздельными кроватями")]
        DoubleBudget,

        [Description("Двухкомнатный двухместный стандарт с 1 или 2 кроватями")]
        TwoRoomDoubleStandard,

        [Description("3-местный Эконом")]
        ThreeBudget,

        [Description("Бизнес с 1 или 2 кроватями")]
        Business,

        [Description("Студия")]
        Atelier,

        [Description("Люкс с 2 двуспальными кроватями")]
        Suite
    }

    public enum Status
    {
        // 1 что может быть
        Confirmed,    // Подтверждённый
        Pending,      // Ожидаемый // если через 1 час до клиета не дозваниваются то отменяем бронирования
        Cancelled,    // Отменённый

        // 2 что может быть
        Completed,     // Завершённый

        // 3 что может быть
        Cleaning, // Назначен к уборке

        // 4 что может быть
        Free, // Свободный
        Busy, // Занятый
        Dirty, // Грязный
        Clean, // Чистый
    }

}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using HotelApp.Models;

namespace HotelApp
{
    public static class ExcelLoader
    {
        private static readonly Dictionary<string, RoomType> RoomTypeMapping = new Dictionary<string, RoomType>(StringComparer.OrdinalIgnoreCase)
        {
            // Одноместные
            ["Одноместный стандарт"] = RoomType.Single,
            ["одноместный стандарт"] = RoomType.Single,

            ["Одноместный эконом"] = RoomType.SingleBudget,
            ["эконом одноместный"] = RoomType.SingleBudget,

            // Двухместные
            ["Стандарт двухместный с 2 раздельными кроватями"] = RoomType.DoubleStandard,
            ["двухместный стандарт с 2 кроватями"] = RoomType.DoubleStandard,

            ["Эконом двухместный с 2 раздельными кроватями"] = RoomType.DoubleBudget,
            ["двухместный эконом с 2 кроватями"] = RoomType.DoubleBudget,

            // Трехместные
            ["3-местный бюджет"] = RoomType.ThreeBudget,
            ["трехместный эконом"] = RoomType.ThreeBudget,

            // Бизнес
            ["Бизнес с 1 или 2 кроватями"] = RoomType.Business,
            ["бизнес класс"] = RoomType.Business,

            // Двухкомнатные
            ["Двухкомнатный двухместный стандарт с 1 или 2 кроватями"] = RoomType.TwoRoomDoubleStandard,
            ["двухкомнатный стандарт"] = RoomType.TwoRoomDoubleStandard,

            // Студии
            ["Студия"] = RoomType.Atelier,
            ["апартаменты студийного типа"] = RoomType.Atelier,

            // Люкс
            ["Люкс с 2 двуспальными кроватями"] = RoomType.Suite,
            ["люкс"] = RoomType.Suite,
        };

        public static RoomType? FindMatchingRoomType(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Попробовать точное совпадение
            if (RoomTypeMapping.TryGetValue(input.Trim(), out var exactMatch))
                return exactMatch;

            // Привести к нижнему регистру для сравнения
            var normalizedInput = input.ToLower();

            // Проверить на частичное совпадение
            foreach (var kvp in RoomTypeMapping)
            {
                if (kvp.Key.ToLower().Contains(normalizedInput) || normalizedInput.Contains(kvp.Key.ToLower()))
                {
                    return kvp.Value;
                }
            }

            return null;
        }

        public static List<Room> LoadRoomsFromExcel(string filePath = null)
        {
            if (filePath == null)
            {
                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rooms.xlsx");

                if (!File.Exists(filePath))
                    throw new FileNotFoundException("Файл Excel не найден.", filePath);
            }

            var rooms = new List<Room>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed();

                foreach (var row in rows.Skip(1)) // Пропуск заголовка
                {
                    var floor = row.Cell(1).Value.ToString();
                    var numberStr = row.Cell(2).GetString();
                    var categoryStr = row.Cell(3).Value.ToString();

                    if (int.TryParse(numberStr, out int roomNumber))
                    {
                        // Найти ближайшее соответствие типу комнаты
                        var roomTypeOpt = FindMatchingRoomType(categoryStr);

                        if (roomTypeOpt.HasValue)
                        {
                            var roomType = roomTypeOpt.Value;
                            var roomName = EnumHelper.GetDescription(roomType); // Получаем описание

                            rooms.Add(new Room
                            {
                                NameRoom = roomName,
                                FloorRoom = floor,
                                NumberRoom = roomNumber,
                                TypeRoom = roomType
                                // Client, Status, CheckInDate, CheckOutDate остаются пустыми
                            });

                            Debug.WriteLine(roomName);
                        }
                        else
                        {
                            Console.WriteLine($"Не удалось определить тип комнаты для строки: {categoryStr}");
                        }
                    }
                }
            }

            return rooms;
        }
    }

    public static class EnumHelper
    {
        public static string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(
                field,
                typeof(DescriptionAttribute)) ?? throw new InvalidOperationException();

            return attribute.Description;
        }
    }
}
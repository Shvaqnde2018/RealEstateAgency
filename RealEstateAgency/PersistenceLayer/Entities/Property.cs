using System;
using System.Net.NetworkInformation;
using System.Collections.Generic;

namespace RealEstateAgency.PersistenceLayer.Entities
{
    public class Property
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = string.Empty;
        public double Area { get; set; }
        public string Address { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Landmark { get; set; } = string.Empty;
        public int Rooms { get; set; }
        public int YearBuilt { get; set; } = DateTime.Now.Year;
        public decimal Price { get; set; }
        public PropertyStatus Status { get; set; }
        public List<StatusHistoryEntry> StatusHistory { get; set; } = new List<StatusHistoryEntry>();
        public List<PriceHistoryEntry> PriceHistory { get; set; } = new List<PriceHistoryEntry>();

        public Property() { }

        public Property(string type, double area, string address, string district, string landmark, int rooms, decimal price, PropertyStatus status, int yearBuilt)
        {
            Validate(type, area, address, district, landmark, rooms, price, status, yearBuilt);
            Type = type;
            Area = area;
            Address = address;
            District = district;
            Landmark = landmark;
            Rooms = rooms;
            Price = price;
            Status = status;
            YearBuilt = yearBuilt;

            StatusHistory.Add(new StatusHistoryEntry
            {
                OldStatus = status,
                NewStatus = status,
                Comment = "Початковий статус при створенні об'єкта"
            });

            PriceHistory.Add(new PriceHistoryEntry
            {
                OldPrice = price,
                NewPrice = price,
                Comment = "Початкова ціна при створенні об'єкта"
            });
        }

        public void Update(string type, double area, string address, string district, string landmark, int rooms, decimal newPrice, PropertyStatus newStatus, int yearBuilt)
        {
            Validate(type, area, address, district, landmark, rooms, newPrice, newStatus, yearBuilt);

            if(Price != newPrice)
            {
                PriceHistory.Add(new PriceHistoryEntry
                {
                    ChangedAt = DateTime.Now,
                    OldPrice = Price,
                    NewPrice = newPrice,
                    Comment = $"Зміна ціни з ${Price} на ${newPrice}"
                });
            }

            if(Status != newStatus)
            {
                ChangeStatus(newStatus, $"Статус змінено з {Status} на {newStatus}");
            }

            Type = type;
            Area = area;
            Address = address;
            District = district;
            Landmark = landmark;
            Rooms = rooms;
            YearBuilt = yearBuilt;
            Price = newPrice;
            Status = newStatus;
        }

        public void ChangeStatus(PropertyStatus newStatus, string comment = "Зміна статусу", bool allowFromSold = false)
        {
            if(!Enum.IsDefined(typeof(PropertyStatus), newStatus))
                throw new ArgumentException("Некоректний статус нерухомості.");

            if (Status == newStatus)
                return;

            if(Status == PropertyStatus.Sold && !allowFromSold)
            {
                if(!allowFromSold)
                    throw new InvalidOperationException("Неможливо змінити статус нерухомості, яка вже продана (спочатку розірвіть угоду).");
            }

            StatusHistory.Add(new StatusHistoryEntry
            {
                ChangedAt = DateTime.Now,
                OldStatus = Status,
                NewStatus = newStatus,
                Comment = comment
            });

            Status = newStatus;
        }

        public decimal GetPricePerSquareMeter() => Area > 0 ? Price / (decimal)Area : 0m;

        private void Validate(string type, double area, string address, string district, string Landmark, int rooms, decimal price, PropertyStatus status, int yearBuilt)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Тип нерухомості не може бути порожнім.");
            if (area <= 0)
                throw new ArgumentException("Площа нерухомості повинна бути більшою за нуль.");
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Адреса нерухомості не може бути порожньою.");
            if (string.IsNullOrWhiteSpace(district))
                throw new ArgumentException("Район не може бути порожнім.");
            if (string.IsNullOrWhiteSpace(Landmark))
                throw new ArgumentException("Орієнтир не може бути порожнім.");
            if (rooms <= 0)
                throw new ArgumentException("Кількість кімнат повинна бути більшою за нуль.");
            if(yearBuilt < 1800 || yearBuilt > DateTime.Now.Year)
                throw new ArgumentException("Рік побудови повинен бути в межах від 1800 до поточного року.");
            if (price <= 0)
                throw new ArgumentException("Ціна нерухомості повинна бути більшою за нуль.");
            if (!Enum.IsDefined(typeof(PropertyStatus), status))
                throw new ArgumentException("Некоректний статус нерухомості.");
        }

        public override string ToString()
        {
            return $"[{Id.ToString().Substring(0, 8)}] {Type} ({Rooms} кімн., {Area} м², {YearBuilt} р.) | Район: {District} | Адреса: {Address} | Орієнтири: {Landmark} | ${Price} (${GetPricePerSquareMeter():F0}/м²) | Статус: {Status}";
        }
    }
}
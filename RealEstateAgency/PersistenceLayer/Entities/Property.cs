using System;

namespace RealEstateAgency.PersistenceLayer.Entities
{
    public class Property
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = string.Empty;
        public double Area { get; set; }
        public string Address { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public PropertyStatus Status { get; set; }

        public Property() { }

        public Property(string type, double area, string address, decimal price, PropertyStatus status)
        {
            Validate(type, area, address, price);
            Type = type;
            Area = area;
            Address = address;
            Price = price;
            Status = status;
        }

        public void Update(string type, double area, string address, decimal price, PropertyStatus status)
        {
            Validate(type, area, address, price);
            Type = type;
            Area = area;
            Address = address;
            Price = price;
            Status = status;
        }

        private void Validate(string type, double area, string address, decimal price)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Тип нерухомості не може бути порожнім.");
            if (area <= 0)
                throw new ArgumentException("Площа нерухомості повинна бути більшою за нуль.");
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Адреса нерухомості не може бути порожньою.");
            if (price <= 0)
                throw new ArgumentException("Ціна нерухомості повинна бути більшою за нуль.");
        }

        public override string ToString()
        {
            return $"[{Id.ToString().Substring(0, 8)}] {Type} | {Area} м² | {Address} | ${Price} | Статус: {Status}";
        }
    }
}
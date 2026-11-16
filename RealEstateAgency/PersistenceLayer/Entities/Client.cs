using System;

namespace RealEstateAgency.PersistenceLayer.Entities
{
    public class Client
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public ClientType Type { get; set; }

        public Client() { }

        public Client(string name, string contactInfo, ClientType type)
        {
            Validate(name, contactInfo, type);
            Name = name;
            ContactInfo = contactInfo;
            Type = type;
        }

        public void Update(string name, string contactInfo, ClientType type)
        {
            Validate(name, contactInfo, type);
            Name = name;
            ContactInfo = contactInfo;
            Type = type;
        }

        private void Validate(string name, string contactInfo, ClientType type)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ім'я клієнта не може бути порожнім.");
            if (string.IsNullOrWhiteSpace(contactInfo))
                throw new ArgumentException("Контактна інформація клієнта не може бути порожньою.");
            if (!Enum.IsDefined(typeof(ClientType), type))
                throw new ArgumentException("Некоректний тип клієнта.");
        }

        public override string ToString()
        {
            return $"[{Id.ToString().Substring(0, 8)}] {Name} | Контакти: {ContactInfo} | Тип: {Type}";
        }
    }
}
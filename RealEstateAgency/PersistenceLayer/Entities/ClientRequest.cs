using System;

namespace RealEstateAgency.PersistenceLayer.Entities
{
    public class ClientRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ClientId { get; set; }
        public string Title { get; set; } = string.Empty;
        public PropertySearchFilter Filter { get; set; } = new PropertySearchFilter();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public ClientRequest() { }

        public ClientRequest(Guid clientId, string title, PropertySearchFilter filter)
        {
            if(clientId == Guid.Empty)
                if (clientId == Guid.Empty) throw new ArgumentException("Некоректний ID клієнта.");

            if (string.IsNullOrWhiteSpace(title))
                if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Назва запиту не може бути порожньою.");

            ClientId = clientId;
            Title = title;
            Filter = filter ?? new PropertySearchFilter();
        }

        public override string ToString()
        {
            return $"Запит [{Id.ToString()[..8]}] від клієнта [{ClientId.ToString()[..8]}]: \"{Title}\"";
        }
    }
}

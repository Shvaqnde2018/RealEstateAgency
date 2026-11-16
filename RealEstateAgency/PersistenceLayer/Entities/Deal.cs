using System;

namespace RealEstateAgency.PersistenceLayer.Entities
{
    public class Deal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public Guid PropertyId { get; set; }
        public Guid AgentId { get; set; }
        public Guid ClientId { get; set; }
        public decimal TotalCost { get; set; }
        public DealType Type { get; set; }
        public decimal CommissionPercent { get; set; }

        public Deal() { }

        public Deal(Guid propertyId, Guid agentId, Guid clientId, decimal totalCost, DealType type, decimal commissionPercent)
        {
            if(totalCost <= 0)
                throw new ArgumentException("Загальна вартість угоди повинна бути більшою за нуль.");
            if(commissionPercent < 0 || commissionPercent > 100)
                throw new ArgumentException("Відсоток комісії повинен бути в межах від 0 до 100%.");

            Date = DateTime.Now;
            PropertyId = propertyId;
            ClientId = clientId;
            AgentId = agentId;
            TotalCost = totalCost;
            Type = type;
            CommissionPercent = commissionPercent;
        }

        public decimal CalculateCommissionAmount() => TotalCost * (CommissionPercent / 100m);
        
        public override string ToString()
        {
            return $"[{Id.ToString().Substring(0, 8)}] Дата: {Date:dd.MM.yyyy HH:mm} | Тип: {Type} | Сума: ${TotalCost} | Комісія: {CommissionPercent}% (${CalculateCommissionAmount():F2})";
        }
    }
}
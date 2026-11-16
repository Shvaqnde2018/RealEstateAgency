using System;

namespace RealEstateAgency.PersistenceLayer.Entities
{
    public class PriceHistoryEntry
    {
        public DateTime ChangedAt { get; set; } = DateTime.Now;
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public string Comment { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"[{ChangedAt:dd.MM.yyyy HH:mm}] ${OldPrice} -> ${NewPrice} ({Comment})";
        }
    }
}
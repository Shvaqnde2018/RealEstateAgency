using System;
using System.Data;

namespace RealEstateAgency.PersistenceLayer.Entities
{
    public class StatusHistoryEntry
    {
        public DateTime ChangedAt { get; set; } = DateTime.Now;
        public PropertyStatus OldStatus { get; set; }
        public PropertyStatus NewStatus { get; set; }
        public string Comment { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"[{ChangedAt:dd.MM.yyyy HH:mm:ss}] {OldStatus} -> {NewStatus} ({Comment})";
        }
    }
}

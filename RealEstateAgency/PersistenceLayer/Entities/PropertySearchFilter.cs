using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateAgency.PersistenceLayer.Entities
{
    public class PropertySearchFilter
    {
        public string? Type { get; set; }
        public string? Address { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public double? MinArea { get; set; }
        public double? MaxArea { get; set; }
        public string? District { get; set; }
        public string? Landmark { get; set; }
        public int? Rooms { get; set; }
        public int? MinYearBuilt { get; set; }
        public int? MaxYearBuilt { get; set; }
        public PropertyStatus? Status { get; set; }

        public bool Matches(Property p)
        {
            if (!string.IsNullOrWhiteSpace(Type) && !p.Type.Contains(Type, StringComparison.OrdinalIgnoreCase))
                return false;
            if (MinPrice.HasValue && p.Price < MinPrice.Value)
                return false;
            if (MaxPrice.HasValue && p.Price > MaxPrice.Value)
                return false;
            if (MinArea.HasValue && p.Area < MinArea.Value)
                return false;
            if (MaxArea.HasValue && p.Area > MaxArea.Value)
                return false;
            if (!string.IsNullOrWhiteSpace(District) && !p.District.Contains(District, StringComparison.OrdinalIgnoreCase))
                return false;
            if (!string.IsNullOrWhiteSpace(Landmark) && !p.Landmark.Contains(Landmark, StringComparison.OrdinalIgnoreCase))
                return false;
            if (Rooms.HasValue && p.Rooms != Rooms.Value)
                return false;
            if (MinYearBuilt.HasValue && p.YearBuilt < MinYearBuilt.Value)
                return false;
            if (MaxYearBuilt.HasValue && p.YearBuilt > MaxYearBuilt.Value)
                return false;
            if (Status.HasValue && p.Status != Status.Value)
                return false;

            return true;
        }

        public override string ToString()
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(Type)) parts.Add($"Тип: {Type}");
            if (!string.IsNullOrWhiteSpace(District)) parts.Add($"Район: {District}");
            if (!string.IsNullOrWhiteSpace(Landmark)) parts.Add($"Орієнтир: {Landmark}");

            if (Rooms.HasValue) parts.Add($"Кімнат: {Rooms}");
            if (MinArea.HasValue || MaxArea.HasValue) parts.Add($"Площа: {MinArea ?? 0}..{MaxArea?.ToString() ?? "∞"} м²");
            if (MinPrice.HasValue || MaxPrice.HasValue) parts.Add($"Ціна: ${MinPrice ?? 0}..${MaxPrice?.ToString() ?? "∞"}");
            if (MinYearBuilt.HasValue || MaxYearBuilt.HasValue) parts.Add($"Рік: {MinYearBuilt ?? 0}..{MaxYearBuilt?.ToString() ?? "∞"}");
            if (Status.HasValue) parts.Add($"Статус: {Status}");

            return parts.Count > 0 ? string.Join(", ", parts) : "Всі об'єкти (без обмежень)";
        }
    }
}

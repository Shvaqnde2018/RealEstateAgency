using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateAgency.PersistenceLayer.Entities
{
    public class PropertySearchFilter
    {
        public string? Type { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public double? MinArea { get; set; }
        public double? MaxArea { get; set; }
        public string? District { get; set; }
        public string? Landmark { get; set; }
        public int? Rooms { get; set; }
        public PropertyStatus? Status { get; set; }
    }
}

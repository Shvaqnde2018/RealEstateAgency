using RealEstateAgency.PersistenceLayer.Entities;

namespace RealEstateAgency.DTOs
{
    public record CreatePropertyDto(
        string Type,
        double Area,
        string Address,
        string District,
        string Landmark,
        int Rooms,
        decimal Price,
        PropertyStatus Status,
        int YearBuilt
     );
}

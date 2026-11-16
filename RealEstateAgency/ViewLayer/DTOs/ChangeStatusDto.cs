using RealEstateAgency.PersistenceLayer.Entities;

namespace RealEstateAgency.DTOs
{
    public record ChangeStatusDto(
        PropertyStatus NewStatus,
        string Comment
    );
}
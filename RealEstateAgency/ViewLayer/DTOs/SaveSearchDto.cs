using RealEstateAgency.PersistenceLayer.Entities;

namespace RealEstateAgency.DTOs
{
    public record SaveSearchDto(
        string Title,
        PropertySearchFilter Filter
    );
}
using RealEstateAgency.PersistenceLayer.Entities;

namespace RealEstateAgency.DTOs
{
    public record CreateRequestDto(
        string Title,
        PropertySearchFilter Filter
    );
}

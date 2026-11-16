using RealEstateAgency.PersistenceLayer.Entities;

namespace RealEstateAgency.DTOs
{
    public record CreateClientDto(
        string Name,
        string ContactInfo,
        ClientType Type
    );
}
using RealEstateAgency.PersistenceLayer.Entities;

namespace RealEstateAgency.DTOs
{
    public record CreateDealDto(
        string PropertyId,
        string ClientId,
        string AgentId,
        decimal Price,
        DealType DealType,
        decimal Commission
    );
}
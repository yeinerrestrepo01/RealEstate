using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Services
{
    public interface IPropertyService
    {
        Task<Property> CreateAsync(CreatePropertyRequest request, CancellationToken ct = default);
        Task<Property> UpdateAsync(Guid id, UpdatePropertyRequest request, CancellationToken ct = default);
        Task<Property> ChangePriceAsync(Guid id, decimal newPrice, CancellationToken ct = default);
        Task<PropertyImage> AddImageAsync(Guid propertyId, string url, bool isCover, CancellationToken ct = default);
        Task<Property?> GetAsync(Guid id, CancellationToken ct = default);
        Task<PagedResult<Property>> ListAsync(ListPropertiesQuery query, CancellationToken ct = default);
    }
}

using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Services
{
    public interface IPropertyService
    {
        Task<Property> CreateAsync(CreatePropertyRequest request, CancellationToken ct = default);
        Task<Property> UpdateAsync(int id, UpdatePropertyRequest request, CancellationToken ct = default);
        Task<Property> ChangePriceAsync(int id, decimal newPrice, CancellationToken ct = default);
        Task<PropertyImage> AddImageAsync(int idProperty, string file, bool enabled, CancellationToken ct = default);
        Task<PropertyTrace> AddTraceAsync(int idProperty, AddTraceRequest req, CancellationToken ct = default);

        Task<PropertyDto?> GetAsync(int id, CancellationToken ct = default);
        Task<PagedResult<PropertyListItemDto>> ListAsync(ListPropertiesQuery query, CancellationToken ct = default);
        Task<IReadOnlyList<PropertyTraceDto>> GetTracesAsync(int idProperty, CancellationToken ct = default);
    }
}

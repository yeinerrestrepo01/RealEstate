using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Services
{
    public interface IOwnerService
    {
        Task<Owner> CreateAsync(CreateOwnerRequest req, CancellationToken ct = default);
        Task<Owner?> GetAsync(int id, CancellationToken ct = default);
        Task<PagedResult<Owner>> ListAsync(ListOwnersQuery query, CancellationToken ct = default);
        Task<Owner> UpdateAsync(int id, UpdateOwnerRequest req, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}

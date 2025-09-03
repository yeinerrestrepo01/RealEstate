using Microsoft.EntityFrameworkCore;
using RealEstate.Application.DTOs;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure
{
    public class OwnerService : IOwnerService
    {
        private readonly RealEstateDbContext _db;
        public OwnerService(RealEstateDbContext db) => _db = db;

        public async Task<Owner> CreateAsync(CreateOwnerRequest req, CancellationToken ct = default)
        {
            var owner = new Owner
            {
                Name = req.Name.Trim(),
                Address = req.Address.Trim(),
                Photo = string.IsNullOrWhiteSpace(req.Photo) ? null : req.Photo!.Trim(),
                Birthday = req.Birthday
            };
            _db.Owners.Add(owner);
            await _db.SaveChangesAsync(ct);
            return owner;
        }

        public async Task<Owner?> GetAsync(int id, CancellationToken ct = default)
        {
            return await _db.Owners.AsNoTracking()
                                   .Include(o => o.Properties)
                                   .FirstOrDefaultAsync(o => o.IdOwner == id, ct);
        }

        public async Task<PagedResult<Owner>> ListAsync(ListOwnersQuery query, CancellationToken ct = default)
        {
            var q = _db.Owners.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Name))
                q = q.Where(o => o.Name.Contains(query.Name));

            var total = await q.CountAsync(ct);
            var skip = Math.Max(0, (query.Page - 1) * query.PageSize);
            var items = await q.OrderBy(o => o.IdOwner)
                               .Skip(skip).Take(query.PageSize)
                               .ToListAsync(ct);

            return new PagedResult<Owner>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<Owner> UpdateAsync(int id, UpdateOwnerRequest req, CancellationToken ct = default)
        {
            var owner = await _db.Owners.FirstOrDefaultAsync(o => o.IdOwner == id, ct)
                ?? throw new KeyNotFoundException("Owner not found");

            owner.Name = req.Name.Trim();
            owner.Address = req.Address.Trim();
            owner.Photo = string.IsNullOrWhiteSpace(req.Photo) ? null : req.Photo!.Trim();
            owner.Birthday = req.Birthday;

            await _db.SaveChangesAsync(ct);
            return owner;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var owner = await _db.Owners.Include(o => o.Properties).FirstOrDefaultAsync(o => o.IdOwner == id, ct);
            if (owner is null) return false;
            if (owner.Properties.Any())
                throw new InvalidOperationException("Owner has related properties. Reassign or delete them first.");

            _db.Owners.Remove(owner);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using RealEstate.Application.DTOs;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure
{
    public class PropertyService : IPropertyService
    {
        private readonly RealEstateDbContext _db;

        public PropertyService(RealEstateDbContext db) => _db = db;

        public async Task<Property> CreateAsync(CreatePropertyRequest request, CancellationToken ct = default)
        {
            if (await _db.Properties.AnyAsync(p => p.CodeInternal == request.CodeInternal, ct))
                throw new InvalidOperationException($"Property with CodeInternal '{request.CodeInternal}' already exists.");

            var ownerExists = await _db.Owners.AnyAsync(o => o.IdOwner == request.IdOwner, ct);
            if (!ownerExists) throw new KeyNotFoundException("Owner not found");

            var entity = new Property
            {
                Name = request.Name.Trim(),
                Address = request.Address.Trim(),
                Price = request.Price,
                CodeInternal = request.CodeInternal.Trim(),
                Year = request.Year,
                IdOwner = request.IdOwner
            };
            _db.Properties.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task<Property> UpdateAsync(int id, UpdatePropertyRequest request, CancellationToken ct = default)
        {
            var prop = await _db.Properties.FirstOrDefaultAsync(p => p.IdProperty == id, ct)
                ?? throw new KeyNotFoundException("Property not found");

            if (!string.Equals(prop.CodeInternal, request.CodeInternal, StringComparison.OrdinalIgnoreCase) &&
                await _db.Properties.AnyAsync(p => p.CodeInternal == request.CodeInternal && p.IdProperty != id, ct))
            {
                throw new InvalidOperationException($"Property with CodeInternal '{request.CodeInternal}' already exists.");
            }

            if (!await _db.Owners.AnyAsync(o => o.IdOwner == request.IdOwner, ct))
                throw new KeyNotFoundException("Owner not found");

            prop.Name = request.Name.Trim();
            prop.Address = request.Address.Trim();
            prop.Price = request.Price;
            prop.CodeInternal = request.CodeInternal.Trim();
            prop.Year = request.Year;
            prop.IdOwner = request.IdOwner;

            await _db.SaveChangesAsync(ct);
            return prop;
        }

        public async Task<Property> ChangePriceAsync(int id, decimal newPrice, CancellationToken ct = default)
        {
            var prop = await _db.Properties.FirstOrDefaultAsync(p => p.IdProperty == id, ct)
                ?? throw new KeyNotFoundException("Property not found");
            prop.Price = newPrice;
            await _db.SaveChangesAsync(ct);
            return prop;
        }

        public async Task<PropertyImage> AddImageAsync(int idProperty, string file, bool enabled, CancellationToken ct = default)
        {
            var exists = await _db.Properties.AnyAsync(p => p.IdProperty == idProperty, ct);
            if (!exists) throw new KeyNotFoundException("Property not found");

            var img = new PropertyImage { IdProperty = idProperty, Files = file.Trim(), Enabled = enabled };
            _db.PropertyImages.Add(img);
            await _db.SaveChangesAsync(ct);
            return img;
        }

        public async Task<PropertyTrace> AddTraceAsync(int idProperty, AddTraceRequest req, CancellationToken ct = default)
        {
            var exists = await _db.Properties.AnyAsync(p => p.IdProperty == idProperty, ct);
            if (!exists) throw new KeyNotFoundException("Property not found");

            var trace = new PropertyTrace
            {
                IdProperty = idProperty,
                DateSale = req.DateSale,
                Name = req.Name.Trim(),
                Value = req.Value,
                Tax = req.Tax
            };
            _db.PropertyTraces.Add(trace);
            await _db.SaveChangesAsync(ct);
            return trace;
        }

        public async Task<PropertyDto?> GetAsync(int id, CancellationToken ct = default)
        {
            return await _db.Properties
                .AsNoTracking()
                .Where(p => p.IdProperty == id)
                .Select(PropertyDto.Selector)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<PagedResult<PropertyListItemDto>> ListAsync(ListPropertiesQuery query, CancellationToken ct = default)
        {
            var q = _db.Properties.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name)) q = q.Where(p => p.Name.Contains(query.Name));
            if (!string.IsNullOrWhiteSpace(query.Address)) q = q.Where(p => p.Address.Contains(query.Address));
            if (query.MinPrice.HasValue) q = q.Where(p => p.Price >= query.MinPrice.Value);
            if (query.MaxPrice.HasValue) q = q.Where(p => p.Price <= query.MaxPrice.Value);
            if (query.MinYear.HasValue) q = q.Where(p => p.Year >= query.MinYear.Value);
            if (query.MaxYear.HasValue) q = q.Where(p => p.Year <= query.MaxYear.Value);
            if (query.IdOwner.HasValue) q = q.Where(p => p.IdOwner == query.IdOwner.Value);

            q = (query.SortBy?.ToLowerInvariant()) switch
            {
                "price" => query.Desc ? q.OrderByDescending(p => p.Price) : q.OrderBy(p => p.Price),
                "year" => query.Desc ? q.OrderByDescending(p => p.Year) : q.OrderBy(p => p.Year),
                "name" => query.Desc ? q.OrderByDescending(p => p.Name) : q.OrderBy(p => p.Name),
                _ => q.OrderBy(p => p.IdProperty)
            };

            var total = await q.CountAsync(ct);
            var projected = q.Select(PropertyListItemDto.Selector);
            var skip = Math.Max(0, (query.Page - 1) * query.PageSize);
            var items = await projected.Skip(skip).Take(query.PageSize).ToListAsync(ct);

            return new PagedResult<PropertyListItemDto>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<IReadOnlyList<PropertyTraceDto>> GetTracesAsync(int idProperty, CancellationToken ct = default)
        {
            var traces = await _db.PropertyTraces
                .AsNoTracking()
                .Where(t => t.IdProperty == idProperty)
                .OrderByDescending(t => t.DateSale)
                .Select(PropertyTraceDto.Selector)
                .ToListAsync(ct);

            return traces;
        }
    }
}
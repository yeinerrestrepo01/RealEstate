using Microsoft.EntityFrameworkCore;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;
using RealEstate.Application.Services;

namespace RealEstate.Infrastructure
{
    public class PropertyService : IPropertyService
    {
        private readonly RealEstateDbContext _db;

        public PropertyService(RealEstateDbContext db)
        {
            _db = db;
        }

        public async Task<Property> CreateAsync(CreatePropertyRequest request, CancellationToken ct = default)
        {
            if (await _db.Properties.AnyAsync(p => p.Code == request.Code, ct))
                throw new InvalidOperationException($"Property with code '{request.Code}' already exists.");

            var entity = new Property
            {
                Code = request.Code.Trim(),
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                Address = request.Address.Trim(),
                City = request.City.Trim(),
                State = request.State.Trim().ToUpperInvariant(),
                ZipCode = request.ZipCode.Trim(),
                Bedrooms = request.Bedrooms,
                Bathrooms = request.Bathrooms,
                AreaSqFt = request.AreaSqFt,
                YearBuilt = request.YearBuilt,
                Price = request.Price,
                Stories = request.Stories,
                ParkingSpaces = request.ParkingSpaces,
                HasHeating = request.HasHeating,
                HasCooling = request.HasCooling,
                LotSizeSqFt = request.LotSizeSqFt,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Properties.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task<Property> UpdateAsync(Guid id, UpdatePropertyRequest request, CancellationToken ct = default)
        {
            var prop = await _db.Properties.FirstOrDefaultAsync(p => p.Id == id, ct)
                ?? throw new KeyNotFoundException("Property not found");

            prop.Title = request.Title.Trim();
            prop.Description = request.Description?.Trim();
            prop.Address = request.Address.Trim();
            prop.City = request.City.Trim();
            prop.State = request.State.Trim().ToUpperInvariant();
            prop.ZipCode = request.ZipCode.Trim();
            prop.Bedrooms = request.Bedrooms;
            prop.Bathrooms = request.Bathrooms;
            prop.AreaSqFt = request.AreaSqFt;
            prop.YearBuilt = request.YearBuilt;
            prop.Status = request.Status;
            prop.Stories = request.Stories;
            prop.ParkingSpaces = request.ParkingSpaces;
            prop.HasHeating = request.HasHeating;
            prop.HasCooling = request.HasCooling;
            prop.LotSizeSqFt = request.LotSizeSqFt;
            prop.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            return prop;
        }

        public async Task<Property> ChangePriceAsync(Guid id, decimal newPrice, CancellationToken ct = default)
        {
            var prop = await _db.Properties.FirstOrDefaultAsync(p => p.Id == id, ct)
                ?? throw new KeyNotFoundException("Property not found");

            prop.Price = newPrice;
            prop.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return prop;
        }

        public async Task<PropertyImage> AddImageAsync(Guid propertyId, string url, bool isCover, CancellationToken ct = default)
        {
            var prop = await _db.Properties.FirstOrDefaultAsync(p => p.Id == propertyId, ct)
                ?? throw new KeyNotFoundException("Property not found");

            if (isCover)
            {
                // ensure single cover image
                var covers = _db.PropertyImages.Where(i => i.PropertyId == propertyId && i.IsCover);
                await covers.ForEachAsync(i => i.IsCover = false, ct);
            }

            var img = new PropertyImage
            {
                PropertyId = propertyId,
                Url = url.Trim(),
                IsCover = isCover
            };

            _db.PropertyImages.Add(img);
            await _db.SaveChangesAsync(ct);
            return img;
        }

        public async Task<Property?> GetAsync(Guid id, CancellationToken ct = default)
        {
            return await _db.Properties
                .AsNoTracking()
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<PagedResult<Property>> ListAsync(ListPropertiesQuery query, CancellationToken ct = default)
        {
            var q = _db.Properties.AsNoTracking().Include(p => p.Images).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.City))
                q = q.Where(p => p.City == query.City);
            if (!string.IsNullOrWhiteSpace(query.State))
                q = q.Where(p => p.State == query.State);
            if (query.MinBedrooms.HasValue)
                q = q.Where(p => p.Bedrooms >= query.MinBedrooms.Value);
            if (query.MinBathrooms.HasValue)
                q = q.Where(p => p.Bathrooms >= query.MinBathrooms.Value);
            if (query.MinArea.HasValue)
                q = q.Where(p => p.AreaSqFt >= query.MinArea.Value);
            if (query.MaxArea.HasValue)
                q = q.Where(p => p.AreaSqFt <= query.MaxArea.Value);
            if (query.MinPrice.HasValue)
                q = q.Where(p => p.Price >= query.MinPrice.Value);
            if (query.MaxPrice.HasValue)
                q = q.Where(p => p.Price <= query.MaxPrice.Value);
            if (query.Status.HasValue)
                q = q.Where(p => p.Status == query.Status.Value);

            q = (query.SortBy?.ToLowerInvariant()) switch
            {
                "price" => (query.Desc ? q.OrderByDescending(p => p.Price) : q.OrderBy(p => p.Price)),
                "area" => (query.Desc ? q.OrderByDescending(p => p.AreaSqFt) : q.OrderBy(p => p.AreaSqFt)),
                "createdat" => (query.Desc ? q.OrderByDescending(p => p.CreatedAt) : q.OrderBy(p => p.CreatedAt)),
                _ => q.OrderBy(p => p.Title)
            };

            var total = await q.CountAsync(ct);
            var skip = Math.Max(0, (query.Page - 1) * query.PageSize);
            var items = await q.Skip(skip).Take(query.PageSize).ToListAsync(ct);

            return new PagedResult<Property>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total,
                Items = items
            };
        }
    }
}

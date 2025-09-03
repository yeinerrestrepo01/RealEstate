using System.Linq.Expressions;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.DTOs
{
    public class OwnerSummaryDto
    {
        public int IdOwner { get; set; }
        public string Name { get; set; } = default!;

        public static Expression<Func<Owner, OwnerSummaryDto>> Selector =>
            o => new OwnerSummaryDto { IdOwner = o.IdOwner, Name = o.Name };

        public static implicit operator OwnerSummaryDto?(Owner? o) =>
            o is null ? null : new OwnerSummaryDto { IdOwner = o.IdOwner, Name = o.Name };
    }

    public class PropertyImageDto
    {
        public int IdPropertyImage { get; set; }
        public string File { get; set; } = default!;
        public bool Enabled { get; set; }

        public static Expression<Func<PropertyImage, PropertyImageDto>> Selector =>
            i => new PropertyImageDto { IdPropertyImage = i.IdPropertyImage, File = i.Files, Enabled = i.Enabled };

        public static implicit operator PropertyImageDto?(PropertyImage? i) =>
            i is null ? null : new PropertyImageDto { IdPropertyImage = i.IdPropertyImage, File = i.Files, Enabled = i.Enabled };
    }

    public class PropertyTraceDto
    {
        public int IdPropertyTrace { get; set; }
        public DateTime DateSale { get; set; }
        public string Name { get; set; } = default!;
        public decimal Value { get; set; }
        public decimal Tax { get; set; }

        public static Expression<Func<PropertyTrace, PropertyTraceDto>> Selector =>
            t => new PropertyTraceDto { IdPropertyTrace = t.IdPropertyTrace, DateSale = t.DateSale, Name = t.Name, Value = t.Value, Tax = t.Tax };

        public static implicit operator PropertyTraceDto?(PropertyTrace? t) =>
            t is null ? null : new PropertyTraceDto { IdPropertyTrace = t.IdPropertyTrace, DateSale = t.DateSale, Name = t.Name, Value = t.Value, Tax = t.Tax };
    }

    public class PropertyDto
    {
        public int IdProperty { get; set; }
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public decimal Price { get; set; }
        public string CodeInternal { get; set; } = default!;
        public int Year { get; set; }
        public OwnerSummaryDto Owner { get; set; } = default!;
        public List<PropertyImageDto> Images { get; set; } = new();
        public List<PropertyTraceDto> Traces { get; set; } = new();

        public static Expression<Func<Property, PropertyDto>> Selector =>
            p => new PropertyDto
            {
                IdProperty = p.IdProperty,
                Name = p.Name,
                Address = p.Address,
                Price = p.Price,
                CodeInternal = p.CodeInternal,
                Year = p.Year,
                Owner = new OwnerSummaryDto { IdOwner = p.Owner.IdOwner, Name = p.Owner.Name },
                Images = p.Images.Select(i => new PropertyImageDto
                {
                    IdPropertyImage = i.IdPropertyImage,
                    File = i.Files,
                    Enabled = i.Enabled
                }).ToList(),
                Traces = p.Traces.OrderByDescending(t => t.DateSale).Select(t => new PropertyTraceDto
                {
                    IdPropertyTrace = t.IdPropertyTrace,
                    DateSale = t.DateSale,
                    Name = t.Name,
                    Value = t.Value,
                    Tax = t.Tax
                }).ToList()
            };

        public static implicit operator PropertyDto?(Property? p) =>
            p is null ? null : new PropertyDto
            {
                IdProperty = p.IdProperty,
                Name = p.Name,
                Address = p.Address,
                Price = p.Price,
                CodeInternal = p.CodeInternal,
                Year = p.Year,
                Owner = (OwnerSummaryDto?)p.Owner!,
                Images = p.Images?.Select(i => (PropertyImageDto)i!).ToList() ?? new(),
                Traces = p.Traces?.OrderByDescending(t => t.DateSale).Select(t => (PropertyTraceDto)t!).ToList() ?? new()
            };
    }

    public class PropertyListItemDto
    {
        public int IdProperty { get; set; }
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public decimal Price { get; set; }
        public int Year { get; set; }
        public string CodeInternal { get; set; } = default!;
        public OwnerSummaryDto Owner { get; set; } = default!;
        public int ImagesCount { get; set; }

        public static Expression<Func<Property, PropertyListItemDto>> Selector =>
            p => new PropertyListItemDto
            {
                IdProperty = p.IdProperty,
                Name = p.Name,
                Address = p.Address,
                Price = p.Price,
                Year = p.Year,
                CodeInternal = p.CodeInternal,
                Owner = new OwnerSummaryDto { IdOwner = p.Owner.IdOwner, Name = p.Owner.Name },
                ImagesCount = p.Images.Count()
            };

        public static implicit operator PropertyListItemDto?(Property? p) =>
            p is null ? null : new PropertyListItemDto
            {
                IdProperty = p.IdProperty,
                Name = p.Name,
                Address = p.Address,
                Price = p.Price,
                Year = p.Year,
                CodeInternal = p.CodeInternal,
                Owner = (OwnerSummaryDto?)p.Owner!,
                ImagesCount = p.Images?.Count ?? 0
            };
    }
}
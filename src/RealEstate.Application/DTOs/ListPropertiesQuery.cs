using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs
{
    public class ListPropertiesQuery
    {
        public string? City { get; set; }
        public string? State { get; set; }
        public int? MinBedrooms { get; set; }
        public decimal? MinBathrooms { get; set; }
        public int? MinArea { get; set; }
        public int? MaxArea { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public PropertyStatus? Status { get; set; }
        public string? SortBy { get; set; } // price, area, createdAt
        public bool Desc { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}

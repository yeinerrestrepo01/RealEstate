using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities
{
    public class Property
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = default!; // Unique code per property (e.g., MLS-like)
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string Address { get; set; } = default!;
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string ZipCode { get; set; } = default!;
        public string Country { get; set; } = "USA";

        public int Bedrooms { get; set; }
        public decimal Bathrooms { get; set; } // allow half baths
        public int AreaSqFt { get; set; }
        public int YearBuilt { get; set; }

        public decimal Price { get; set; }
        public PropertyStatus Status { get; set; } = PropertyStatus.Draft;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();

        // Building details (owned-like data)
        public int Stories { get; set; }
        public int ParkingSpaces { get; set; }
        public bool HasHeating { get; set; }
        public bool HasCooling { get; set; }
        public decimal LotSizeSqFt { get; set; }
    }
}

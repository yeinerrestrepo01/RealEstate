using System.ComponentModel.DataAnnotations;

namespace RealEstate.Application.DTOs
{
    public class CreatePropertyRequest
    {
        [Required, StringLength(32, MinimumLength = 3)]
        public string Code { get; set; } = default!;

        [Required, StringLength(160)]
        public string Title { get; set; } = default!;

        [StringLength(4000)]
        public string? Description { get; set; }

        [Required] public string Address { get; set; } = default!;
        [Required] public string City { get; set; } = default!;
        [Required] public string State { get; set; } = default!;
        [Required] public string ZipCode { get; set; } = default!;

        [Range(0, 50)] public int Bedrooms { get; set; }
        [Range(0, 50)] public decimal Bathrooms { get; set; }
        [Range(0, 100000)] public int AreaSqFt { get; set; }
        [Range(1800, 2100)] public int YearBuilt { get; set; }
        [Range(0, 1000000000)] public decimal Price { get; set; }

        // Building
        public int Stories { get; set; }
        public int ParkingSpaces { get; set; }
        public bool HasHeating { get; set; }
        public bool HasCooling { get; set; }
        public decimal LotSizeSqFt { get; set; }
    }
}

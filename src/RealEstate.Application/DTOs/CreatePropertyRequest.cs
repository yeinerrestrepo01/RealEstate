using System.ComponentModel.DataAnnotations;
namespace RealEstate.Application.DTOs
{
    public class CreatePropertyRequest
    {
        [Required, StringLength(200)] public string Name { get; set; } = default!;
        [Required, StringLength(250)] public string Address { get; set; } = default!;
        [Range(0, 1000000000)] public decimal Price { get; set; }
        [Required, StringLength(64)] public string CodeInternal { get; set; } = default!;
        [Range(1800, 2100)] public int Year { get; set; }
        [Range(1, int.MaxValue)] public int IdOwner { get; set; }
    }
}

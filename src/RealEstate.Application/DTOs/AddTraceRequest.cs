using System.ComponentModel.DataAnnotations;
namespace RealEstate.Application.DTOs
{
    public class AddTraceRequest
    {
        public DateTime DateSale { get; set; }
        [Required, StringLength(200)] public string Name { get; set; } = default!;
        [Range(0, 1000000000)] public decimal Value { get; set; }
        [Range(0, 1000000000)] public decimal Tax { get; set; }
    }
}

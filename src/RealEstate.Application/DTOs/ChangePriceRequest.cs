using System.ComponentModel.DataAnnotations;

namespace RealEstate.Application.DTOs
{
    public class ChangePriceRequest
    {
        [Range(1, 1000000000)]
        public decimal Price { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
namespace RealEstate.Application.DTOs
{
    public class UpdateOwnerRequest
    {
        [Required, StringLength(200)] public string Name { get; set; } = default!;
        [Required, StringLength(250)] public string Address { get; set; } = default!;
        [Url] public string? Photo { get; set; }
        public DateTime? Birthday { get; set; }
    }
}

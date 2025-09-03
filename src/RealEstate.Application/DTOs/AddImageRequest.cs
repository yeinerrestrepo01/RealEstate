using System.ComponentModel.DataAnnotations;
namespace RealEstate.Application.DTOs
{
    public class AddImageRequest
    {
        [Required, Url]
        public string File { get; set; } = default!;
        public bool Enabled { get; set; } = true;
    }
}

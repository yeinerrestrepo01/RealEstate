using System.ComponentModel.DataAnnotations;

namespace RealEstate.Application.DTOs
{
    public class AddImageRequest
    {
        [Required, Url]
        public string Url { get; set; } = default!;
        public bool IsCover { get; set; } = false;
    }
}

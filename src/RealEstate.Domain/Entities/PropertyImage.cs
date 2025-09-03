using System.Text.Json.Serialization;

namespace RealEstate.Domain.Entities
{
    public class PropertyImage
    {
        public int IdPropertyImage { get; set; }
        public int IdProperty { get; set; }
        public string Files { get; set; } = default!;
        public bool Enabled { get; set; } = true;

        [JsonIgnore]
        public Property Property { get; set; } = default!;
    }
}
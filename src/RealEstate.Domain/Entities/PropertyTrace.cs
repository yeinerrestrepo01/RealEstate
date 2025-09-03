using System.Text.Json.Serialization;
namespace RealEstate.Domain.Entities
{
    public class PropertyTrace
    {
        public int IdPropertyTrace { get; set; }
        public DateTime DateSale { get; set; }
        public string Name { get; set; } = default!;
        public decimal Value { get; set; }
        public decimal Tax { get; set; }
        public int IdProperty { get; set; }
        [JsonIgnore]
        public Property Property { get; set; } = default!;
    }
}

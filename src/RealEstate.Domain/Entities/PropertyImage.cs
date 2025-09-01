namespace RealEstate.Domain.Entities
{
    public class PropertyImage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PropertyId { get; set; }
        public string Url { get; set; } = default!;
        public bool IsCover { get; set; }

        public Property? Property { get; set; }
    }
}

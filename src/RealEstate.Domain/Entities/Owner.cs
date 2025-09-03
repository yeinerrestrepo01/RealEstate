namespace RealEstate.Domain.Entities
{
    public class Owner
    {
        public int IdOwner { get; set; }
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string? Photo { get; set; }
        public DateTime? Birthday { get; set; }
        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}

namespace RealEstate.Application.DTOs
{
    public class ListOwnersQuery
    {
        public string? Name { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}

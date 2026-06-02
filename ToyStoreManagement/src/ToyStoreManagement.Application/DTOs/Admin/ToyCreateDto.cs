namespace ToyStoreManagement.Application.DTOs.Admin
{
    public class ToyCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid CategoryId { get; set; } 
        public decimal Price { get; set; }
        public int MinimumAge { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
    }
}

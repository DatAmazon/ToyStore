using System;

namespace ToyStoreManagement.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class ProductExportDto
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinimumAge { get; set; }
        public string Manufacturer { get; set; }
        public string CategoryName { get; set; }
    }
}

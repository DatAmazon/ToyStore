using System;

namespace ToyStoreManagement.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinimumAge { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }

    public class ProductExportDto
    {
        public string Name { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int MinimumAge { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }
}

using System;

namespace ToyStoreManagement.Application.DTOs
{
    public class CategoryDto
    {
        public Guid? Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}

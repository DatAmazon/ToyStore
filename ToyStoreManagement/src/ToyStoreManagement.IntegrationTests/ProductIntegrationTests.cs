using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ToyStoreManagement.Domain.Entities;
using Xunit;

namespace ToyStoreManagement.IntegrationTests
{
    public class ProductIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ProductIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsSuccessAndProductList()
        {
            // Act
            var response = await _client.GetAsync("/api/Products");

            // Assert
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadFromJsonAsync<IEnumerable<object>>();
            Assert.NotNull(products);
        }

        [Fact]
        public async Task CreateMultiple_WithNewCategory_ShouldSucceed()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Integration Test Toy " + Guid.NewGuid(),
                    Price = 100000,
                    StockQuantity = 10,
                    CategoryId = categoryId,
                    MinimumAge = 3,
                    Manufacturer = "Test Factory"
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Products/create-products", products);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains("Processed successfully!", result);
        }
    }
}

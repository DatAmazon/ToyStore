using ToyStoreManagement.Application.DTOs.Inventory;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IRepository<InventoryReceipt> _receiptRepo;
        private readonly IRepository<Product> _productRepo;

        public InventoryService(IRepository<InventoryReceipt> receiptRepo, IRepository<Product> productRepo)
        {
            _receiptRepo = receiptRepo;
            _productRepo = productRepo;
        }

        public async Task<Guid> ReceiveGoodsAsync(InventoryRequest request)
        {
            try
            {
                var receipt = new InventoryReceipt
                {
                    SupplierId = request.SupplierId,
                    ReceivedDate = DateTime.UtcNow,
                    TotalAmount = request.Items.Sum(x => x.Quantity * x.UnitPrice)
                };

                foreach (var item in request.Items)
                {
                    var product = await _productRepo.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity; // CỘNG KHO
                    }
                    receipt.Details.Add(new InventoryReceiptDetail
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    });
                }

                await _receiptRepo.AddAsync(receipt);
                await _receiptRepo.SaveChangesAsync();
                
                return receipt.InventoryReceiptId;
            }
            catch
            {
                throw;
            }
        }
    }
}

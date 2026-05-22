using ToyStoreManagement.Data;
using ToyStoreManagement.DTOs.Inventory;
using ToyStoreManagement.Entities;
using ToyStoreManagement.IServices;

namespace ToyStoreManagement.Services
{
    public class InventoryService : IInventoryService
    {

        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context) => _context = context;

        public async Task<Guid> ReceiveGoodsAsync(InventoryRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
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
                    var product = await _context.Products.FindAsync(item.ProductId);
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

                _context.InventoryReceipts.Add(receipt);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return receipt.InventoryReceiptId;
            }
            catch
            {
                await transaction.RollbackAsync(); throw;
            }
        }
    }
}

using ToyStoreManagement.DTOs.Inventory;

namespace ToyStoreManagement.IServices
{
    public interface IInventoryService
    {
        Task<Guid> ReceiveGoodsAsync(InventoryRequest request);
    }
}

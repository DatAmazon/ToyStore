using ToyStoreManagement.Application.DTOs.Inventory;

namespace ToyStoreManagement.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<Guid> ReceiveGoodsAsync(InventoryRequest request);
    }
}

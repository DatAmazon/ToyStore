using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.DTOs.Inventory;
using ToyStoreManagement.IServices;

namespace ToyStoreManagement.Controllers.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        public InventoryController(IInventoryService inventoryService) => _inventoryService = inventoryService;

        [HttpPost("receive")]
        public async Task<IActionResult> Receive(InventoryRequest request) => Ok(await _inventoryService.ReceiveGoodsAsync(request));
    }
}

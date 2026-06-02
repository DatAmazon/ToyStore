using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.Application.DTOs.Inventory;
using ToyStoreManagement.Application.Interfaces;

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

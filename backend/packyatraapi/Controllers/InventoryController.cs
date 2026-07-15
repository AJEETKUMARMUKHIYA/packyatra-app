using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoversAndPackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _inventoryService;

        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        [Route("GetAllInventory")]
        public async Task<IActionResult> GetAllInventory()
        {
            var items = await _inventoryService.GetAllInventoryItemsAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryItemById(int id)
        {
            var item = await _inventoryService.GetInventoryItemByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetInventoryByCategory(string category)
        {
            var items = await _inventoryService.GetInventoryItemsByCategoryAsync(category);
            return Ok(items);
        }
    }
}

using Application.DTOs.InventoryStock;
using Application.UseCases.InventoryStock;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Logistic
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryStockController : BaseController
    {
        private readonly CreateInventoryStock _createInventoryStock;

        public InventoryStockController(CreateInventoryStock createInventoryStock)
        {
            _createInventoryStock = createInventoryStock;
        }

        [HttpPost]
        [Route("InventoryStockCreate")]
        public async Task<IActionResult> Create([FromBody] InventoryStockCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createInventoryStock.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }
    }
}

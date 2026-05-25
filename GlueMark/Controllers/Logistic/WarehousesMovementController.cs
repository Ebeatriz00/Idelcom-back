using Application.DTOs.WarehousesMovement;
using Application.UseCases.WarehousesMovement;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Logistic
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesMovementController : BaseController
    {
        private readonly CreateWarehousesMovement _createWarehousesMovement;
        private readonly WarehouseMovementQueryService _warehouseMovementQueryService;

        public WarehousesMovementController(
            CreateWarehousesMovement createWarehousesMovement,
            WarehouseMovementQueryService warehouseMovementQueryService)
        {
            _createWarehousesMovement = createWarehousesMovement;
            _warehouseMovementQueryService = warehouseMovementQueryService;
        }

        [HttpGet]
        [Route("GetAllWarehousesMovement")]
        public async Task<IActionResult> List([FromQuery] WarehouseMovementFilterDto filter)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _warehouseMovementQueryService.ListAsync(businessId, filter);
            return Ok(result);
        }

        [HttpGet()]
        [Route("GetByIdWarehousesMovement")]
        public async Task<IActionResult> GetById(long id)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _warehouseMovementQueryService.GetByIdAsync(businessId, id);
            return Ok(result);
        }

        [HttpGet()]
        [Route("GetAvailableStock")]
        public async Task<IActionResult> GetAvailableStock([FromQuery] InventoryStockAvailableFilterDto filter)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _warehouseMovementQueryService.GetAvailableStockAsync(businessId, filter);
            return Ok(result);
        }

        [HttpPost]
        [Route("WarehousesMovementCreate")]
        public async Task<IActionResult> Create([FromBody] WarehousesMovementCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createWarehousesMovement.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }
    }
}

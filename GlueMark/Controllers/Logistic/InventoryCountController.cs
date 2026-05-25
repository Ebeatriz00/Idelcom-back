using Application.DTOs.InventoryCount;
using Application.UseCases.InventoryCount;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Logistic
{
    [Route("api/inventory-count")]
    [ApiController]
    public class InventoryCountController(
        CreateInventoryCountUseCase createInventoryCount,
        StartInventoryCountUseCase startInventoryCount,
        UpdateInventoryCountDetailsUseCase updateInventoryCountDetails,
        CloseInventoryCountUseCase closeInventoryCount,
        GenerateInventoryCountAdjustmentsUseCase generateAdjustments,
        CancelInventoryCountUseCase cancelInventoryCount,
        ListInventoryCountUseCase listInventoryCount,
        GetInventoryCountByIdUseCase getInventoryCountById) : BaseController
    {
        private readonly CreateInventoryCountUseCase _createInventoryCount = createInventoryCount;
        private readonly StartInventoryCountUseCase _startInventoryCount = startInventoryCount;
        private readonly UpdateInventoryCountDetailsUseCase _updateInventoryCountDetails = updateInventoryCountDetails;
        private readonly CloseInventoryCountUseCase _closeInventoryCount = closeInventoryCount;
        private readonly GenerateInventoryCountAdjustmentsUseCase _generateAdjustments = generateAdjustments;
        private readonly CancelInventoryCountUseCase _cancelInventoryCount = cancelInventoryCount;
        private readonly ListInventoryCountUseCase _listInventoryCount = listInventoryCount;
        private readonly GetInventoryCountByIdUseCase _getInventoryCountById = getInventoryCountById;

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] InventoryCountCreateDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createInventoryCount.ExecuteAsync(request, userId, businessId);
            return Ok(result);
        }

        [HttpPost("start/{inventoryCountId:long}")]
        public async Task<IActionResult> Start(long inventoryCountId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _startInventoryCount.ExecuteAsync(inventoryCountId, userId, businessId);
            return Ok(result);
        }

        [HttpPut("update-details")]
        public async Task<IActionResult> UpdateDetails([FromBody] InventoryCountUpdateDetailsDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updateInventoryCountDetails.ExecuteAsync(request, userId, businessId);
            return Ok(result);
        }

        [HttpPost("close/{inventoryCountId:long}")]
        public async Task<IActionResult> Close(long inventoryCountId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _closeInventoryCount.ExecuteAsync(inventoryCountId, userId, businessId);
            return Ok(result);
        }

        [HttpPost("generate-adjustments/{inventoryCountId:long}")]
        public async Task<IActionResult> GenerateAdjustments(long inventoryCountId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _generateAdjustments.ExecuteAsync(inventoryCountId, userId, businessId);
            return Ok(result);
        }

        [HttpPost("cancel/{inventoryCountId:long}")]
        public async Task<IActionResult> Cancel(long inventoryCountId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _cancelInventoryCount.ExecuteAsync(inventoryCountId, userId, businessId);
            return Ok(result);
        }

        [HttpGet("list")]
        public async Task<IActionResult> List([FromQuery] InventoryCountListFilterDto filter)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _listInventoryCount.ExecuteAsync(filter, businessId);
            return Ok(result);
        }

        [HttpGet("get/{inventoryCountId:long}")]
        public async Task<IActionResult> GetById(long inventoryCountId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getInventoryCountById.ExecuteAsync(businessId, inventoryCountId);
            return Ok(result);
        }
    }
}

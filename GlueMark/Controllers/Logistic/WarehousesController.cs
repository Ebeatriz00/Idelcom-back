using Application.DTOs.Warehouses;
using Application.UseCases.Warehouses;
using Azure;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Logistic
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : BaseController
    {
        private readonly CreateWarehouses _createWarehouses;
        private readonly GetAllWarehouses _getAllWarehouses;
        private readonly GetWarehousesById _getWarehousesById;
        private readonly UpdateWarehouses _updateWarehouses;
        private readonly PatchWarehousesStatus _patchWarehousesStatus;
        private readonly GetSelectWarehouses _getSelectWarehouses;

        public WarehousesController(
            CreateWarehouses createWarehouses,
            GetAllWarehouses getAllWarehouses,
            GetWarehousesById getWarehousesById,
            UpdateWarehouses updateWarehouses,
            PatchWarehousesStatus patchWarehousesStatus,
            GetSelectWarehouses getSelectWarehouses)
        {
            _createWarehouses = createWarehouses;
            _getAllWarehouses = getAllWarehouses;
            _getWarehousesById = getWarehousesById;
            _updateWarehouses = updateWarehouses;
            _patchWarehousesStatus = patchWarehousesStatus;
            _getSelectWarehouses = getSelectWarehouses;
        }

        [HttpPost]
        [Route("WarehousesCreate")]
        public async Task<IActionResult> Create([FromBody] WarehousesCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createWarehouses.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("WarehousesList")]
        public async Task<IActionResult> GetList([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllWarehouses.ExecuteAsync(businessId, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron almacenes." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("WarehousesSelect")]
        public async Task<IActionResult> WarehousesSelect(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectWarehouses.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("WarehousesById")]
        public async Task<IActionResult> GetById([FromQuery] long warehousesId)
        {
            var result = await _getWarehousesById.ExecuteAsync(warehousesId);
            if (result == null)
                return NotFound(new { message = "No se encontró el almacén." });
            return Ok(result);
        }

        [HttpPut]
        [Route("WarehousesUpdate")]
        public async Task<IActionResult> Update([FromBody] WarehousesUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updateWarehouses.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPatch]
        [Route("WarehousesStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] WarehousesStatusToggleDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _patchWarehousesStatus.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }
    }
}

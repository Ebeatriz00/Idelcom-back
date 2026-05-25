using Application.DTOs.Operations.OperationsWorkOrder;
using Application.UseCases.Operations.OperationsWorkOrder;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OperationsWorkOrderController(
        CreateOperationsWorkOrder createOperationsWorkOrder,
        UpdateOperationsWorkOrder updateOperationsWorkOrder,
        DeleteOperationsWorkOrder deleteOperationsWorkOrder,
        GetAllOperationsWorkOrder getAllOperationsWorkOrder,
        GetByIdOperationsWorkOrder getByIdOperationsWorkOrder
        ) : BaseController
    {
        private readonly CreateOperationsWorkOrder _createOperationsWorkOrder = createOperationsWorkOrder;
        private readonly UpdateOperationsWorkOrder _updateOperationsWorkOrder = updateOperationsWorkOrder;
        private readonly DeleteOperationsWorkOrder _deleteOperationsWorkOrder = deleteOperationsWorkOrder;
        private readonly GetAllOperationsWorkOrder _getAllOperationsWorkOrder = getAllOperationsWorkOrder;
        private readonly GetByIdOperationsWorkOrder _getByIdOperationsWorkOrder = getByIdOperationsWorkOrder;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OperationsWorkOrderCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _createOperationsWorkOrder.ExecuteAsync(dto, userId, businessId);
            return Ok(result);

        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] OperationsWorkOrderUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _updateOperationsWorkOrder.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long workOrderId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _deleteOperationsWorkOrder.ExecuteAsync(workOrderId, businessId, userId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] long? operationsId = null)
        {
            var businessId = GetCurrentBusinessId();

            var result = await _getAllOperationsWorkOrder.ExecuteAsync(businessId, page, pageSize, search, operationsId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] long workOrderId)
        {
            var businessId = GetCurrentBusinessId();

            var result = await _getByIdOperationsWorkOrder.ExecuteAsync(workOrderId, businessId);
            return Ok(result);
        }
    }
}

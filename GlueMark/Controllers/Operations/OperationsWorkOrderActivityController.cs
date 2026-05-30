using Application.DTOs.Operations.OperationsWorkOrderActivity;
using Application.UseCases.Operations.OperationsWorkOrderActivity;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OperationsWorkOrderActivityController(
        CreateOperationsWorkOrderActivity createUseCase,
        UpdateOperationsWorkOrderActivity updateUseCase,
        GetAllOperationsWorkOrderActivity getAllUseCase,
        GetSelectOperationsWorkOrderActivity getSelectUseCase,
        DeleteOperationsWorkOrderActivity deleteUseCase,
        CloneOperationsWorkOrderActivity cloneUseCase) : BaseController
    {
        private readonly CreateOperationsWorkOrderActivity _createUseCase = createUseCase;
        private readonly UpdateOperationsWorkOrderActivity _updateUseCase = updateUseCase;
        private readonly GetAllOperationsWorkOrderActivity _getAllUseCase = getAllUseCase;
        private readonly GetSelectOperationsWorkOrderActivity _getSelectUseCase = getSelectUseCase;
        private readonly DeleteOperationsWorkOrderActivity _deleteUseCase = deleteUseCase;
        private readonly CloneOperationsWorkOrderActivity _cloneUseCase = cloneUseCase;

        [HttpPost]
        public async Task<IActionResult> Clone([FromBody] OperationsWorkOrderActivityCloneDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _cloneUseCase.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OperationsWorkOrderActivityCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _createUseCase.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] OperationsWorkOrderActivityUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _updateUseCase.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] List<long> activityIds)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _deleteUseCase.ExecuteAsync(activityIds, businessId, userId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] long workOrderId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllUseCase.ExecuteAsync(workOrderId, businessId, page, pageSize, search);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSelect(
            [FromQuery] long operationsId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectUseCase.ExecuteAsync(businessId, operationsId, page, pageSize, search);
            return Ok(result);
        }
    }
}

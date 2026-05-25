using Application.DTOs.Operations.OperationsWorkOrderResponsible;
using Application.UseCases.Operations.OperationsWorkOrderResponsible;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OperationsWorkOrderResponsibleController(
        CreateOperationsWorkOrderResponsible createUseCase,
        UpdateOperationsWorkOrderResponsible updateUseCase,
        DeleteOperationsWorkOrderResponsible deleteUseCase,
        GetAllOperationsWorkOrderResponsible getAllUseCase,
        GetByIdOperationsWorkOrderResponsible getByIdUseCase) : BaseController
    {
        private readonly CreateOperationsWorkOrderResponsible _createUseCase = createUseCase;
        private readonly UpdateOperationsWorkOrderResponsible _updateUseCase = updateUseCase;
        private readonly DeleteOperationsWorkOrderResponsible _deleteUseCase = deleteUseCase;
        private readonly GetAllOperationsWorkOrderResponsible _getAllUseCase = getAllUseCase;
        private readonly GetByIdOperationsWorkOrderResponsible _getByIdUseCase = getByIdUseCase;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OperationsWorkOrderResponsibleCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _createUseCase.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] OperationsWorkOrderResponsibleUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _updateUseCase.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long workOrderResponsibleId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _deleteUseCase.ExecuteAsync(workOrderResponsibleId, businessId, userId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllUseCase.ExecuteAsync(businessId, page, pageSize, search);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] long workOrderResponsibleId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getByIdUseCase.ExecuteAsync(workOrderResponsibleId, businessId);
            return Ok(result);
        }
    }
}

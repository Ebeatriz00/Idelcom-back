using Application.DTOs.OperationsSupervisor;
using Application.UseCases.Operations;
using Application.UseCases.OperationsSupervisor;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.OperationsSupervisor
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OperationsSupervisorController(
        CreateOperationsSupervisor createOperationsSupervisor,
        GetAllOperationsSupervisor getAllOperationsSupervisor,
        GetByIdOperationsSupervisor getByIdOperationsSupervisor,
        UpdateOperationsSupervisor updateOperationsSupervisor,
        DeleteOperationsSupervisor deleteOperationsSupervisor
        ) : BaseController

    {
        private readonly CreateOperationsSupervisor _createOperationsSupervisor = createOperationsSupervisor;
        private readonly GetAllOperationsSupervisor _getAllOperationsSupervisor = getAllOperationsSupervisor;
        private readonly GetByIdOperationsSupervisor _getByIdOperationsSupervisor = getByIdOperationsSupervisor;
        private readonly UpdateOperationsSupervisor _updateOperationsSupervisor = updateOperationsSupervisor;
        private readonly DeleteOperationsSupervisor _deleteOperationsSupervisor = deleteOperationsSupervisor;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OperationsSupervisorCreateDto dto)
        {
            var businessId = GetCurrentBusinessId();

            var result = await _createOperationsSupervisor.ExecuteAsync(dto, businessId);
            return Ok(result);
        }

        [HttpGet] 
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();

            var result = await _getAllOperationsSupervisor.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] long supervisorId)
        {
            var result = await _getByIdOperationsSupervisor.ExecuteAsync(supervisorId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut] 
        public async Task<IActionResult> Update([FromBody] OperationsSupervisorUpdateDto dto)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _updateOperationsSupervisor.ExecuteAsync(dto, businessId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long supervisorId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId(); 

            var result = await _deleteOperationsSupervisor.ExecuteAsync(supervisorId, userId, businessId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}

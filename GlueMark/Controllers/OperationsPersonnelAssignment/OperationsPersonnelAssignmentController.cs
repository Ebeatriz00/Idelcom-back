using Application.DTOs.OperationsPersonnelAssignment;
using Application.UseCases.OperationsPersonnelAssignment;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Idelcom.Controllers.OperationsPersonnelAssignment
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OperationsPersonnelAssignmentController(
        CreateOperationsPersonnelAssignment createUseCase,
        GetAllOperationsPersonnelAssignment getAllUseCase,
        GetByIdOperationsPersonnelAssignment getByIdUseCase,
        UpdateOperationsPersonnelAssignment updateUseCase,
        DeleteOperationsPersonnelAssignment deleteUseCase
    ) : BaseController
    {
        private readonly CreateOperationsPersonnelAssignment _createUseCase = createUseCase;
        private readonly GetAllOperationsPersonnelAssignment _getAllUseCase = getAllUseCase;
        private readonly GetByIdOperationsPersonnelAssignment _getByIdUseCase = getByIdUseCase;
        private readonly UpdateOperationsPersonnelAssignment _updateUseCase = updateUseCase;
        private readonly DeleteOperationsPersonnelAssignment _deleteUseCase = deleteUseCase;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OperationsPersonnelAssignmentCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createUseCase.ExecuteAsync(dto, businessId, userId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllUseCase.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] long assignmentId)
        {
            var result = await _getByIdUseCase.ExecuteAsync(assignmentId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] OperationsPersonnelAssignmentUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updateUseCase.ExecuteAsync(dto, businessId, userId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long assignmentId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _deleteUseCase.ExecuteAsync(assignmentId, userId, businessId);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}

using Application.UseCases.Operations.AssignmentStatus;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AssignmentStatusController(
        GetSelectAssignmentStatus getAllUseCase,
        GetByIdAssignmentStatus getByIdUseCase) : BaseController
    {
        private readonly GetSelectAssignmentStatus _getAllUseCase = getAllUseCase;
        private readonly GetByIdAssignmentStatus _getByIdUseCase = getByIdUseCase;

        [HttpGet]
        public async Task<IActionResult> GetSelect(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllUseCase.ExecuteAsync(businessId, page, pageSize, search);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] long assignmentStatusId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getByIdUseCase.ExecuteAsync(assignmentStatusId, businessId);
            return Ok(result);
        }
    }
}

using Application.UseCases.Operations.MovementStatus;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MovementStatusController(
        GetSelectMovementStatus getSelect,
        GetByIdMovementStatus getById
        ) : BaseController
    {
        private readonly GetSelectMovementStatus _getAllUseCase = getSelect;
        private readonly GetByIdMovementStatus _getByIdUseCase = getById;

        [HttpGet]
        public async Task<IActionResult> GetSelect(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? search)
        {
            var business = GetCurrentBusinessId();
            var result = await _getAllUseCase.ExecuteAsync(business, page, pageSize, search);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] long movementStatusId)
        {
            var business = GetCurrentBusinessId();
            var result = await _getByIdUseCase.ExacuteAsync(movementStatusId, business);
            return Ok(result);
        }
    }
}

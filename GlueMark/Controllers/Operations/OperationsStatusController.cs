using Application.UseCases.Operations.OperationsStatus;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OperationsStatusController(GetSelectOperationsStatus getSelect, GetByIdOperationsStatus getById) : BaseController
    {
        private readonly GetSelectOperationsStatus _getSelect = getSelect;
        private readonly GetByIdOperationsStatus _getById = getById;

        [HttpGet]
        public async Task<IActionResult> GetSelect(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var businessId = GetCurrentBusinessId();

            var result = await _getSelect.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetByID([FromQuery] long operationsStatusId)
        {
            var businessId = GetCurrentBusinessId();

            var result = await _getById.ExecuteAsync(operationsStatusId, businessId);
            return Ok(result);
        }
    }
}

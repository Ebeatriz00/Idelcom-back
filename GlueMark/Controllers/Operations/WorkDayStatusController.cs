using Application.UseCases.Operations.WorkDayStatus;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WorkDayStatusController(GetSelectWorkDayStatus getSelectWorkDayStatus,
                                         GetByIdWorkDayStatus getByIdWorkDayStatus) : BaseController
    {
        private readonly GetSelectWorkDayStatus _getSelectWorkDayStatus = getSelectWorkDayStatus;
        private readonly GetByIdWorkDayStatus _getByIdWorkDayStatus = getByIdWorkDayStatus;

        [HttpGet]
        public async Task<IActionResult> GetSelect(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectWorkDayStatus.ExecuteAsync(businessId, page, pageSize, search);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] long WorkdayStatusId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getByIdWorkDayStatus.ExecuteAsync(WorkdayStatusId, businessId);
            return Ok(result);
        }
    }
}

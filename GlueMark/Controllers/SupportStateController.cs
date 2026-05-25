using Application.UseCases.SupportState;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.General
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SupportStateController(
        GetSelectSupportState getSelectSupportState,
        GetByIdSupportState getByIdSupportState) : BaseController
    {
        private readonly GetSelectSupportState _getSelectSupportState = getSelectSupportState;
        private readonly GetByIdSupportState _getByIdSupportState = getByIdSupportState;

        [HttpGet]
        public async Task<IActionResult> Select(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectSupportState.ExecuteAsync(businessId, page, pageSize, search);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] int supportStateId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getByIdSupportState.ExecuteAsync(supportStateId, businessId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}

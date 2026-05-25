using Application.UseCases.Ssoma;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Ssoma
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SsomaRoleController(GetSelectSsomaRole getSelectUseCase) : BaseController
    {
        private readonly GetSelectSsomaRole _getSelectUseCase = getSelectUseCase;

        [HttpGet]
        public async Task<IActionResult> GetSelect(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();

            var result = await _getSelectUseCase.ExecuteAsync(businessId, page, pageSize, search);
            return Ok(result);
        }
    }
}

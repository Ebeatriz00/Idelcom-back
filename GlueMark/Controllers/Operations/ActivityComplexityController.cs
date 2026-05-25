using Application.UseCases.Operations.ActivityComplexity;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ActivityComplexityController(GetSelectActivityComplexity getSelectUseCase) : BaseController
    {
        private readonly GetSelectActivityComplexity _getSelectUseCase = getSelectUseCase;

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

using Application.UseCases.AccountLevel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountLevelController : ControllerBase
    {
        private readonly GetSelectAccountLevel _getSelectAccountLevel;
        public AccountLevelController(GetSelectAccountLevel getSelectAccountLevel)
        {
            _getSelectAccountLevel = getSelectAccountLevel;
        }
        [HttpGet]
        [Route("AccountLevelSelect")]
        public async Task<IActionResult> GetAccountLevelSelect([FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectAccountLevel.ExecuteAsync(search, page, pageSize);
            return Ok(result);
        }
    }
}

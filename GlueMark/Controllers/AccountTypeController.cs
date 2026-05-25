using Application.UseCases.AccountType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountTypeController : ControllerBase
    {
        private readonly GetSelectAccountType _getSelectAccountType;
        public AccountTypeController(GetSelectAccountType getSelectAccountType)
        {
            _getSelectAccountType = getSelectAccountType;
        }
        [HttpGet]
        [Route("AccountTypeSelect")]
        public async Task<IActionResult> GetAccountTypeSelect([FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectAccountType.ExecuteAsync(search, page, pageSize);
            return Ok(result);
        }
    }
}

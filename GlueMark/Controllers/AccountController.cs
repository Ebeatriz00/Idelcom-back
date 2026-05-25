using Application.DTOs.Account;
using Application.UseCases.Account;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    namespace Idelcom.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class AccountController : ControllerBase
        {
            private readonly CreateAccount _createAccount;
            private readonly GetAllAccount _getAllAccount;
            private readonly GetAccountById _getAccountById;
            private readonly UpdateAccount _updateAccount;
            private readonly PatchAccountStatus _patchAccountStatus;
            private readonly GetSelectAccount _getSelectAccount;

            public AccountController(
                CreateAccount createAccount,
                GetAllAccount getAllAccount,
                GetAccountById getAccountById,
                UpdateAccount updateAccount,
                PatchAccountStatus patchAccountStatus,
                GetSelectAccount getSelectAccount)
            {
                _createAccount = createAccount;
                _getAllAccount = getAllAccount;
                _getAccountById = getAccountById;
                _updateAccount = updateAccount;
                _patchAccountStatus = patchAccountStatus;
                _getSelectAccount = getSelectAccount;
            }

            [HttpPost]
            [Route("AccountCreate")]
            public async Task<IActionResult> Create([FromBody] AccountCreateDto dto)
            {
                var result = await _createAccount.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpGet]
            [Route("AccountList")]
            public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
            {
                var result = await _getAllAccount.ExecuteAsync(business_id, search, page, pageSize, usersBy);
                if (result == null || !result.Items.Any())
                    return NotFound(new { message = "No se encontraron cuentas." });

                Response.Headers["X-Total-Count"] = result.Total.ToString();
                Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

                return Ok(result);
            }

            [HttpGet]
            [Route("AccountSelect")]
            public async Task<IActionResult> AccountSelect(
                [FromQuery] long business_id,
                [FromQuery] string? search = null,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20)
            {
                var result = await _getSelectAccount.ExecuteAsync(business_id, search, page, pageSize);
                return Ok(result);
            }

            [HttpGet]
            [Route("AccountById")]
            public async Task<IActionResult> GetById([FromQuery] long accountId)
            {
                var result = await _getAccountById.ExecuteAsync(accountId);
                if (result == null)
                    return NotFound(new { message = "No se encontro la cuenta." });
                return Ok(result);
            }

            [HttpPut]
            [Route("AccountUpdate")]
            public async Task<IActionResult> Update([FromBody] AccountUpdateDto dto)
            {
                var result = await _updateAccount.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpPatch]
            [Route("AccountStatus")]
            public async Task<IActionResult> PatchStatus([FromBody] AccountStatusToggleDto dto)
            {
                var result = await _patchAccountStatus.ExecuteAsync(dto);
                return Ok(result);
            }
        }
    }
}

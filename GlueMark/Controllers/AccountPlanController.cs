using Application.DTOs.AccountPlan;
using Application.UseCases.AccountPlan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountPlanController : ControllerBase
    {
        private readonly CreateAccountPlan _createAccountPlan;
        private readonly GetAllAccountPlan _getAllAccountPlan;
        private readonly GetByIdAccountPlan _getByIdAccountPlan;
        private readonly UpdateAccountPlan _updateAccountPlan;
        private readonly PatchAccountPlan _patchAccountPlanStatus;
        private readonly GetSelectAccountPlan _getSelectAccountPlan;
        public AccountPlanController(CreateAccountPlan createAccountPlan, GetAllAccountPlan getAllAccountPlan, GetByIdAccountPlan getByIdAccountPlan, UpdateAccountPlan updateAccountPlan, PatchAccountPlan patchAccountPlanStatus, GetSelectAccountPlan getSelectAccountPlan)
        {
            _createAccountPlan = createAccountPlan;
            _getAllAccountPlan = getAllAccountPlan;
            _getByIdAccountPlan = getByIdAccountPlan;
            _updateAccountPlan = updateAccountPlan;
            _patchAccountPlanStatus = patchAccountPlanStatus;
            _getSelectAccountPlan = getSelectAccountPlan;
        }

        [HttpPost]
        [Route("AccountPlanCreate")]
        public async Task<IActionResult> Create([FromBody] AccountPlanCreateDto dto)
        {
            var result = await _createAccountPlan.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpGet]
        [Route("AccountPlanList")]
        public async Task<IActionResult> GetList([FromQuery] int businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllAccountPlan.ExecuteAsync(businessId, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron planes de cuenta." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }
        [HttpGet]
        [Route("AccountPlanSelect")]
        public async Task<IActionResult> AccountPlanSelect(
            [FromQuery] long businessId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectAccountPlan.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }
        [HttpGet]
        [Route("AccountPlanById")]
        public async Task<IActionResult> GetById([FromQuery] int accountPlanId)
        {
            var result = await _getByIdAccountPlan.ExecuteAsync(accountPlanId);
            if (result == null)
                return NotFound(new { message = "No se encontro el plan de cuenta." });
            return Ok(result);
        }
        [HttpPut]
        [Route("AccountPlanUpdate")]
        public async Task<IActionResult> Update([FromBody] AccountPlanUpdateDto dto)
        {
            var result = await _updateAccountPlan.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPatch]
        [Route("AccountPlanStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] AccountPlanStatusToggleDto dto)
        {
            var result = await _patchAccountPlanStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

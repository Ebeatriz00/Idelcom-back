using Application.UseCases.PayableStatus;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayableStatusController : BaseController
    {
        private readonly GetSelectPayableStatus _getSelectPayableStatus;

        public PayableStatusController(GetSelectPayableStatus getSelectPayableStatus)
        {
            _getSelectPayableStatus = getSelectPayableStatus;
        }

        [HttpGet]
        [Route("PayableStatusSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectPayableStatus.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }
    }
}

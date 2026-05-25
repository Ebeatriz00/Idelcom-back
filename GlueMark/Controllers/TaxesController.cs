using Application.UseCases.Taxes;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxesController : BaseController
    {
        private readonly GetSelectTaxes _getSelectTaxes;

        public TaxesController(GetSelectTaxes getSelectTaxes)
        {
            _getSelectTaxes = getSelectTaxes;
        }

        [HttpGet]
        [Route("TaxesSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectTaxes.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }
    }
}

using Application.UseCases.SupplierInvoiceStatus;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierInvoiceStatusController : BaseController
    {
        private readonly GetSelectSupplierInvoiceStatus _getSelectSupplierInvoiceStatus;

        public SupplierInvoiceStatusController(GetSelectSupplierInvoiceStatus getSelectSupplierInvoiceStatus)
        {
            _getSelectSupplierInvoiceStatus = getSelectSupplierInvoiceStatus;
        }

        [HttpGet]
        [Route("SupplierInvoiceStatusSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectSupplierInvoiceStatus.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }
    }
}

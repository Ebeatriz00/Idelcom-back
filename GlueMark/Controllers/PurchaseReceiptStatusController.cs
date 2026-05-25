using Application.UseCases.PurchaseReceiptStatus;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseReceiptStatusController : BaseController
    {
        private readonly GetSelectPurchaseReceiptStatus _getSelectPurchaseReceiptStatus;

        public PurchaseReceiptStatusController(GetSelectPurchaseReceiptStatus getSelectPurchaseReceiptStatus)
        {
            _getSelectPurchaseReceiptStatus = getSelectPurchaseReceiptStatus;
        }

        [HttpGet]
        [Route("PurchaseReceiptStatusSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectPurchaseReceiptStatus.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }
    }
}

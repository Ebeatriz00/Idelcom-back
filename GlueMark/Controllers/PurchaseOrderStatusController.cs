using Application.UseCases.PurchaseOrderStatus;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderStatusController : BaseController
    {
        private readonly GetSelectPurchaseOrderStatus _getSelectPurchaseOrderStatus;

        public PurchaseOrderStatusController(GetSelectPurchaseOrderStatus getSelectPurchaseOrderStatus)
        {
            _getSelectPurchaseOrderStatus = getSelectPurchaseOrderStatus;
        }

        [HttpGet]
        [Route("PurchaseOrderStatusSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectPurchaseOrderStatus.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }
    }
}

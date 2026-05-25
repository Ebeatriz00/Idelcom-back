using Application.UseCases.PurchaseOrderDetailStatus;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderDetailStatusController : BaseController
    {
        private readonly GetSelectPurchaseOrderDetailStatus _getSelectPurchaseOrderDetailStatus;

        public PurchaseOrderDetailStatusController(GetSelectPurchaseOrderDetailStatus getSelectPurchaseOrderDetailStatus)
        {
            _getSelectPurchaseOrderDetailStatus = getSelectPurchaseOrderDetailStatus;
        }

        [HttpGet]
        [Route("PurchaseOrderDetailStatusSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectPurchaseOrderDetailStatus.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }
    }
}

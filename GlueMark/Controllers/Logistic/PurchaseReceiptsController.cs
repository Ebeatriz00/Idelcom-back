using Application.DTOs.PurchaseReceipt;
using Application.UseCases.PurchaseReceipt;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Logistic
{
    [Route("api/purchase-receipts")]
    [ApiController]
    public class PurchaseReceiptsController(
        CreatePurchaseReceiptUseCase createPurchaseReceipt,
        ListPurchaseReceiptUseCase listPurchaseReceipt,
        GetPurchaseReceiptByIdUseCase getPurchaseReceiptById,
        VoidPurchaseReceiptUseCase voidPurchaseReceipt,
        RegularizePurchaseReceiptWithPurchaseOrderUseCase regularizePurchaseReceipt) : BaseController
    {
        private readonly CreatePurchaseReceiptUseCase _createPurchaseReceipt = createPurchaseReceipt;
        private readonly ListPurchaseReceiptUseCase _listPurchaseReceipt = listPurchaseReceipt;
        private readonly GetPurchaseReceiptByIdUseCase _getPurchaseReceiptById = getPurchaseReceiptById;
        private readonly VoidPurchaseReceiptUseCase _voidPurchaseReceipt = voidPurchaseReceipt;
        private readonly RegularizePurchaseReceiptWithPurchaseOrderUseCase _regularizePurchaseReceipt = regularizePurchaseReceipt;

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] PurchaseReceiptCreateDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createPurchaseReceipt.ExecuteAsync(request, userId, businessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> List([FromQuery] PurchaseReceiptListFilterDto filter)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _listPurchaseReceipt.ExecuteAsync(filter, businessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("get/{purchaseReceiptId:long}")]
        public async Task<IActionResult> GetById(long purchaseReceiptId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getPurchaseReceiptById.ExecuteAsync(businessId, purchaseReceiptId);
            return Ok(result);
        }

        [HttpPatch]
        [Route("void/{purchaseReceiptId:long}")]
        public async Task<IActionResult> Void(long purchaseReceiptId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _voidPurchaseReceipt.ExecuteAsync(businessId, purchaseReceiptId, userId);
            return Ok(result);
        }

        [HttpPost]
        [Route("regularize-purchase-order")]
        public async Task<IActionResult> RegularizeWithPurchaseOrder([FromBody] PurchaseReceiptRegularizeDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _regularizePurchaseReceipt.ExecuteAsync(request, userId, businessId);
            return Ok(result);
        }
    }
}

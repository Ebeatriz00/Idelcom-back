using Application.DTOs.PurchaseOrder;
using Application.UseCases.PurchaseOrder;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Logistic
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController(
        RegisterPurchaseOrder registerPurchaseOrder,
        ListPurchaseOrder listPurchaseOrder,
        GetPurchaseOrderById getPurchaseOrderById,
        UpdatePurchaseOrder updatePurchaseOrder,
        ApprovePurchaseOrder approvePurchaseOrder,
        CancelPurchaseOrder cancelPurchaseOrder,
        AttachPurchaseOrderInvoice attachPurchaseOrderInvoice,
        CreatePurchaseOrderFromInvoice createPurchaseOrderFromInvoice,
        GeneratePurchaseOrderPdf generatePurchaseOrderPdf,
        SendPurchaseOrderForApproval sendPurchaseOrderForApproval) : BaseController
    {
        private readonly RegisterPurchaseOrder _registerPurchaseOrder = registerPurchaseOrder;
        private readonly ListPurchaseOrder _listPurchaseOrder = listPurchaseOrder;
        private readonly GetPurchaseOrderById _getPurchaseOrderById = getPurchaseOrderById;
        private readonly UpdatePurchaseOrder _updatePurchaseOrder = updatePurchaseOrder;
        private readonly ApprovePurchaseOrder _approvePurchaseOrder = approvePurchaseOrder;
        private readonly CancelPurchaseOrder _cancelPurchaseOrder = cancelPurchaseOrder;
        private readonly AttachPurchaseOrderInvoice _attachPurchaseOrderInvoice = attachPurchaseOrderInvoice;
        private readonly CreatePurchaseOrderFromInvoice _createPurchaseOrderFromInvoice = createPurchaseOrderFromInvoice;
        private readonly GeneratePurchaseOrderPdf _generatePurchaseOrderPdf = generatePurchaseOrderPdf;
        private readonly SendPurchaseOrderForApproval _sendPurchaseOrderForApproval = sendPurchaseOrderForApproval;

        [HttpPost]
        [Route("PurchaseOrderCreate")]
        public async Task<IActionResult> Register([FromBody] PurchaseOrderCreateDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _registerPurchaseOrder.ExecuteAsync(request, userId, businessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllPurchaseOrder")]
        public async Task<IActionResult> List([FromQuery] PurchaseOrderListFilterDto filter)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _listPurchaseOrder.ExecuteAsync(filter, businessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetByIdPurchaseOrder")]
        public async Task<IActionResult> GetById(long id)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getPurchaseOrderById.ExecuteAsync(businessId, id);
            return Ok(result);
        }

        [HttpPut]
        [Route("PurchaseOrderUpdate")]
        public async Task<IActionResult> Update([FromBody] PurchaseOrderUpdateDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updatePurchaseOrder.ExecuteAsync(request, userId, businessId);
            return Ok(result);
        }

        [HttpPost]
        [Route("PurchaseOrderApprove")]
        public async Task<IActionResult> Approve([FromBody] PurchaseOrderApproveDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _approvePurchaseOrder.ExecuteAsync(request, userId, businessId);
            return Ok(result);
        }

        [HttpPost]
        [Route("PurchaseOrderCancel")]
        public async Task<IActionResult> Cancel([FromBody] PurchaseOrderCancelDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _cancelPurchaseOrder.ExecuteAsync(request, userId, businessId);
            return Ok(result);
        }

        [HttpPost]
        [Route("AttachInvoice")]
        public async Task<IActionResult> AttachInvoice([FromBody] PurchaseOrderAttachInvoiceDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _attachPurchaseOrderInvoice.ExecuteAsync(request, userId, businessId);
            return Ok(result);
        }

        [HttpPost]
        [Route("CreateFromInvoice")]
        public async Task<IActionResult> CreateFromInvoice([FromBody] PurchaseOrderCreateFromInvoiceDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createPurchaseOrderFromInvoice.ExecuteAsync(request, userId, businessId);
            return Ok(result);
        }

        [HttpPost]
        [Route("SendForApproval")]
        public async Task<IActionResult> SendForApproval([FromBody] PurchaseOrderSendForApprovalDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _sendPurchaseOrderForApproval.ExecuteAsync(request, userId, businessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("PrintPdf")]
        public async Task<IActionResult> PrintPdf(long id)
        {
            var businessId = GetCurrentBusinessId();
            var (stream, orderNumber) = await _generatePurchaseOrderPdf.ExecuteAsync(businessId, id);
            stream.Position = 0;
            return File(stream, "application/pdf", $"{orderNumber}.pdf");
        }
    }
}

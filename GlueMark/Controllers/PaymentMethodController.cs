using Application.DTOs.PaymentMethod;
using Application.UseCases.PaymentMethod;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly CreatePaymentMethod _createPaymentMethod;
        private readonly GetAllPaymentMethods _getAllPaymentMethods;
        private readonly GetPaymentMethodById _getPaymentMethodById;
        private readonly UpdatePaymentMethod _updatePaymentMethod;
        private readonly PatchPaymentMethodStatus _patchPaymentMethodStatus;
        private readonly GetSelectPaymentMethod _getSelectPaymentMethod;

        public PaymentMethodController(
            CreatePaymentMethod createPaymentMethod,
            GetAllPaymentMethods getAllPaymentMethods,
            GetPaymentMethodById getPaymentMethodById,
            UpdatePaymentMethod updatePaymentMethod,
            PatchPaymentMethodStatus patchPaymentMethodStatus,
            GetSelectPaymentMethod getSelectPaymentMethod)
        {
            _createPaymentMethod = createPaymentMethod;
            _getAllPaymentMethods = getAllPaymentMethods;
            _getPaymentMethodById = getPaymentMethodById;
            _updatePaymentMethod = updatePaymentMethod;
            _patchPaymentMethodStatus = patchPaymentMethodStatus;
            _getSelectPaymentMethod = getSelectPaymentMethod;
        }

        [HttpPost]
        [Route("PaymentMethodCreate")]
        public async Task<IActionResult> Create([FromBody] PaymentMethodCreateDto dto)
        {
            var result = await _createPaymentMethod.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("PaymentMethodList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllPaymentMethods.ExecuteAsync(business_id, search, page, pageSize);

            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron métodos de pago." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("PaymentMethodSelect")]
        public async Task<IActionResult> PaymentMethodSelect(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectPaymentMethod.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("PaymentMethodById")]
        public async Task<IActionResult> GetById([FromQuery] long paymentMethodId)
        {
            var result = await _getPaymentMethodById.ExecuteAsync(paymentMethodId);

            if (result == null)
                return NotFound(new { message = "No se encontro el método de pago." });

            return Ok(result);
        }

        [HttpPut]
        [Route("PaymentMethodUpdate")]
        public async Task<IActionResult> Update([FromBody] PaymentMethodUpdateDto dto)
        {
            var result = await _updatePaymentMethod.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("PaymentMethodStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] PaymentMethodStatusToggleDto dto)
        {
            var result = await _patchPaymentMethodStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

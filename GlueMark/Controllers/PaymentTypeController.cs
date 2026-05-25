using Application.DTOs.PaymentType;
using Application.UseCases.PaymentType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTypeController : ControllerBase
    {
        private readonly CreatePaymentType _createPaymentType;
        private readonly GetAllPaymentType _getAllPaymentType;
        private readonly GetByIdPaymentType _getByIdPaymentType;
        private readonly UpdatePaymentType _updatePaymentType;
        private readonly PatchPaymentType _patchPaymentTypeStatus;
        private readonly GetSelectPaymentType _getSelectPaymentType;

        public PaymentTypeController(CreatePaymentType createPaymentType, GetAllPaymentType getAllPaymentType, GetByIdPaymentType getByIdPaymentType, UpdatePaymentType updatePaymentType, PatchPaymentType patchPaymentTypeStatus, GetSelectPaymentType getSelectPaymentType)
        {
            _createPaymentType = createPaymentType;
            _getAllPaymentType = getAllPaymentType;
            _getByIdPaymentType = getByIdPaymentType;
            _updatePaymentType = updatePaymentType;
            _patchPaymentTypeStatus = patchPaymentTypeStatus;
            _getSelectPaymentType = getSelectPaymentType;
        }
        [HttpPost]
        [Route("PaymentTypeCreate")]
        public async Task<IActionResult> Create([FromBody] PaymentTypeCreateDto dto)
        {
            var result = await _createPaymentType.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpGet]
        [Route("PaymentTypeList")]
        public async Task<IActionResult> GetList([FromQuery] int businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllPaymentType.ExecuteAsync(businessId, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de pagos." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }
        [HttpGet]
        [Route("PaymentTypeSelect")]
        public async Task<IActionResult> PaymentTypeSelect(
            [FromQuery] long businessId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectPaymentType.ExecuteAsync(businessId, search, page, pageSize);
           return Ok(result);


        }
        [HttpGet]
        [Route("PaymentTypeById")]
        public async Task<IActionResult> GetById([FromQuery] int paymentTypeId)
        {
            var result = await _getByIdPaymentType.ExecuteAsync(paymentTypeId);
            if (result == null)
                return NotFound(new { message = "Tipo de pago no encontrado." });
            return Ok(result);
        }
        [HttpPut]
        [Route("PaymentTypeUpdate")]
        public async Task<IActionResult> Update([FromBody] PaymentTypeUpdateDto dto)
        {
            var result = await _updatePaymentType.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPatch]
        [Route("PaymentTypeStatus")]
        public async Task<IActionResult> Patch([FromBody] PaymentTypeStatusToggleDto dto)
        {
            var result = await _patchPaymentTypeStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

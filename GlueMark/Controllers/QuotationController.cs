using Application.DTOs.Quotation;
using Application.UseCases.Quotation;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationController : ControllerBase
    {
        private readonly GetAllSalesQuotation _getAllSalesQuotation;
        private readonly GetAllSalesQuotationVer _getAllSalesQuotationVer;
        private readonly GetDetailSalesQuotation _getDetailSalesQuotation;
        private readonly ValidateQuotationExcel _validateQuotationExcel;
        private readonly ILinkTokenService _linkToken;

        public QuotationController(
            GetAllSalesQuotation getAllSalesQuotation,
            GetAllSalesQuotationVer getAllSalesQuotationVer,
            ILinkTokenService linkToken,
            GetDetailSalesQuotation getDetailSalesQuotation,
            ValidateQuotationExcel validateQuotationExcel)
        {
            _getAllSalesQuotation = getAllSalesQuotation;
            _getAllSalesQuotationVer = getAllSalesQuotationVer;
            _linkToken = linkToken;
            _getDetailSalesQuotation = getDetailSalesQuotation;
            _validateQuotationExcel = validateQuotationExcel;
        }

        [HttpGet]
        [Route("QuotationList")]
        public async Task<IActionResult> GetQuotationList([FromQuery] long businessId, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersId = null, [FromQuery] string? verDesc = null, [FromQuery] long? workerId = null)
        {
            var result = await _getAllSalesQuotation.ExecuteAsync(businessId, search, page, pageSize, usersId, verDesc, workerId);
            return Ok(result);
        }

        [HttpGet]
        [Route("QuotationVerList")]
        public async Task<IActionResult> GetQuotationVerList([FromQuery] string quotationId, [FromQuery] long businessId, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? verDesc = null, [FromQuery] long? workerId = null, [FromQuery] long? workerResponsibles = null)
        {
            try
            {


                if (!_linkToken.TryValidate(quotationId, out var entity, out var resourceId))
                    return Unauthorized("Token inválido o expirado.");
                if (entity != "quotation" && entity != "opportunity")
                    return BadRequest("Token no pertenece a cotizaciones.");

                quotationId = Convert.ToString(resourceId);

                var result = await _getAllSalesQuotationVer.ExecuteAsync(quotationId, businessId, search, page, pageSize, verDesc, workerId, workerResponsibles);
                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, new { message = "La solicitud fue cancelada." });
            }
        }
        [HttpGet]
        [Route("QuotationDetail")]
        public async Task<IActionResult> GetQuotationDetail([FromQuery] string quotationVerId, [FromQuery] long businessId, [FromQuery] string versionNo)
        {
            try
            {
                if (!_linkToken.TryValidate(quotationVerId, out var entity, out var resourceId))
                    return Unauthorized("Token inválido o expirado.");
                if (entity != "quotation-ver" )
                    return BadRequest("Token no pertenece a cotizaciones.");
                var result = await _getDetailSalesQuotation.ExecuteAsync(resourceId, businessId, versionNo);
                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, new { message = "La solicitud fue cancelada." });
            }
        }

        [HttpPost]
        [Route("validate-excel")]
        public IActionResult ValidateExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Ok(new QuotationExcelValidationResponseDto
                {
                    IsValid = false,
                    Errors = new List<QuotationExcelValidationError>
                    {
                        new() { Sheet = "", Message = "No se recibió ningún archivo." }
                    }
                });

            using var stream = file.OpenReadStream();
            var result = _validateQuotationExcel.Execute(stream);
            return Ok(result);
        }
    }
}

using Application.DTOs.ExchangeRate;
using Application.UseCases.ExchangeRate;
using Microsoft.AspNetCore.Mvc;

namespace GlueMark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly CreateExchangeRate _createExchangeRate;
        private readonly GetAllExchangeRate _getAllExchangeRates;
        private readonly GetByIdExchangeRate _getByIdExchangeRates;
        private readonly UpdateExchangeRate _updateExchangeRates;
        private readonly PatchExchangeRateStatus _patchExchangeRateStatus;
        private readonly GetSelectExchangeRate _getSelectExchangeRate;

        public ExchangeRateController(
            CreateExchangeRate createExchangeRate,
            GetAllExchangeRate getAllExchangeRates,
            GetByIdExchangeRate getByIdExchangeRates,
            UpdateExchangeRate updateExchangeRates,
            PatchExchangeRateStatus patchExchangeRateStatus,
            GetSelectExchangeRate getSelectExchangeRate)
        {
            _createExchangeRate = createExchangeRate;
            _getAllExchangeRates = getAllExchangeRates;
            _getByIdExchangeRates = getByIdExchangeRates;
            _updateExchangeRates = updateExchangeRates;
            _patchExchangeRateStatus = patchExchangeRateStatus;
            _getSelectExchangeRate = getSelectExchangeRate;
        }

        [HttpPost]
        [Route("ExchangeRateCreate")]
        public async Task<IActionResult> Create([FromBody] ExchangeRateCreateDto dto)
        {
            var result = await _createExchangeRate.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("ExchangeRateList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllExchangeRates.ExecuteAsync(business_id, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de cambio." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("ExchangeRateSelect")]
        public async Task<IActionResult> ExchangeRateSelect(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectExchangeRate.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("ExchangeRateIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long exchangeRateId)
        {
            var result = await _getByIdExchangeRates.ExecuteAsync(exchangeRateId);
            if (result == null)
                return NotFound(new { message = "No se encontró el tipo de cambio." });

            return Ok(result);
        }

        [HttpPut]
        [Route("ExchangeRateUpdate")]
        public async Task<IActionResult> Update([FromBody] ExchangeRateUpdateDto dto)
        {
            var result = await _updateExchangeRates.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("ExchangeRateStatus")]
        public async Task<IActionResult> Patch([FromBody] ExchangeRateStatusToggleDto dto)
        {
            var result = await _patchExchangeRateStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

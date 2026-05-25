using Application.DTOs.Currency;
using Application.UseCases.Area;
using Application.UseCases.Currency;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GlueMark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly CreateCurrency _createCurrency;
        private readonly GetAllCurrency _getAllCurrencys;
        private readonly GetByIdCurrency _getByIdCurrencys;
        private readonly UpdateCurrency _updateCurrencys;
        private readonly PatchCurrencyStatus _patchCurrencyStatus;
        private readonly GetSelectCurrency _getSelectCurrency;

        public CurrencyController(CreateCurrency createCurrency, GetAllCurrency getAllCurrencys, GetByIdCurrency getByIdCurrencys, UpdateCurrency updateCurrencys, PatchCurrencyStatus patchCurrencyStatus, GetSelectCurrency getSelectCurrency)
        {
            _createCurrency = createCurrency;
            _getAllCurrencys = getAllCurrencys;
            _getByIdCurrencys = getByIdCurrencys;
            _updateCurrencys = updateCurrencys;
            _patchCurrencyStatus = patchCurrencyStatus;
            _getSelectCurrency = getSelectCurrency;
        }

        [HttpPost]
        [Route("CurrencyCreate")]
        public async Task<IActionResult> Create([FromBody] CurrencyCreateDto dto)
        {
            var result = await _createCurrency.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpGet]
        [Route("CurrencyList")]
        public async Task<IActionResult> GetList([FromQuery] int business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllCurrencys.ExecuteAsync(business_id,search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de documentos." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }


        [HttpGet]
        [Route("CurrencySelect")]
        public async Task<IActionResult> CurrencySelect(
        [FromQuery] long business_id,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectCurrency.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("CurrencyIdList")]
        public async Task<IActionResult> GetListId([FromQuery] int CurrencyId)
        {
            var result = await _getByIdCurrencys.ExecuteAsync(CurrencyId);
            if (result == null)
                return NotFound(new { message = "No se encontraron tipos de documentos." });

            return Ok(result);
        }
        [HttpPut]
        [Route("CurrencyUpdate")]
        public async Task<IActionResult> Update([FromBody] CurrencyUpdateDto dto)
        {
            var result = await _updateCurrencys.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("CurrencyStatus")]
        public async Task<IActionResult> Patch([FromBody] CurrencyStatusToggleDto dto)
        {
            var result = await _patchCurrencyStatus.ExecuteAsync(dto);
            return Ok(result);
        }


    }
}

using Application.DTOs.TaxAffType;
using Application.UseCases.TaxAffType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxAffTypeController : ControllerBase
    {
        private readonly CreateTaxAffType _createTaxAffType;
        private readonly GetAllTaxAffType _getAllTaxAffType;
        private readonly GetByIdTaxAffType _getByIdTaxAffType;
        private readonly UpdateTaxAffType _updateTaxAffType;
        private readonly PatchTaxAffType _patchTaxAffTypeStatus;
        private readonly GetSelectTaxAffType _getSelectTaxAffType;
        public TaxAffTypeController(CreateTaxAffType createTaxAffType, GetAllTaxAffType getAllTaxAffType, GetByIdTaxAffType getByIdTaxAffType, UpdateTaxAffType updateTaxAffType, PatchTaxAffType patchTaxAffTypeStatus, GetSelectTaxAffType getSelectTaxAffType)
        {
            _createTaxAffType = createTaxAffType;
            _getAllTaxAffType = getAllTaxAffType;
            _getByIdTaxAffType = getByIdTaxAffType;
            _updateTaxAffType = updateTaxAffType;
            _patchTaxAffTypeStatus = patchTaxAffTypeStatus;
            _getSelectTaxAffType = getSelectTaxAffType;
        }
        [HttpPost]
        [Route("TaxAffTypeCreate")]
        public async Task<IActionResult> Create([FromBody] TaxAffTypeCreateDto dto)
        {
            var result = await _createTaxAffType.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpGet]
        [Route("TaxAffTypeList")]
        public async Task<IActionResult> GetList([FromQuery] int businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllTaxAffType.ExecuteAsync(businessId, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de afectacion tributarias." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }
        [HttpGet]
        [Route("TaxAffTypeSelect")]
        public async Task<IActionResult> TaxAffTypeSelect(
            [FromQuery] long businessId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectTaxAffType.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }
        [HttpGet]
        [Route("TaxAffTypeById")]
        public async Task<IActionResult> GetById([FromQuery] int taxAffTypeId)
        {
            var result = await _getByIdTaxAffType.ExecuteAsync(taxAffTypeId);
            if (result == null)
                return NotFound(new { message = "Tipo de afectacion tributaria no encontrado." });
            return Ok(result);
        }
        [HttpPut]
        [Route("TaxAffTypeUpdate")]
        public async Task<IActionResult> Update([FromBody] TaxAffTypeUpdateDto dto)
        {
            var result = await _updateTaxAffType.ExecuteAsync(dto);
           return Ok(result);
        }
        [HttpPatch]
        [Route("TaxAffTypeStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] TaxAffTypeStatusToggleDto dto)
        {
            var result = await _patchTaxAffTypeStatus.ExecuteAsync(dto);
           return Ok(result);
        }
    }
}

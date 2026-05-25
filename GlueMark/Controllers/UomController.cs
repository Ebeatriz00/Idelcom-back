using Application.DTOs.Uom;
using Application.UseCases.Uom;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace GlueMark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UomController : ControllerBase
    {
        private readonly CreateUom _createUom;
        private readonly GetAllUom _getAllUoms;
        private readonly GetByIdUom _getByIdUom;
        private readonly UpdateUom _updateUom;
        private readonly PatchUomStatus _patchUomStatus;
        private readonly GetSelectUom _getSelectUom;

        public UomController(
            CreateUom createUom,
            GetAllUom getAllUoms,
            GetByIdUom getByIdUom,
            UpdateUom updateUom,
            PatchUomStatus patchUomStatus,
            GetSelectUom getSelectUom)
        {
            _createUom = createUom;
            _getAllUoms = getAllUoms;
            _getByIdUom = getByIdUom;
            _updateUom = updateUom;
            _patchUomStatus = patchUomStatus;
            _getSelectUom = getSelectUom;
        }

        [HttpPost]
        [Route("UomCreate")]
        public async Task<IActionResult> Create([FromBody] UomCreateDto dto)
        {
            var result = await _createUom.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("UomList")]
        public async Task<IActionResult> GetList(
            [FromQuery] int business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10, 
            [FromQuery] long? usersBy = null)
        {
            var result = await _getAllUoms.ExecuteAsync(business_id, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron unidades de medida." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("UomSelect")]
        public async Task<IActionResult> GetSelect(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectUom.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("UomIdList")]
        public async Task<IActionResult> GetById([FromQuery] int UomId)
        {
            var result = await _getByIdUom.ExecuteAsync(UomId);
            if (result == null)
                return NotFound(new { message = "No se encontró la unidad de medida." });

            return Ok(result);
        }

        [HttpPut]
        [Route("UomUpdate")]
        public async Task<IActionResult> Update([FromBody] UomUpdateDto dto)
        {
            var result = await _updateUom.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("UomStatus")]
        public async Task<IActionResult> Patch([FromBody] UomStatusToggleDto dto)
        {
            var result = await _patchUomStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

using Application.DTOs.BusinessLine;
using Application.UseCases.BusinessLine;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessLineController : ControllerBase
    {
        private readonly CreateBusinessLine _createBusinessLine;
        private readonly GetAllBusinessLine _getAllBusinessLine;
        private readonly GetByIdBusinessLine _getByIdBusinessLine;
        private readonly UpdateBusinessLine _updateBusinessLine;
        private readonly PatchBusinessLine _patchBusinessLineStatus;
        private readonly GetSelectBusinessLine _getSelectBusinessLine;

        public BusinessLineController(CreateBusinessLine createBusinessLine, GetAllBusinessLine getAllBusinessLine, GetByIdBusinessLine getByIdBusinessLine, UpdateBusinessLine updateBusinessLine, PatchBusinessLine patchBusinessLineStatus, GetSelectBusinessLine getSelectBusinessLine)
        {
            _createBusinessLine = createBusinessLine;
            _getAllBusinessLine = getAllBusinessLine;
            _getByIdBusinessLine = getByIdBusinessLine;
            _updateBusinessLine = updateBusinessLine;
            _patchBusinessLineStatus = patchBusinessLineStatus;
            _getSelectBusinessLine = getSelectBusinessLine;
        }

        [HttpPost]
        [Route("BusinessLineCreate")]
        public async Task<IActionResult> Create([FromBody] BusinessLineCreateDto dto)
        {
            var result = await _createBusinessLine.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpGet]
        [Route("BusinessLineList")]
        public async Task<IActionResult> GetList([FromQuery] int businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllBusinessLine.ExecuteAsync(businessId, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de pagos." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }
        [HttpGet]
        [Route("BusinessLineSelect")]
        public async Task<IActionResult> BusinessLineSelect(
            [FromQuery] long businessId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectBusinessLine.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);


        }
        [HttpGet]
        [Route("BusinessLineById")]
        public async Task<IActionResult> GetById([FromQuery] int BusinessLineId)
        {
            var result = await _getByIdBusinessLine.ExecuteAsync(BusinessLineId);
            if (result == null)
                return NotFound(new { message = "Estado de oportunidades no encontrado." });
            return Ok(result);
        }
        [HttpPut]
        [Route("BusinessLineUpdate")]
        public async Task<IActionResult> Update([FromBody] BusinessLineUpdateDto dto)
        {
            var result = await _updateBusinessLine.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPatch]
        [Route("BusinessLineStatus")]
        public async Task<IActionResult> Patch([FromBody] BusinessLineStatusToggleDto dto)
        {
            var result = await _patchBusinessLineStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

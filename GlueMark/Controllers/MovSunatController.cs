using Application.DTOs.MovSunat;
using Application.UseCases.MovSunat;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovSunatController : ControllerBase
    {
        private readonly CreateMovSunat _createMovSunat;
        private readonly GetAllMovSunat _getAllMovSunat;
        private readonly GetMovSunatById _getMovSunatById;
        private readonly UpdateMovSunat _updateMovSunat;
        private readonly PatchMovSunatStatus _patchMovSunatStatus;
        private readonly GetSelectMovSunat _getSelectMovSunat;

        public MovSunatController(
            CreateMovSunat createMovSunat,
            GetAllMovSunat getAllMovSunat,
            GetMovSunatById getMovSunatById,
            UpdateMovSunat updateMovSunat,
            PatchMovSunatStatus patchMovSunatStatus,
            GetSelectMovSunat getSelectMovSunat)
        {
            _createMovSunat = createMovSunat;
            _getAllMovSunat = getAllMovSunat;
            _getMovSunatById = getMovSunatById;
            _updateMovSunat = updateMovSunat;
            _patchMovSunatStatus = patchMovSunatStatus;
            _getSelectMovSunat = getSelectMovSunat;
        }

        [HttpPost]
        [Route("MovSunatCreate")]
        public async Task<IActionResult> Create([FromBody] MovSunatCreateDto dto)
        {
            var result = await _createMovSunat.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("MovSunatList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllMovSunat.ExecuteAsync(business_id, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos SUNAT." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("MovSunatSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectMovSunat.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("MovSunatIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long movSunatId)
        {
            var result = await _getMovSunatById.ExecuteAsync(movSunatId);
            if (result == null)
                return NotFound(new { message = "No se encontró el tipo SUNAT." });

            return Ok(result);
        }

        [HttpPut]
        [Route("MovSunatUpdate")]
        public async Task<IActionResult> Update([FromBody] MovSunatUpdateDto dto)
        {
            var result = await _updateMovSunat.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("MovSunatStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] MovSunatStatusToggleDto dto)
        {
            var result = await _patchMovSunatStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
using Application.DTOs.MovOper;
using Application.UseCases.MovOper;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovOperController : ControllerBase
    {
        private readonly CreateMovOper _createMovOper;
        private readonly GetAllMovOper _getAllMovOper;
        private readonly GetMovOperById _getMovOperById;
        private readonly UpdateMovOper _updateMovOper;
        private readonly PatchMovOperStatus _patchMovOperStatus;
        private readonly GetSelectMovOper _getSelectMovOper;

        public MovOperController(
            CreateMovOper createMovOper,
            GetAllMovOper getAllMovOper,
            GetMovOperById getMovOperById,
            UpdateMovOper updateMovOper,
            PatchMovOperStatus patchMovOperStatus,
            GetSelectMovOper getSelectMovOper)
        {
            _createMovOper = createMovOper;
            _getAllMovOper = getAllMovOper;
            _getMovOperById = getMovOperById;
            _updateMovOper = updateMovOper;
            _patchMovOperStatus = patchMovOperStatus;
            _getSelectMovOper = getSelectMovOper;
        }

        [HttpPost]
        [Route("MovOperCreate")]
        public async Task<IActionResult> Create([FromBody] MovOperCreateDto dto)
        {
            var result = await _createMovOper.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("MovOperList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllMovOper.ExecuteAsync(business_id, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de operación." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("MovOperSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectMovOper.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("MovOperIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long movOperId)
        {
            var result = await _getMovOperById.ExecuteAsync(movOperId);
            if (result == null)
                return NotFound(new { message = "No se encontró el tipo de operación." });

            return Ok(result);
        }

        [HttpPut]
        [Route("MovOperUpdate")]
        public async Task<IActionResult> Update([FromBody] MovOperUpdateDto dto)
        {
            var result = await _updateMovOper.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("MovOperStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] MovOperStatusToggleDto dto)
        {
            var result = await _patchMovOperStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

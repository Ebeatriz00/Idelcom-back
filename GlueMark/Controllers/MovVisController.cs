using Application.DTOs.MovVis;
using Application.UseCases.MovVis;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovVisController : ControllerBase
    {
        private readonly CreateMovVis _createMovVis;
        private readonly GetAllMovVis _getAllMovVis;
        private readonly GetMovVisById _getMovVisById;
        private readonly UpdateMovVis _updateMovVis;
        private readonly PatchMovVisStatus _patchMovVisStatus;
        private readonly GetSelectMovVis _getSelectMovVis;

        public MovVisController(
            CreateMovVis createMovVis,
            GetAllMovVis getAllMovVis,
            GetMovVisById getMovVisById,
            UpdateMovVis updateMovVis,
            PatchMovVisStatus patchMovVisStatus,
            GetSelectMovVis getSelectMovVis)
        {
            _createMovVis = createMovVis;
            _getAllMovVis = getAllMovVis;
            _getMovVisById = getMovVisById;
            _updateMovVis = updateMovVis;
            _patchMovVisStatus = patchMovVisStatus;
            _getSelectMovVis = getSelectMovVis;
        }

        [HttpPost]
        [Route("MovVisCreate")]
        public async Task<IActionResult> Create([FromBody] MovVisCreateDto dto)
        {
            var result = await _createMovVis.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("MovVisList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllMovVis.ExecuteAsync(business_id, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron visibilidades de movimiento." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("MovVisSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectMovVis.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("MovVisIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long movVisId)
        {
            var result = await _getMovVisById.ExecuteAsync(movVisId);
            if (result == null)
                return NotFound(new { message = "No se encontró la visibilidad de movimiento." });

            return Ok(result);
        }

        [HttpPut]
        [Route("MovVisUpdate")]
        public async Task<IActionResult> Update([FromBody] MovVisUpdateDto dto)
        {
            var result = await _updateMovVis.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("MovVisStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] MovVisStatusToggleDto dto)
        {
            var result = await _patchMovVisStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

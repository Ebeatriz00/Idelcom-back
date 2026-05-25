using Application.DTOs.MovPer;
using Application.UseCases.MovPer;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovPerController : ControllerBase
    {
        private readonly CreateMovPer _createMovPer;
        private readonly GetAllMovPer _getAllMovPer;
        private readonly GetMovPerById _getMovPerById;
        private readonly UpdateMovPer _updateMovPer;
        private readonly PatchMovPerStatus _patchMovPerStatus;
        private readonly GetSelectMovPer _getSelectMovPer;

        public MovPerController(
            CreateMovPer createMovPer,
            GetAllMovPer getAllMovPer,
            GetMovPerById getMovPerById,
            UpdateMovPer updateMovPer,
            PatchMovPerStatus patchMovPerStatus,
            GetSelectMovPer getSelectMovPer)
        {
            _createMovPer = createMovPer;
            _getAllMovPer = getAllMovPer;
            _getMovPerById = getMovPerById;
            _updateMovPer = updateMovPer;
            _patchMovPerStatus = patchMovPerStatus;
            _getSelectMovPer = getSelectMovPer;
        }

        [HttpPost]
        [Route("MovPerCreate")]
        public async Task<IActionResult> Create([FromBody] MovPerCreateDto dto)
        {
            var result = await _createMovPer.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("MovPerList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllMovPer.ExecuteAsync(business_id, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron periodos de movimiento." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("MovPerSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectMovPer.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("MovPerIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long movPerId)
        {
            var result = await _getMovPerById.ExecuteAsync(movPerId);
            if (result == null)
                return NotFound(new { message = "No se encontró el periodo de movimiento." });

            return Ok(result);
        }

        [HttpPut]
        [Route("MovPerUpdate")]
        public async Task<IActionResult> Update([FromBody] MovPerUpdateDto dto)
        {
            var result = await _updateMovPer.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("MovPerStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] MovPerStatusToggleDto dto)
        {
            var result = await _patchMovPerStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

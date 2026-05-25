using Application.DTOs.MovClas;
using Application.UseCases.MovClas;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovClasController : ControllerBase
    {
        private readonly CreateMovClas _createMovClas;
        private readonly GetAllMovClas _getAllMovClas;
        private readonly GetMovClasById _getMovClasById;
        private readonly UpdateMovClas _updateMovClas;
        private readonly PatchMovClasStatus _patchMovClasStatus;
        private readonly GetSelectMovClas _getSelectMovClas;

        public MovClasController(
            CreateMovClas createMovClas,
            GetAllMovClas getAllMovClas,
            GetMovClasById getMovClasById,
            UpdateMovClas updateMovClas,
            PatchMovClasStatus patchMovClasStatus,
            GetSelectMovClas getSelectMovClas)
        {
            _createMovClas = createMovClas;
            _getAllMovClas = getAllMovClas;
            _getMovClasById = getMovClasById;
            _updateMovClas = updateMovClas;
            _patchMovClasStatus = patchMovClasStatus;
            _getSelectMovClas = getSelectMovClas;
        }

        [HttpPost]
        [Route("MovClasCreate")]
        public async Task<IActionResult> Create([FromBody] MovClasCreateDto dto)
        {
            var result = await _createMovClas.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("MovClasList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllMovClas.ExecuteAsync(business_id, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron clases de movimiento." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("MovClasSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectMovClas.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("MovClasIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long movClasId)
        {
            var result = await _getMovClasById.ExecuteAsync(movClasId);
            if (result == null)
                return NotFound(new { message = "No se encontró la clase de movimiento." });

            return Ok(result);
        }

        [HttpPut]
        [Route("MovClasUpdate")]
        public async Task<IActionResult> Update([FromBody] MovClasUpdateDto dto)
        {
            var result = await _updateMovClas.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("MovClasStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] MovClasStatusToggleDto dto)
        {
            var result = await _patchMovClasStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

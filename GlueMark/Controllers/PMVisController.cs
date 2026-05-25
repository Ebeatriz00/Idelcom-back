using Application.DTOs.PMVis;
using Application.UseCases.PMVis;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PMVisController : ControllerBase
    {
        private readonly CreatePMVis _createPMVis;
        private readonly GetAllPMVis _getAllPMVis;
        private readonly GetPMVisById _getPMVisById;
        private readonly UpdatePMVis _updatePMVis;
        private readonly PatchPMVisStatus _patchPMVisStatus;
        private readonly GetSelectPMVis _getSelectPMVis;

        public PMVisController(
            CreatePMVis createPMVis,
            GetAllPMVis getAllPMVis,
            GetPMVisById getPMVisById,
            UpdatePMVis updatePMVis,
            PatchPMVisStatus patchPMVisStatus,
            GetSelectPMVis getSelectPMVis)
        {
            _createPMVis = createPMVis;
            _getAllPMVis = getAllPMVis;
            _getPMVisById = getPMVisById;
            _updatePMVis = updatePMVis;
            _patchPMVisStatus = patchPMVisStatus;
            _getSelectPMVis = getSelectPMVis;
        }

        [HttpPost]
        [Route("PMVisCreate")]
        public async Task<IActionResult> Create([FromBody] PMVisCreateDto dto)
        {
            var result = await _createPMVis.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("PMVisList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllPMVis.ExecuteAsync(business_id, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron visitas." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("PMVisSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectPMVis.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("PMVisIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long pmVisId)
        {
            var result = await _getPMVisById.ExecuteAsync(pmVisId);
            if (result == null)
                return NotFound(new { message = "No se encontró la visita." });

            return Ok(result);
        }

        [HttpPut]
        [Route("PMVisUpdate")]
        public async Task<IActionResult> Update([FromBody] PMVisUpdateDto dto)
        {
            var result = await _updatePMVis.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("PMVisStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] PMVisStatusToggleDto dto)
        {
            var result = await _patchPMVisStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

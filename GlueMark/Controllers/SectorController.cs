using Application.DTOs.Sector;
using Application.UseCases.Sector;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectorController : ControllerBase
    {
        private readonly CreateSector _createSector;
        private readonly GetAllSector _getAllLeadsStaus;
        private readonly GetSectorById _getByIdLeadsStaus;
        private readonly UpdateSector _updateLeadsStaus;
        private readonly GetSelectSector _getSelectLeadsStaus;
        private readonly PatchSectorStatus _patchLeadsStaus;

        public SectorController(GetAllSector getAllLeadsStaus, GetSectorById getByIdLeadsStaus, UpdateSector updateLeadsStaus, GetSelectSector getSelectLeadsStaus, PatchSectorStatus patchLeadsStaus, CreateSector createSector)
        {

            _getAllLeadsStaus = getAllLeadsStaus;
            _getByIdLeadsStaus = getByIdLeadsStaus;
            _updateLeadsStaus = updateLeadsStaus;
            _getSelectLeadsStaus = getSelectLeadsStaus;
            _patchLeadsStaus = patchLeadsStaus;
            _createSector = createSector;
        }

        [HttpPost]
        [Route("SectorCreate")]
        public async Task<IActionResult> Create([FromBody] SectorCreateDto dto)
        {
            var result = await _createSector.ExecuteAsync(dto);
            return Ok(result);
        }


        [HttpGet]
        [Route("SectorList")]
        public async Task<IActionResult> GetList([FromQuery] long businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllLeadsStaus.ExecuteAsync(businessId, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron calificaciones de leads." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("SectorSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] long businessId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectLeadsStaus.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("SectorIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long SectorId)
        {
            var result = await _getByIdLeadsStaus.ExecuteAsync(SectorId);
            if (result == null)
                return NotFound(new { message = "No se encontró la calificación de lead." });

            return Ok(result);
        }

        [HttpPut]
        [Route("SectorUpdate")]
        public async Task<IActionResult> Update([FromBody] SectorUpdateDto dto)
        {
            var result = await _updateLeadsStaus.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("SectorStatus")]
        public async Task<IActionResult> PatchSources([FromBody] SectorStatusToggleDto dto)
        {
            var result = await _patchLeadsStaus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

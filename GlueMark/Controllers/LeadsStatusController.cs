
using Application.DTOs.LeadsStatus;
using Application.UseCases.LeadsStatus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsStatusController : ControllerBase
    {
        private readonly  CreateLeadsStatus _createLeadsStaus;
        private readonly GetAllLeadsStatus _getAllLeadsStaus;
        private readonly GetByIdLeadsStatus _getByIdLeadsStaus;
        private readonly UpdateLeadsStatus _updateLeadsStaus;
        private readonly GetSelectLeadsStatus _getSelectLeadsStaus;
        private readonly PatchLeadsStatus _patchLeadsStaus;

        public LeadsStatusController(CreateLeadsStatus createLeadsStaus, GetAllLeadsStatus getAllLeadsStaus, GetByIdLeadsStatus getByIdLeadsStaus, UpdateLeadsStatus updateLeadsStaus, GetSelectLeadsStatus getSelectLeadsStaus, PatchLeadsStatus patchLeadsStaus)
        {
            _createLeadsStaus = createLeadsStaus;
            _getAllLeadsStaus = getAllLeadsStaus;
            _getByIdLeadsStaus = getByIdLeadsStaus;
            _updateLeadsStaus = updateLeadsStaus;
            _getSelectLeadsStaus = getSelectLeadsStaus;
            _patchLeadsStaus = patchLeadsStaus;
        }

        [HttpPost]
        [Route("LeadsStatusCreate")]
        public async Task<IActionResult> Create([FromBody] LeadsStatusCreateDto dto)
        {
            var result = await _createLeadsStaus.ExecuteAsync(dto);
            return Ok(result);
        }


        [HttpGet]
        [Route("LeadsStatusList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllLeadsStaus.ExecuteAsync(business_id, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron calificaciones de leads." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("LeadsStatusSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectLeadsStaus.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("LeadsStatusIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long leadsStatusId)
        {
            var result = await _getByIdLeadsStaus.ExecuteAsync(leadsStatusId);
            if (result == null)
                return NotFound(new { message = "No se encontró la calificación de lead." });

            return Ok(result);
        }

        [HttpPut]
        [Route("LeadsStatusUpdate")]
        public async Task<IActionResult> Update([FromBody] LeadsStatusUpdateDto dto)
        {
            var result = await _updateLeadsStaus.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("LeadsStatusStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] LeadsStatusStatusToggleDto dto)
        {
            var result = await _patchLeadsStaus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

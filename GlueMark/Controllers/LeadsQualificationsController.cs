using Application.DTOs.LeadsQualifications;
using Application.UseCases.LeadsQualifications;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsQualificationsController : ControllerBase
    {
        private readonly CreateLeadsQualifications _createLeadsQualifications;
        private readonly GetAllLeadsQualifications _getAllLeadsQualifications;
        private readonly GetLeadsQualificationsById _getLeadsQualificationsById;
        private readonly UpdateLeadsQualifications _updateLeadsQualifications;
        private readonly PatchLeadsQualificationsStatus _patchLeadsQualificationsStatus;
        private readonly GetSelectLeadsQualifications _getSelectLeadsQualifications;

        public LeadsQualificationsController(
            CreateLeadsQualifications createLeadsQualifications,
            GetAllLeadsQualifications getAllLeadsQualifications,
            GetLeadsQualificationsById getLeadsQualificationsById,
            UpdateLeadsQualifications updateLeadsQualifications,
            PatchLeadsQualificationsStatus patchLeadsQualificationsStatus,
            GetSelectLeadsQualifications getSelectLeadsQualifications)
        {
            _createLeadsQualifications = createLeadsQualifications;
            _getAllLeadsQualifications = getAllLeadsQualifications;
            _getLeadsQualificationsById = getLeadsQualificationsById;
            _updateLeadsQualifications = updateLeadsQualifications;
            _patchLeadsQualificationsStatus = patchLeadsQualificationsStatus;
            _getSelectLeadsQualifications = getSelectLeadsQualifications;
        }

        [HttpPost]
        [Route("LeadsQualificationsCreate")]
        public async Task<IActionResult> Create([FromBody] LeadsQualificationsCreateDto dto)
        {
            var result = await _createLeadsQualifications.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("LeadsQualificationsList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllLeadsQualifications.ExecuteAsync(business_id, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron calificaciones de leads." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("LeadsQualificationsSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectLeadsQualifications.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("LeadsQualificationsIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long leadsQualificationsId)
        {
            var result = await _getLeadsQualificationsById.ExecuteAsync(leadsQualificationsId);
            if (result == null)
                return NotFound(new { message = "No se encontró la calificación de lead." });

            return Ok(result);
        }

        [HttpPut]
        [Route("LeadsQualificationsUpdate")]
        public async Task<IActionResult> Update([FromBody] LeadsQualificationsUpdateDto dto)
        {
            var result = await _updateLeadsQualifications.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("LeadsQualificationsStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] LeadsQualificationsStatusToggleDto dto)
        {
            var result = await _patchLeadsQualificationsStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

using Application.DTOs.ReasonRejection;
using Application.UseCases.ReasonRejection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReasonRejectionController : ControllerBase
    {
        private readonly CreateReasonRejection _createLeadsStaus;
        private readonly GetAllReasonRejection _getAllLeadsStaus;
        private readonly GetByIdReasonRejection _getByIdLeadsStaus;
        private readonly UpdateReasonRejection _updateLeadsStaus;
        private readonly GetSelectReasonRejection _getSelectLeadsStaus;
        private readonly PatchReasonRejection _patchLeadsStaus;

        public ReasonRejectionController(CreateReasonRejection createLeadsStaus, GetAllReasonRejection getAllLeadsStaus, GetByIdReasonRejection getByIdLeadsStaus, UpdateReasonRejection updateLeadsStaus, GetSelectReasonRejection getSelectLeadsStaus, PatchReasonRejection patchLeadsStaus)
        {
            _createLeadsStaus = createLeadsStaus;
            _getAllLeadsStaus = getAllLeadsStaus;
            _getByIdLeadsStaus = getByIdLeadsStaus;
            _updateLeadsStaus = updateLeadsStaus;
            _getSelectLeadsStaus = getSelectLeadsStaus;
            _patchLeadsStaus = patchLeadsStaus;
        }

        [HttpPost]
        [Route("ReasonRejectionCreate")]
        public async Task<IActionResult> Create([FromBody] ReasonRejectionCreateDto dto)
        {
            var result = await _createLeadsStaus.ExecuteAsync(dto);
            return Ok(result);
        }


        [HttpGet]
        [Route("ReasonRejectionList")]
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
        [Route("ReasonRejectionSelect")]
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
        [Route("ReasonRejectionIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long reasonRejectionId)
        {
            var result = await _getByIdLeadsStaus.ExecuteAsync(reasonRejectionId);
            if (result == null)
                return NotFound(new { message = "No se encontró la calificación de lead." });

            return Ok(result);
        }

        [HttpPut]
        [Route("ReasonRejectionUpdate")]
        public async Task<IActionResult> Update([FromBody] ReasonRejectionUpdateDto dto)
        {
            var result = await _updateLeadsStaus.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("ReasonRejectionStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] ReasonRejectionStatusToggleDto dto)
        {
            var result = await _patchLeadsStaus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

using Application.DTOs.PMCondition;
using Application.UseCases.PMCondition;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PMConditionController : ControllerBase
    {
        private readonly CreatePMCondition _createPMCondition;
        private readonly GetAllPMCondition _getAllPMCondition;
        private readonly GetPMConditionById _getPMConditionById;
        private readonly UpdatePMCondition _updatePMCondition;
        private readonly PatchPMConditionStatus _patchPMConditionStatus;
        private readonly GetSelectPMCondition _getSelectPMCondition;

        public PMConditionController(
            CreatePMCondition createPMCondition,
            GetAllPMCondition getAllPMCondition,
            GetPMConditionById getPMConditionById,
            UpdatePMCondition updatePMCondition,
            PatchPMConditionStatus patchPMConditionStatus,
            GetSelectPMCondition getSelectPMCondition)
        {
            _createPMCondition = createPMCondition;
            _getAllPMCondition = getAllPMCondition;
            _getPMConditionById = getPMConditionById;
            _updatePMCondition = updatePMCondition;
            _patchPMConditionStatus = patchPMConditionStatus;
            _getSelectPMCondition = getSelectPMCondition;
        }

        [HttpPost]
        [Route("PMConditionCreate")]
        public async Task<IActionResult> Create([FromBody] PMConditionCreateDto dto)
        {
            var result = await _createPMCondition.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("PMConditionList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllPMCondition.ExecuteAsync(business_id, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron condiciones de pago." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("PMConditionSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectPMCondition.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("PMConditionIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long pmConditionId)
        {
            var result = await _getPMConditionById.ExecuteAsync(pmConditionId);
            if (result == null)
                return NotFound(new { message = "No se encontró la condición de pago." });

            return Ok(result);
        }

        [HttpPut]
        [Route("PMConditionUpdate")]
        public async Task<IActionResult> Update([FromBody] PMConditionUpdateDto dto)
        {
            var result = await _updatePMCondition.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("PMConditionStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] PMConditionStatusToggleDto dto)
        {
            var result = await _patchPMConditionStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

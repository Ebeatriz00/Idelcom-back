using Application.DTOs.ConceptGroups;
using Application.UseCases.ConceptGroups;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConceptGroupsController : ControllerBase
    {
        private readonly CreateConceptGroups _createConceptGroups;
        private readonly GetAllConceptGroups _getAllConceptGroups;
        private readonly GetConceptGroupsById _getConceptGroupsById;
        private readonly UpdateConceptGroups _updateConceptGroups;
        private readonly PatchConceptGroupsStatus _patchConceptGroupsStatus;
        private readonly GetSelectConceptGroups _getSelectConceptGroups;

        public ConceptGroupsController(
            CreateConceptGroups createConceptGroups,
            GetAllConceptGroups getAllConceptGroups,
            GetConceptGroupsById getConceptGroupsById,
            UpdateConceptGroups updateConceptGroups,
            PatchConceptGroupsStatus patchConceptGroupsStatus,
            GetSelectConceptGroups getSelectConceptGroups)
        {
            _createConceptGroups = createConceptGroups;
            _getAllConceptGroups = getAllConceptGroups;
            _getConceptGroupsById = getConceptGroupsById;
            _updateConceptGroups = updateConceptGroups;
            _patchConceptGroupsStatus = patchConceptGroupsStatus;
            _getSelectConceptGroups = getSelectConceptGroups;
        }

        [HttpPost]
        [Route("ConceptGroupsCreate")]
        public async Task<IActionResult> Create([FromBody] ConceptGroupsCreateDto dto)
        {
            var result = await _createConceptGroups.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("ConceptGroupsList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllConceptGroups.ExecuteAsync(business_id, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron grupos de conceptos." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("ConceptGroupsSelect")]
        public async Task<IActionResult> ConceptGroupsSelect(
            [FromQuery] long businessid,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectConceptGroups.ExecuteAsync(businessid, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("ConceptGroupsIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long conceptGroupsId)
        {
            var result = await _getConceptGroupsById.ExecuteAsync(conceptGroupsId);
            if (result == null)
                return NotFound(new { message = "No se encontró el grupo de conceptos." });

            return Ok(result);
        }

        [HttpPut]
        [Route("ConceptGroupsUpdate")]
        public async Task<IActionResult> Update([FromBody] ConceptGroupsUpdateDto dto)
        {
            var result = await _updateConceptGroups.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("ConceptGroupsStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] ConceptGroupsStatusToggleDto dto)
        {
            var result = await _patchConceptGroupsStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}


using Application.DTOs.StateOpportunity;
using Application.UseCases.StateOpportunity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateOpportunityController : ControllerBase
    {
        private readonly CreateStateOpportunity _createStateOpportunity;
        private readonly GetAllStateOpportunity _getAllStateOpportunity;
        private readonly GetByIdStateOpportunity _getByIdStateOpportunity;
        private readonly UpdateStateOpportunity _updateStateOpportunity;
        private readonly PatchStateOpportunity _patchStateOpportunityStatus;
        private readonly GetSelectStateOpportunity _getSelectStateOpportunity;

        public StateOpportunityController(CreateStateOpportunity createStateOpportunity, GetAllStateOpportunity getAllStateOpportunity, GetByIdStateOpportunity getByIdStateOpportunity, UpdateStateOpportunity updateStateOpportunity, PatchStateOpportunity patchStateOpportunityStatus, GetSelectStateOpportunity getSelectStateOpportunity)
        {
            _createStateOpportunity = createStateOpportunity;
            _getAllStateOpportunity = getAllStateOpportunity;
            _getByIdStateOpportunity = getByIdStateOpportunity;
            _updateStateOpportunity = updateStateOpportunity;
            _patchStateOpportunityStatus = patchStateOpportunityStatus;
            _getSelectStateOpportunity = getSelectStateOpportunity;
        }

        [HttpPost]
        [Route("StateOpportunityCreate")]
        public async Task<IActionResult> Create([FromBody] StateOpportunityCreateDto dto)
        {
            var result = await _createStateOpportunity.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpGet]
        [Route("StateOpportunityList")]
        public async Task<IActionResult> GetList([FromQuery] int businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllStateOpportunity.ExecuteAsync(businessId, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de pagos." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }
        [HttpGet]
        [Route("StateOpportunitySelect")]
        public async Task<IActionResult> StateOpportunitySelect(
            [FromQuery] long businessId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectStateOpportunity.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);


        }
        [HttpGet]
        [Route("StateOpportunityById")]
        public async Task<IActionResult> GetById([FromQuery] int stateOpportunityId)
        {
            var result = await _getByIdStateOpportunity.ExecuteAsync(stateOpportunityId);
            if (result == null)
                return NotFound(new { message = "Estado de oportunidades no encontrado." });
            return Ok(result);
        }
        [HttpPut]
        [Route("StateOpportunityUpdate")]
        public async Task<IActionResult> Update([FromBody] StateOpportunityUpdateDto dto)
        {
            var result = await _updateStateOpportunity.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPatch]
        [Route("StateOpportunityStatus")]
        public async Task<IActionResult> Patch([FromBody] StateOpportunityStatusToggleDto dto)
        {
            var result = await _patchStateOpportunityStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

using Application.DTOs.LeadsSources;
using Application.UseCases.LeadsSources;
using Application.UseCases.LeadsSources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsSourcesController : ControllerBase
    {
        private readonly CreateLeadsSources _createLeadsSources;
        private readonly GetAllLeadsSources _getAllLeadsStaus;
        private readonly GetLeadsSourcesById _getByIdLeadsStaus;
        private readonly UpdateLeadsSources _updateLeadsStaus;
        private readonly GetSelectLeadsSources _getSelectLeadsStaus;
        private readonly PatchLeadsSourcesStatus _patchLeadsStaus;

        public LeadsSourcesController(GetAllLeadsSources getAllLeadsStaus, GetLeadsSourcesById getByIdLeadsStaus, UpdateLeadsSources updateLeadsStaus, GetSelectLeadsSources getSelectLeadsStaus, PatchLeadsSourcesStatus patchLeadsStaus, CreateLeadsSources createLeadsSources)
        {

            _getAllLeadsStaus = getAllLeadsStaus;
            _getByIdLeadsStaus = getByIdLeadsStaus;
            _updateLeadsStaus = updateLeadsStaus;
            _getSelectLeadsStaus = getSelectLeadsStaus;
            _patchLeadsStaus = patchLeadsStaus;
            _createLeadsSources = createLeadsSources;
        }

        [HttpPost]
        [Route("LeadsSourcesCreate")]
        public async Task<IActionResult> Create([FromBody] LeadsSourcesCreateDto dto)
        {
            var result = await _createLeadsSources.ExecuteAsync(dto);
            return Ok(result);
        }


        [HttpGet]
        [Route("LeadsSourcesList")]
        public async Task<IActionResult> GetList([FromQuery] long businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllLeadsStaus.ExecuteAsync(businessId, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron calificaciones de leads." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("LeadsSourcesSelect")]
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
        [Route("LeadsSourcesIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long leadsSourcesId)
        {
            var result = await _getByIdLeadsStaus.ExecuteAsync(leadsSourcesId);
            if (result == null)
                return NotFound(new { message = "No se encontró la calificación de lead." });

            return Ok(result);
        }

        [HttpPut]
        [Route("LeadsSourcesUpdate")]
        public async Task<IActionResult> Update([FromBody] LeadsSourcesUpdateDto dto)
        {
            var result = await _updateLeadsStaus.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("LeadsSourcesStatus")]
        public async Task<IActionResult> PatchSources([FromBody] LeadsSourcesStatusToggleDto dto)
        {
            var result = await _patchLeadsStaus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

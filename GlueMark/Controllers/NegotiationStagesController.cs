using Application.UseCases.ConceptGroups;
using Application.UseCases.NegotiationStages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NegotiationStagesController : ControllerBase
    {
        private readonly GetSelectNegotiationStages _getSelectNegotiationStages;

        public NegotiationStagesController(GetSelectNegotiationStages getSelectNegotiationStages)
        {
            _getSelectNegotiationStages = getSelectNegotiationStages;
        }
        [HttpGet]
        [Route("NegotiationStagesSelect")]
        public async Task<IActionResult> NegotiationStagesSelect(
            [FromQuery] long businessid,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectNegotiationStages.ExecuteAsync(businessid, search, page, pageSize);
            return Ok(result);
        }
    }
}

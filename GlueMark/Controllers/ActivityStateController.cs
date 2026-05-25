using Application.UseCases.ActivityState;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityStateController : ControllerBase
    {
        private readonly CreateActivityState _createActivityState;
        private readonly GetAllActivityState _getAllActivityState;
        private readonly GetByIdActivityState _getByIdActivityState;
        private readonly GetSelectSearchActivityState _getSelectSearchActivityState;
        private readonly GetSelectActivityState _getSelectActivityState;
        private readonly PatchActivityState _patchActivityState;
        private readonly UpdateActivityState _updateActivityState;
        private readonly ILinkTokenService _linkToken;

        public ActivityStateController(CreateActivityState createActivityState, GetAllActivityState getAllActivityState, GetByIdActivityState getByIdActivityState, GetSelectActivityState getSelectActivityState, PatchActivityState patchActivityState, UpdateActivityState updateActivityState, ILinkTokenService linkToken, GetSelectSearchActivityState getSelectSearchActivityState)
        {
            _createActivityState = createActivityState;
            _getAllActivityState = getAllActivityState;
            _getByIdActivityState = getByIdActivityState;
            _getSelectActivityState = getSelectActivityState;
            _patchActivityState = patchActivityState;
            _updateActivityState = updateActivityState;
            _linkToken = linkToken;
            _getSelectSearchActivityState = getSelectSearchActivityState;
        }

        [HttpPost]
        [Route("ActivityStateCreate")]
        public async Task<IActionResult> Create([FromBody] dynamic dto)
        {
            var result = await _createActivityState.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpGet]
        [Route("ActivityStateList")]
        public async Task<IActionResult> GetList([FromQuery] int businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllActivityState.ExecuteAsync(businessId, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron estados de actividad." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }
        [HttpGet]
        [Route("ActivityStateSelect")]
        public async Task<IActionResult> ActivityStateSelect(
            [FromQuery] long businessId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectSearchActivityState.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }
        [HttpGet]
        [Route("ActivityStateSelectNo")]
        public async Task<IActionResult> ActivityStateSelectAll([FromQuery] long businessId)
        {
            var result = await _getSelectActivityState.ExecuteAsync(businessId);
            return Ok(result);
        }
        [HttpGet]
        [Route("ActivityStateById")]
        public async Task<IActionResult> GetById([FromQuery] string linkToken)
        {
            if (!_linkToken.TryValidate(linkToken, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (entity != "opportunity")
                return BadRequest("Token no pertenece a Oportunidades.");

            var result = await _getByIdActivityState.ExecuteAsync(resourceId);
            if (result == null)
                return NotFound(new { message = "Estado de actividad no encontrado." });
            return Ok(result);
        }
    }
}

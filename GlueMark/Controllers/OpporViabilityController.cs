using Application.DTOs.OpporViability;
using Application.UseCases.OpporViability;
using Azure;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpporViabilityController : ControllerBase
    {
        private readonly GetAllOpporViability _getAllOpporViability;
        private readonly PatchOpporViability _patchOpporViability;
        private readonly ConvertPreOpportunity _convertPreOpportunity;
        private readonly ILinkTokenService _linkToken;

        public OpporViabilityController(
            GetAllOpporViability getAllOpporViability,
            PatchOpporViability patchOpporViability,
            ConvertPreOpportunity convertPreOpportunity,
            ILinkTokenService linkToken)
        {
            _getAllOpporViability = getAllOpporViability;
            _patchOpporViability = patchOpporViability;
            _convertPreOpportunity = convertPreOpportunity;
            _linkToken = linkToken;
        }

        [HttpGet]
        [Route("OpporViabilityList")]
        public async Task<IActionResult> GetList([FromQuery] long businessId, [FromQuery] long? usersBy = null, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllOpporViability.ExecuteAsync(businessId, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron oportunidades." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();

            return Ok(result);
        }

        [HttpPatch]
        [Route("OpporViabilityStatus")]
        public async Task<IActionResult> Patch([FromBody] OpporViabilityStatusToggleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.LinkToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entity, "opporviability", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a una oportunidad.");

            dto.LinkToken = resourceId;

            var result = await _patchOpporViability.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPost("ProcessDecision")]
        public async Task<IActionResult> ProcessDecision([FromBody] OpporViabilityConvertDto dto)
        {
            if (!_linkToken.ValidateToken(dto.LinkToken, out _, out var entity, out var resourceId))
                return Unauthorized("Token inválido.");

            if (!string.Equals(entity, "opporviability", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token incorrecto.");

            dto.LinkToken = resourceId;

            return Ok(await _convertPreOpportunity.ExecuteAsync(dto));
        }

    }
}
                                    
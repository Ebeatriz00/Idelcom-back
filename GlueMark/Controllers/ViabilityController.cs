using Application.DTOs.Viability;
using Application.UseCases.Viability;
using Azure;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    namespace Idelcom.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class ViabilityController : ControllerBase
        {
            private readonly GetAllViability _getAllViability;
            private readonly GetSelectViability _getSelectViability;
            private readonly GetByIdViability _getByIdViability;
            private readonly PatchViabilityStatus _patchViabilityStatus;
            private readonly ILinkTokenService _linkToken;

            public ViabilityController(
                GetAllViability getAllViability,
                GetSelectViability getSelectViability,
                GetByIdViability getByIdViability,
                PatchViabilityStatus patchViabilityStatus,
                ILinkTokenService linkToken)
            {
                _getAllViability = getAllViability;
                _getSelectViability = getSelectViability;
                _getByIdViability = getByIdViability;
                _patchViabilityStatus = patchViabilityStatus;
                _linkToken = linkToken;
            }

           

            [HttpGet]
            [Route("ViabilityList")]
            public async Task<IActionResult> GetList(
                [FromQuery] long businessId,
                [FromQuery] string? search = null,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10)
            {
                var result = await _getAllViability.ExecuteAsync(businessId, search, page, pageSize);

                if (result == null || !result.Items.Any())
                    return NotFound(new { message = "No se encontraron análisis de viabilidad." });

                Response.Headers["X-Total-Count"] = result.Total.ToString();
                Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

                return Ok(result);
            }

            [HttpGet]
            [Route("ViabilitySelect")]
            public async Task<IActionResult> ViabilitySelect(
                [FromQuery] long businessId,
                [FromQuery] string? search = null,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20)
            {
                var result = await _getSelectViability.ExecuteAsync(businessId, search, page, pageSize);
                return Ok(result);
            }

            [HttpGet]
            [Route("ViabilityById")]
            public async Task<IActionResult> GetById([FromQuery] string linkToken)
            {
                if (!_linkToken.TryValidate(linkToken, out var entity, out var resourceId))
                    return Unauthorized("Token inválido o expirado.");

                if (entity != "viability")
                    return BadRequest("Token no pertenece a Viabilidad.");

                var viabilityToken = Convert.ToString(resourceId);

                var result = await _getByIdViability.ExecuteAsync(viabilityToken);

                if (result == null)
                    return NotFound(new { message = "Análisis de viabilidad no encontrado." });

                return Ok(result);
            }

            
            [HttpPatch]
            [Route("ViabilityStatus")]
            public async Task<IActionResult> Patch([FromBody] ViabilityStatusToggleDto dto)
            {
                if (string.IsNullOrWhiteSpace(dto.LinkToken))
                    return Unauthorized("Falta token.");

                if (!_linkToken.ValidateToken(dto.LinkToken, out var claims, out var entity, out var resourceId))
                    return Unauthorized("Token inválido o expirado.");

                if (!string.Equals(entity, "viability", StringComparison.OrdinalIgnoreCase))
                    return BadRequest("Token no pertenece a viabilidad.");

                dto.LinkToken = resourceId;

                var result = await _patchViabilityStatus.ExecuteAsync(dto);
                return Ok(result);
            }
        }
    }
}

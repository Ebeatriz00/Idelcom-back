using Application.DTOs.Hiring;
using Application.UseCases.ExchangeRate;
using Application.UseCases.Hiring;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HiringController : ControllerBase
    {
        private readonly GetAllHiring _getAllHiring;
        private readonly UpdateStatusHiring _updateHiringStatus;
        private readonly MarkFilesRead _markFilesRead;
        private readonly ILinkTokenService _linkToken;

        public HiringController(GetAllHiring getAllHiring, UpdateStatusHiring updateStatusHiring, MarkFilesRead markFilesRead,
            ILinkTokenService linkToken)
        {
            _getAllHiring = getAllHiring;
            _updateHiringStatus = updateStatusHiring;
            _markFilesRead = markFilesRead;
            _linkToken = linkToken;
        }

        [HttpGet]
        [Route("HiringList")]
        public async Task<IActionResult> GetList([FromQuery] long businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? workerId = null, [FromQuery] long? usersId = null)
        {
            var result = await _getAllHiring.ExecuteAsync(businessId, search, page, pageSize, workerId, usersId);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de cambio." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }


        [HttpPut]
        [Route("UpdateHiringStatus")]

        public async Task<ActionResult> UpdateStatus([FromBody] HiringUpdateStatusDto dto)
        {
            var result = await _updateHiringStatus.ExecuteAsync(dto);
            return Ok(result);
        }



        [HttpPost("MarkFilesRead")]
        public async Task<IActionResult> MarkFilesRead([FromBody] MarkFileReadDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.OpporToken))
                return Unauthorized("Falta token.");
            if (!_linkToken.ValidateToken(dto.OpporToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a oportunidad.");
            dto.OpporToken = resourceId;
            var result = await _markFilesRead.ExecuteAsync(dto);

            return Ok(result);
        }

    }
}

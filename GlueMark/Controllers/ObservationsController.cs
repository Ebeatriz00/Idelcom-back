using Application.DTOs;
using Application.DTOs.Observations;
using Application.UseCases.Observations;
using Core.Entities;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObservationsController : ControllerBase
    {
        private readonly CreateObservations _createObservations;
        private readonly GetAllObservations _getAllObservations;
        private readonly GetObservationsById _getObservationsById;
        private readonly UpdateObservation _updateObservation;
        private readonly UpdateObservationDate _updateObservationDate;
        private readonly GetAllByProjectObservations _getAllByProjectObservations;
        private readonly GetAllByHiringObservations _getAllByHiringObservations;
        ILinkTokenService _linkToken;

        public ObservationsController(
            CreateObservations createObservationsOppor,
            GetAllObservations getAllObservationsOppor,
            GetObservationsById getObservationsById,
            UpdateObservation updateObservation,
            UpdateObservationDate updateObservationDate,
            GetAllByProjectObservations getAllByProjectObservations,
            GetAllByHiringObservations getAllByHiringObservations,
            ILinkTokenService linkToken)
        {
            _createObservations = createObservationsOppor;
            _getAllObservations = getAllObservationsOppor;
            _getObservationsById = getObservationsById;
            _updateObservation = updateObservation;
            _updateObservationDate = updateObservationDate;
            _getAllByProjectObservations = getAllByProjectObservations;
            _getAllByHiringObservations = getAllByHiringObservations;
            _linkToken = linkToken;
        }

        [HttpPost]
        [Route("ObservationsCreate")]
        public async Task<IActionResult> Create([FromBody] ObservationsCreateDto dto)
        {
            var result = await _createObservations.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("ObservationsList")]
        public async Task<IActionResult> GetList([FromQuery] string opporToken)
        {
            var result = await _getAllObservations.ExecuteAsync(opporToken);

            if (result == null || !result.Any())
                return NotFound(new { message = "No se encontraron observaciones." });

            return Ok(result);
        }

        [HttpGet]
        [Route("ObservationsListProject")]
        public async Task<IActionResult> GetListProject([FromQuery] string opporToken)
        {
            var result = await _getAllByProjectObservations.ExecuteAsync(opporToken);

            if (result == null || !result.Any())
                return NotFound(new { message = "No se encontraron observaciones." });

            return Ok(result);
        }


        [HttpGet]
        [Route("ObservationsListHiring")]
        public async Task<IActionResult> GetListHiring([FromQuery] string opporToken)
        {
            var result = await _getAllByHiringObservations.ExecuteAsync(opporToken);

            if (result == null || !result.Any())
                return NotFound(new { message = "No se encontraron observaciones." });

            return Ok(result);
        }


        [HttpGet]
        [Route("ObservationsById")]
        public async Task<IActionResult> GetDetail([FromQuery] long obsId)
        {
            var result = await _getObservationsById.ExecuteAsync(obsId);

            if (result == null)
                return NotFound(new { message = "No se encontró la observación." });

            return Ok(result);
        }


        [HttpPut]
        [Route("ObservationsUpdate")]
        public async Task<IActionResult> UpdateReason([FromBody] ObservationUpdateDto dto)
        {
            var result = await _updateObservation.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("ObservationsUpdateDate")]
        public async Task<IActionResult> UpdateDate([FromBody] ObservationsDateUpdateDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.ObsStatusToken))
            {
                // "Abrimos" el token
                if (_linkToken.ValidateToken(dto.ObsStatusToken, out var claims, out var entity, out var resourceId))
                {
                    if (long.TryParse(resourceId, out long parsedId))
                    {
                        dto.ObsStatusId = parsedId;
                    }
                }
                else
                {
                    return Unauthorized("Token de estado inválido o expirado.");
                }
            }
            var result = await _updateObservationDate.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

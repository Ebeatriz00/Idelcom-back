using Application.DTOs.MovementTypes;
using Application.UseCases.MovementTypes;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    namespace Idelcom.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class MovementTypesController : ControllerBase
        {
            private readonly CreateMovementTypes _createMovementTypes;
            private readonly GetAllMovementTypes _getAllMovementTypes;
            private readonly GetMovementTypesById _getMovementTypesById;
            private readonly UpdateMovementTypes _updateMovementTypes;
            private readonly PatchMovementTypesStatus _patchMovementTypesStatus;
            private readonly GetSelectMovementTypes _getSelectMovementTypes;

            public MovementTypesController(
                CreateMovementTypes createMovementTypes,
                GetAllMovementTypes getAllMovementTypes,
                GetMovementTypesById getMovementTypesById,
                UpdateMovementTypes updateMovementTypes,
                PatchMovementTypesStatus patchMovementTypesStatus,
                GetSelectMovementTypes getSelectMovementTypes)
            {
                _createMovementTypes = createMovementTypes;
                _getAllMovementTypes = getAllMovementTypes;
                _getMovementTypesById = getMovementTypesById;
                _updateMovementTypes = updateMovementTypes;
                _patchMovementTypesStatus = patchMovementTypesStatus;
                _getSelectMovementTypes = getSelectMovementTypes;
            }

            [HttpPost]
            [Route("MovementTypesCreate")]
            public async Task<IActionResult> Create([FromBody] MovementTypesCreateDto dto)
            {
                var result = await _createMovementTypes.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpGet]
            [Route("MovementTypesList")]
            public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
            {
                var result = await _getAllMovementTypes.ExecuteAsync(business_id, search, page, pageSize, usersBy);
                if (result == null || !result.Items.Any())
                    return NotFound(new { message = "No se encontraron tipos de movimiento." });

                Response.Headers["X-Total-Count"] = result.Total.ToString();
                Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

                return Ok(result);
            }

            [HttpGet]
            [Route("MovementTypesSelect")]
            public async Task<IActionResult> MovementTypesSelect(
                [FromQuery] long business_id,
                [FromQuery] string? search = null,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20)
            {
                var result = await _getSelectMovementTypes.ExecuteAsync(business_id, search, page, pageSize);
                return Ok(result);
            }

            [HttpGet]
            [Route("MovementTypesById")]
            public async Task<IActionResult> GetById([FromQuery] long movementTypesId)
            {
                var result = await _getMovementTypesById.ExecuteAsync(movementTypesId);
                if (result == null)
                    return NotFound(new { message = "No se encontró el tipo de movimiento." });
                return Ok(result);
            }

            [HttpPut]
            [Route("MovementTypesUpdate")]
            public async Task<IActionResult> Update([FromBody] MovementTypesUpdateDto dto)
            {
                var result = await _updateMovementTypes.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpPatch]
            [Route("MovementTypesStatus")]
            public async Task<IActionResult> PatchStatus([FromBody] MovementTypesStatusToggleDto dto)
            {
                var result = await _patchMovementTypesStatus.ExecuteAsync(dto);
                return Ok(result);
            }
        }
    }
}

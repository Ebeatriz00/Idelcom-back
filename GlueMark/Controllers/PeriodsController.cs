using Application.DTOs.Periods;
using Application.UseCases.Periods;
using Azure;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Idelcom.Controllers
{
    namespace Idelcom.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class PeriodsController : ControllerBase
        {
            private readonly CreatePeriods _createPeriods;
            private readonly GetAllPeriods _getAllPeriods;
            private readonly GetPeriodsById _getPeriodsById;
            private readonly UpdatePeriods _updatePeriods;
            private readonly PatchPeriodsStatus _patchPeriodsStatus;
            private readonly GetSelectPeriods _getSelectPeriods;
            private readonly ToggleBlockPeriods _toggleBlockPeriods; 

            public PeriodsController(
                CreatePeriods createPeriods,
                GetAllPeriods getAllPeriods,
                GetPeriodsById getPeriodsById,
                UpdatePeriods updatePeriods,
                PatchPeriodsStatus patchPeriodsStatus,
                GetSelectPeriods getSelectPeriods,
                ToggleBlockPeriods toggleBlockPeriods) 
            {
                _createPeriods = createPeriods;
                _getAllPeriods = getAllPeriods;
                _getPeriodsById = getPeriodsById;
                _updatePeriods = updatePeriods;
                _patchPeriodsStatus = patchPeriodsStatus;
                _getSelectPeriods = getSelectPeriods;
                _toggleBlockPeriods = toggleBlockPeriods; 
            }

            [HttpPost]
            [Route("PeriodsCreate")]
            public async Task<IActionResult> Create([FromBody] PeriodsCreateDto dto)
            {
                var result = await _createPeriods.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpGet]
            [Route("PeriodsList")]
            public async Task<IActionResult> GetList(
                [FromQuery] long business_id,
                [FromQuery] long exercises_id,
                [FromQuery] string? search = null,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10, 
                [FromQuery] long? usersBy = null)
            {
                var result = await _getAllPeriods.ExecuteAsync(business_id, exercises_id, search, page, pageSize, usersBy);
                if (result == null || !result.Items.Any())
                    return NotFound(new { message = "No se encontraron períodos." });

                Response.Headers["X-Total-Count"] = result.Total.ToString();
                Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

                return Ok(result);
            }

            [HttpGet]
            [Route("PeriodsSelect")]
            public async Task<IActionResult> PeriodsSelect(
                [FromQuery] long business_id,
                [FromQuery] string? search = null,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20)
            {
                var result = await _getSelectPeriods.ExecuteAsync(business_id, search, page, pageSize);
                return Ok(result);
            }

            [HttpGet]
            [Route("PeriodsById")]
            public async Task<IActionResult> GetById([FromQuery] long periodsId)
            {
                var result = await _getPeriodsById.ExecuteAsync(periodsId);
                if (result == null)
                    return NotFound(new { message = "No se encontro el período." });
                return Ok(result);
            }

            [HttpPut]
            [Route("PeriodsUpdate")]
            public async Task<IActionResult> Update([FromBody] PeriodsUpdateDto dto)
            {
                var result = await _updatePeriods.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpPatch]
            [Route("PeriodsStatus")]
            public async Task<IActionResult> PatchStatus([FromBody] PeriodsStatusToggleDto dto)
            {
                var result = await _patchPeriodsStatus.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpPatch]
            [Route("PeriodsBlock")]
            public async Task<IActionResult> PatchBlock([FromBody] PeriodsBlockToggleDto dto)
            {
                var result = await _toggleBlockPeriods.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpPost]
            [Route("PeriodsCreateBulk")]
            public async Task<IActionResult> CreateBulk([FromBody] List<PeriodsCreateDto> dto)
            {
                try
                {
                    foreach (var dtos in dto)
                    {
                        var result = await _createPeriods.ExecuteAsync(dtos);
                        if (result.Status == 0)
                        {
                            return BadRequest(new { message = $"Error al crear '{dtos.Description}': {result.Message}" });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Error en el proceso de creación masiva.", error = ex.Message });
                }
                return Ok(new GlobalResponse { Status = 1, Message = "Año completo creado con éxito." });
            }


        }
    }
}

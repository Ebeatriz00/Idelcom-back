using Application.DTOs.PriorityState;
using Application.UseCases.PriorityState;
using Application.UseCases.StateTask;
using Azure;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    namespace Idelcom.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class PriorityStateController : ControllerBase
        {
            private readonly CreatePriorityState _createPriorityState;
            private readonly GetAllPriorityState _getAllPriorityState;
            private readonly GetByIdPriorityState _getByIdPriorityState;
            private readonly UpdatePriorityState _updatePriorityState;
            private readonly PatchPriorityState _patchPriorityStateStatus;
            private readonly GetSelectPriorityState _getSelectPriorityState;
            private readonly GetSelectNormalPriorityState _getSelectNormalPriorityState;
            private readonly ILinkTokenService _linkToken;

            public PriorityStateController(
                CreatePriorityState createPriorityState,
                GetAllPriorityState getAllPriorityState,
                GetByIdPriorityState getByIdPriorityState,
                UpdatePriorityState updatePriorityState,
                PatchPriorityState patchPriorityStateStatus,
                GetSelectPriorityState getSelectPriorityState,
                GetSelectNormalPriorityState getSelectNormalPriorityState,
                ILinkTokenService linkToken)
            {
                _createPriorityState = createPriorityState;
                _getAllPriorityState = getAllPriorityState;
                _getByIdPriorityState = getByIdPriorityState;
                _updatePriorityState = updatePriorityState;
                _patchPriorityStateStatus = patchPriorityStateStatus;
                _getSelectPriorityState = getSelectPriorityState;
                _getSelectNormalPriorityState = getSelectNormalPriorityState;
                _linkToken = linkToken;
            }

            [HttpPost]
            [Route("PriorityStateCreate")]
            public async Task<IActionResult> Create([FromBody] PriorityStateCreateDto dto)
            {
                var result = await _createPriorityState.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpGet]
            [Route("PriorityStateList")]
            public async Task<IActionResult> GetList(
                [FromQuery] long businessId,
                [FromQuery] string? search = null,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10)
            {
                var result = await _getAllPriorityState.ExecuteAsync(businessId, search, page, pageSize);
                if (result == null || !result.Items.Any())
                    return NotFound(new { message = "No se encontraron prioridades." });


                Response.Headers["X-Total-Count"] = result.Total.ToString();
                Response.Headers["X-Total-Pages"] = result.TotalPages.ToString(); 
                return Ok(result);
            }

            [HttpGet]
            [Route("PriorityStateSelect")]
            public async Task<IActionResult> PriorityStateSelect(
                [FromQuery] long businessId,
                [FromQuery] string? search = null,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20)
            {
                var result = await _getSelectPriorityState.ExecuteAsync(businessId, search, page, pageSize);
                return Ok(result);
            }


            [HttpGet]
            [Route("PriorityStateSelectNo")]
            public async Task<IActionResult> get_profiles_select([FromQuery] long businessId)
            {
                var result = await _getSelectNormalPriorityState.ExecuteAsync(businessId);
                if (result == null)
                    return NotFound(new { message = "Estado de prioridades no encontrado." });
                return Ok(result);
            }

            [HttpGet]
            [Route("PriorityStateById")]
            public async Task<IActionResult> GetById([FromQuery] long priorityStateId)
            {
                var result = await _getByIdPriorityState.ExecuteAsync(priorityStateId);
                if (result == null)
                    return NotFound(new { message = "Prioridad no encontrada." });
                return Ok(result);
            }

            [HttpPut]
            [Route("PriorityStateUpdate")]
            public async Task<IActionResult> Update([FromBody] PriorityStateUpdateDto dto)
            {
                var result = await _updatePriorityState.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpPatch]
            [Route("PriorityStateStatus")]
            public async Task<IActionResult> Patch([FromBody] PriorityStateStatusToggleDto dto)
            {
                var result = await _patchPriorityStateStatus.ExecuteAsync(dto);
                return Ok(result);
            }
        }
    }
}

using Application.DTOs.StateTask;
using Application.UseCases.StateTask;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateTaskController : ControllerBase
    {
        private readonly CreateStateTask _createStateTask;
        private readonly GetAllStateTask _getAllStateTask;
        private readonly GetByIdStateTask _getByIdStateTask;
        private readonly UpdateStateTask _updateStateTask;
        private readonly PatchStateTask _patchStateTaskStatus;
        private readonly GetSelectStateTask _getSelectStateTask;
        private readonly GetSelectNormalStateTask _getSelectNormalStateTask;

        public StateTaskController(CreateStateTask createStateTask, GetAllStateTask getAllStateTask, GetByIdStateTask getByIdStateTask, UpdateStateTask updateStateTask, PatchStateTask patchStateTaskStatus, GetSelectStateTask getSelectStateTask, GetSelectNormalStateTask getSelectNormalStateTask)
        {
            _createStateTask = createStateTask;
            _getAllStateTask = getAllStateTask;
            _getByIdStateTask = getByIdStateTask;
            _updateStateTask = updateStateTask;
            _patchStateTaskStatus = patchStateTaskStatus;
            _getSelectStateTask = getSelectStateTask;
            _getSelectNormalStateTask = getSelectNormalStateTask;
        }

        [HttpPost]
        [Route("StateTaskCreate")]
        public async Task<IActionResult> Create([FromBody] StateTaskCreateDto dto)
        {
            var result = await _createStateTask.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpGet]
        [Route("StateTaskList")]
        public async Task<IActionResult> GetList([FromQuery] int businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllStateTask.ExecuteAsync(businessId, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron estados de tareas." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }
        [HttpGet]
        [Route("StateTaskSelect")]
        public async Task<IActionResult> StateTaskSelect(
            [FromQuery] long businessId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectStateTask.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);


        }
        [HttpGet]
        [Route("StateTaskSelectNo")]
        public async Task<IActionResult> get_profiles_select([FromQuery] long businessId)
        {
            var result = await _getSelectNormalStateTask.ExecuteAsync(businessId);
            if (result == null)
                return NotFound(new { message = "Estado de tareas no encontrado." });
            return Ok(result);
        }


        [HttpGet]
        [Route("StateTaskById")]
        public async Task<IActionResult> GetById([FromQuery] long stateTaskId)
        {
            var result = await _getByIdStateTask.ExecuteAsync(stateTaskId);
            if (result == null)
                return NotFound(new { message = "Estado de tareas no encontrado." });
            return Ok(result);
        }
        [HttpPut]
        [Route("StateTaskUpdate")]
        public async Task<IActionResult> Update([FromBody] StateTaskUpdateDto dto)
        {
            var result = await _updateStateTask.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPatch]
        [Route("StateTaskStatus")]
        public async Task<IActionResult> Patch([FromBody] StateTaskStatusToggleDto dto)
        {
            var result = await _patchStateTaskStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

using Application.DTOs.Activity;
using Application.DTOs.Tasks;
using Application.UseCases.Activity;
using Application.UseCases.Tasks;
using Azure;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly CreateTasks _createTasks;
        private readonly GetAllTasks _getAllTasks;
        private readonly GetAllTasksProject _getAllProjects;
        private readonly GetTasksById _getTasksById;
        private readonly UpdateTasks _updateTasks;
        private readonly PatchTasksStatus _patchTasksStatus;
        private readonly GetSelectTasks _getSelectTasks;
        private readonly PatchTasksCompleted _patchTasksCompleted;
        private readonly PatchTasksChangeState _patchTasksChangeState;
        private readonly PatchTasksPriorityState _patchTasksPriorityState;
        private readonly DeleteTask _deleteTasks;
        private readonly DeleteTasksProject _deleteTasksProject;
        private readonly ILinkTokenService _linkToken;
        public TasksController(
            CreateTasks createTasks,
            GetAllTasks getAllTasks,
            GetTasksById getTasksById,
            UpdateTasks updateTasks,
            PatchTasksStatus patchTasksStatus,
            GetSelectTasks getSelectTasks,
            PatchTasksCompleted patchTasksCompleted,
            PatchTasksChangeState patchTasksChangeState,
            ILinkTokenService linkToken,
            PatchTasksPriorityState patchTasksPriorityState,
            GetAllTasksProject getAllProjects,
            DeleteTask deleteTasks,
            DeleteTasksProject deleteTasksProject)
        {
            _createTasks = createTasks;
            _getAllTasks = getAllTasks;
            _getTasksById = getTasksById;
            _updateTasks = updateTasks;
            _patchTasksStatus = patchTasksStatus;
            _getSelectTasks = getSelectTasks;
            _patchTasksCompleted = patchTasksCompleted;
            _patchTasksChangeState = patchTasksChangeState;
            _linkToken = linkToken;
            _patchTasksPriorityState = patchTasksPriorityState;
            _getAllProjects = getAllProjects;
            _deleteTasks = deleteTasks;
            _deleteTasksProject = deleteTasksProject;
        }

        [HttpPost]
        [Route("TasksCreate")]
        public async Task<IActionResult> Create([FromBody] TasksCreateDto dto)
        {
            var result = await _createTasks.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("TasksList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllTasks.ExecuteAsync(business_id, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tareas." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("TasksProjectList")]
        public async Task<IActionResult> GetProjectList(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? opporToken = null)
        {
            long? opporId = null;

            if (!string.IsNullOrEmpty(opporToken))
            {
                if (_linkToken.ValidateToken(opporToken, out var claims, out var entity, out var resourceId))
                {
                    if (long.TryParse(resourceId, out long parsedId))
                    {
                        opporId = parsedId;
                    }
                }
            }

            var result = await _getAllProjects.ExecuteAsync(business_id, search, page, pageSize, opporId);

            return Ok(result);
        }

        [HttpGet]
        [Route("TasksSelect")]
        public async Task<IActionResult> TasksSelect(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectTasks.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("TasksById")]
        public async Task<IActionResult> GetById([FromQuery] string linkToken) // Cambio: long -> string
        {
            var result = await _getTasksById.ExecuteAsync(linkToken);
            if (result == null)
                return NotFound(new { message = "No se encontro la tarea." });
            return Ok(result);
        }

        [HttpPut]
        [Route("TasksUpdate")]
        public async Task<IActionResult> Update([FromBody] TasksUpdateDto dto)
        {
            var result = await _updateTasks.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("TasksStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] TasksStatusToggleDto dto)
        {
            var result = await _patchTasksStatus.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("TasksStateCompleted")]
        public async Task<IActionResult> PatchTasksStateCompleted([FromBody] TasksCompletedDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.linkToken))
                return Unauthorized("Falta token.");


            if (!_linkToken.ValidateToken(dto.linkToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");


            if (!string.Equals(entity, "tasks", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a Tareas.");


            dto.linkToken = resourceId;
            var result = await _patchTasksCompleted.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("TasksStateChange")]
        public async Task<IActionResult> PatchTasksStateChange([FromBody] TasksChangeStateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.linkToken))
                return Unauthorized("Falta token.");


            if (!_linkToken.ValidateToken(dto.linkToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");


            if (string.IsNullOrWhiteSpace(dto.Status))
                return Unauthorized("Falta token.");


            if (!_linkToken.ValidateToken(dto.Status, out var claimsTtus, out var entityStatus, out var resourcesId))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entity, "tasks", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a Tareas.");


            dto.linkToken = resourceId;
            dto.Status = resourcesId;

            var result = await _patchTasksChangeState.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("TasksPriorityStateChange")]
        public async Task<IActionResult> PatchTasksPriorityStateChange([FromBody] TaskChangePriorityStateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.linkToken))
                return Unauthorized("Falta token.");


            if (!_linkToken.ValidateToken(dto.linkToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");


            if (!string.Equals(entity, "tasks", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a Tareas.");

            if (string.IsNullOrWhiteSpace(dto.Status))
                return Unauthorized("Falta token.");


            if (!_linkToken.ValidateToken(dto.Status, out var claimsTtus, out var entityStatus, out var resourcesId))
                return Unauthorized("Token inválido o expirado.");


            dto.linkToken = resourceId;
            dto.Status = resourcesId;
            var result = await _patchTasksPriorityState.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpDelete]
        [Route("TasksOpporDelete")]
        public async Task<IActionResult> DeleteOppor([FromBody] TaksOpporDeleteDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.OpporToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.OpporToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");


            if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a oportunidad.");


            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");


            if (!_linkToken.ValidateToken(dto.LinkToken, out var claimsL, out var entityL, out var resourceIdL))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entityL, "tasks", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a tareas.");

            dto.OpporToken = resourceId;
            dto.LinkToken = resourceIdL;
            var result = await _deleteTasks.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpDelete]
        [Route("TasksProjectDelete")]
        public async Task<IActionResult> DeleteProject([FromBody] TasksProjectDeleteDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ProjectToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.ProjectToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");


            if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a oportunidad.");


            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");


            if (!_linkToken.ValidateToken(dto.LinkToken, out var claimsL, out var entityL, out var resourceIdL))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entityL, "tasks", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a tareas.");

            dto.ProjectToken = resourceId;
            dto.LinkToken = resourceIdL;
            var result = await _deleteTasksProject.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

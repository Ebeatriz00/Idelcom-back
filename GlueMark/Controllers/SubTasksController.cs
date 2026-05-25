using Application.DTOs.SubTasks;
using Application.DTOs.SubTasks.Application.DTOs.SubTasks;
using Application.UseCases.SubTasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubTasksController : ControllerBase
    {
        private readonly CreateSubTasks _createSubTasks;
        private readonly UpdateSubTasks _updateSubTasks;
        private readonly GetAllSubTasks _getAllSubTasks;
        private readonly DeleteSubTasks _deleteSubTasks;
        private readonly GetSubTasksById _getSubTasksById;

        public SubTasksController(
            CreateSubTasks createSubTasks,
            UpdateSubTasks updateSubTasks,
            GetAllSubTasks getAllSubTasks,
            DeleteSubTasks deleteSubTasks,
            GetSubTasksById getSubTasksById)
        {
            _createSubTasks = createSubTasks;
            _updateSubTasks = updateSubTasks;
            _getAllSubTasks = getAllSubTasks;
            _deleteSubTasks = deleteSubTasks;
            _getSubTasksById = getSubTasksById;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] SubTasksCreateDto dto)
        {
            try
            {
                var result = await _createSubTasks.ExecuteAsync(dto);

                return Ok(new
                {
                    status = 1,
                    message = "Sub-tarea creada correctamente",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { status = 0, message = ex.Message });
            }
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] SubTasksUpdateDto dto)
        {
            try
            {
                var result = await _updateSubTasks.ExecuteAsync(dto);

                if (!result)
                    return NotFound(new { status = 0, message = "No se pudo actualizar. Verifique que la sub-tarea exista." });

                return Ok(new
                {
                    status = 1,
                    message = "Actualizado correctamente",
                    success = true
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { status = 0, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("ListByTask")]
        public async Task<IActionResult> GetListByTask(
            [FromQuery] long businessId,
            [FromQuery] string? taskToken = null)
        {
            var result = await _getAllSubTasks.ExecuteAsync(businessId, taskToken);
            return Ok(result); 
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete([FromBody] SubTasksDeleteDto dto)
        {
            var result = await _deleteSubTasks.ExecuteAsync(dto);

            return Ok(new
            {
                status = 1,
                message = "Eliminado correctamente",
                success = result
            });
        }

        [HttpGet]
        [Route("ById")]
        public async Task<IActionResult> GetById([FromQuery] string linkToken)
        {
            var result = await _getSubTasksById.ExecuteAsync(linkToken);

            if (result == null)
                return NotFound(new { status = 0, message = "No se encontró la sub-tarea." });

            return Ok(result);
        }
    }
}
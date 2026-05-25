using Application.DTOs.WorkerStatus;
using Application.UseCases.WorkerStatus;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerStatusController : ControllerBase
    {
        private readonly CreateWorkerStatus _createWorkerStatus;
        private readonly GetAllWorkerStatus _getAllWorkerStatus;
        private readonly GetByIdWorkerStatus _getByIdWorkerStatus;
        private readonly UpdateWorkerStatus _updateWorkerStatus;
        private readonly GetSelectWorkerStatus _getSelectWorkerStatus;
        private readonly PatchWorkerStatus _patchWorkerStatus;

        public WorkerStatusController(
            CreateWorkerStatus createWorkerStatus,
            GetAllWorkerStatus getAllWorkerStatus,
            GetByIdWorkerStatus getByIdWorkerStatus,
            UpdateWorkerStatus updateWorkerStatus,
            GetSelectWorkerStatus getSelectWorkerStatus,
            PatchWorkerStatus patchWorkerStatus)
        {
            _createWorkerStatus = createWorkerStatus;
            _getAllWorkerStatus = getAllWorkerStatus;
            _getByIdWorkerStatus = getByIdWorkerStatus;
            _updateWorkerStatus = updateWorkerStatus;
            _getSelectWorkerStatus = getSelectWorkerStatus;
            _patchWorkerStatus = patchWorkerStatus;
        }

        [HttpPost]
        [Route("WorkerStatusCreate")]
        public async Task<IActionResult> Create([FromBody] WorkerStatusCreateDto dto)
        {
            var result = await _createWorkerStatus.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("WorkerStatusList")]
        public async Task<IActionResult> GetList(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] long? usersBy = null)
        {
            var result = await _getAllWorkerStatus.ExecuteAsync(business_id, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron estados de trabajador." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("WorkerStatusSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectWorkerStatus.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("WorkerStatusIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long workerStatusId)
        {
            var result = await _getByIdWorkerStatus.ExecuteAsync(workerStatusId);
            if (result == null)
                return NotFound(new { message = "No se encontró el estado de trabajador." });

            return Ok(result);
        }

        [HttpPut]
        [Route("WorkerStatusUpdate")]
        public async Task<IActionResult> Update([FromBody] WorkerStatusUpdateDto dto)
        {
            var result = await _updateWorkerStatus.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("WorkerStatusStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] WorkerStatusStatusToogleDto dto)
        {
            var result = await _patchWorkerStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
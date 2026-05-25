using Application.DTOs.Worker;
using Application.UseCases.DocumentType;
using Application.UseCases.Worker;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly CreateWorker _createWorker;
        private readonly GetAllWorker _getAllWorker;
        private readonly GetByIdWorker _getByIdWorker;
        private readonly UpdateWorker _updateWorker;
        private readonly PatchWorkerStatus _patchWorkerStatus;
        private readonly GetSelectWorker _getSelectWorker;
        private readonly GetSelectSalesWorker _getSelectSalesWorker;
        private readonly GetSelectProyectWorker _getSelectProyectWorker;
        private readonly GetSelectOperationsWorker _getSelectOperationsWorker;

        public WorkerController(
            CreateWorker createWorker,
            GetAllWorker getAllWorker,
            GetByIdWorker getByIdWorker,
            UpdateWorker updateWorker,
            PatchWorkerStatus patchWorkerStatus,
            GetSelectWorker getSelectWorker,
            GetSelectSalesWorker getSelectSalesWorker,
            GetSelectProyectWorker getSelectProyectWorker,
            GetSelectOperationsWorker getSelectOperationsWorker)
        {
            _createWorker = createWorker;
            _getAllWorker = getAllWorker;
            _getByIdWorker = getByIdWorker;
            _updateWorker = updateWorker;
            _patchWorkerStatus = patchWorkerStatus;
            _getSelectWorker = getSelectWorker;
            _getSelectSalesWorker = getSelectSalesWorker;
            _getSelectProyectWorker = getSelectProyectWorker;
            _getSelectOperationsWorker = getSelectOperationsWorker;
        }

        [HttpPost]
        [Route("WorkerCreate")]
        public async Task<IActionResult> Create([FromBody] WorkerCreateDto dto)
        {
            var result = await _createWorker.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("WorkerList")]
        public async Task<IActionResult> GetList([FromQuery] int business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllWorker.ExecuteAsync(business_id, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron trabajadores." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }

        [HttpGet]
        [Route("WorkerSelect")]
        public async Task<IActionResult> WorkerSelect(
        [FromQuery] long business_id,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectWorker.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("WorkerSalesSelect")]
        public async Task<IActionResult> WorkerSalesSelect(
        [FromQuery] long business_id,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectSalesWorker.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("WorkerProyectSelect")]
        public async Task<IActionResult> WorkerProyectSelect(
        [FromQuery] long business_id,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectProyectWorker.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("WorkerOperationsSelect")]
        public async Task<IActionResult> WorkerOperationsSelect(
        [FromQuery] long business_id,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectOperationsWorker.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }


        [HttpGet]
        [Route("WorkerIdList")]
        public async Task<IActionResult> GetListId([FromQuery] int workerId)
        {
            var result = await _getByIdWorker.ExecuteAsync(workerId);
            if (result == null)
                return NotFound(new { message = "No se encontró el trabajador." });

            return Ok(result);
        }

        

        [HttpPut]
        [Route("WorkerUpdate")]
        public async Task<IActionResult> Update([FromBody] WorkerUpdateDto dto)
        {
            var result = await _updateWorker.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("WorkerStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] WorkerStatusToggleDto dto)
        {
            var result = await _patchWorkerStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}


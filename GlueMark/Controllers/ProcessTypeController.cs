using Application.DTOs.ProcessType;
using Application.UseCases.ProcessType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessTypeController : ControllerBase
    {
        private readonly CreateProcessType _createProcessType;
        private readonly GetAllProcessType _getAllProcessType;
        private readonly GetByIdProcessType _getByIdProcessType;
        private readonly UpdateProcessType _updateProcessType;
        private readonly PatchProcessType _patchProcessTypeStatus;
        private readonly GetSelectProcessType _getSelectProcessType;

        public ProcessTypeController(CreateProcessType createProcessType, GetAllProcessType getAllProcessType, GetByIdProcessType getByIdProcessType, UpdateProcessType updateProcessType, PatchProcessType patchProcessTypeStatus, GetSelectProcessType getSelectProcessType)
        {
            _createProcessType = createProcessType;
            _getAllProcessType = getAllProcessType;
            _getByIdProcessType = getByIdProcessType;
            _updateProcessType = updateProcessType;
            _patchProcessTypeStatus = patchProcessTypeStatus;
            _getSelectProcessType = getSelectProcessType;
        }

        [HttpPost]
        [Route("ProcessTypeCreate")]
        public async Task<IActionResult> Create([FromBody] ProcessTypeCreateDto dto)
        {
            var result = await _createProcessType.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpGet]
        [Route("ProcessTypeList")]
        public async Task<IActionResult> GetList([FromQuery] int businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllProcessType.ExecuteAsync(businessId, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de pagos." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }
        [HttpGet]
        [Route("ProcessTypeSelect")]
        public async Task<IActionResult> ProcessTypeSelect(
            [FromQuery] long businessId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectProcessType.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);


        }
        [HttpGet]
        [Route("ProcessTypeById")]
        public async Task<IActionResult> GetById([FromQuery] int ProcessTypeId)
        {
            var result = await _getByIdProcessType.ExecuteAsync(ProcessTypeId);
            if (result == null)
                return NotFound(new { message = "Estado de oportunidades no encontrado." });
            return Ok(result);
        }
        [HttpPut]
        [Route("ProcessTypeUpdate")]
        public async Task<IActionResult> Update([FromBody] ProcessTypeUpdateDto dto)
        {
            var result = await _updateProcessType.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPatch]
        [Route("ProcessTypeStatus")]
        public async Task<IActionResult> Patch([FromBody] ProcessTypeStatusToggleDto dto)
        {
            var result = await _patchProcessTypeStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

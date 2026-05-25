using Application.DTOs.OperationsTeamSsoma;
using Application.UseCases.OperationsTeamSsoma;
using Core.Interfaces.Operations;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsTeamSsomaController(
        CreateOperationsTeamSsoma createOperationsTeamSsoma,
        GetByIdOperationsTeamSsoma getByIdOperationsTeamSsoma,
        GetListByProcessIdOperationsTeamSsoma getListByProcessIdOperationsTeamSsoma,
        UpdateOperationsTeamSsoma updateOperationsTeamSsoma,
        ProcessSsomaAssignmentChange processSsomaAssignmentChange,
        DeleteSsomaAssignment deleteSsomaAssignment,
        IOperationsTeamSsomaRepository repository)
        : BaseController
    {
        private readonly CreateOperationsTeamSsoma _createOperationsTeamSsoma = createOperationsTeamSsoma;
        private readonly GetByIdOperationsTeamSsoma _getByIdOperationsTeamSsoma = getByIdOperationsTeamSsoma;
        private readonly GetListByProcessIdOperationsTeamSsoma _getListByProcessIdOperationsTeamSsoma = getListByProcessIdOperationsTeamSsoma;
        private readonly UpdateOperationsTeamSsoma _updateOperationsTeamSsoma = updateOperationsTeamSsoma;
        private readonly ProcessSsomaAssignmentChange _processSsomaAssignmentChange = processSsomaAssignmentChange;
        private readonly DeleteSsomaAssignment _deleteSsomaAssignment = deleteSsomaAssignment;
        private readonly IOperationsTeamSsomaRepository _repository = repository;

        [HttpGet]
        [Route("GetByIdTeamSsoma")]
        public async Task<IActionResult> GetById([FromQuery] long operationsTeamSsomaId)
        {
            var result = await _getByIdOperationsTeamSsoma.ExecuteAsync(operationsTeamSsomaId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet]
        [Route("GetListProcessId")]
        public async Task<IActionResult> GetByProcessId([FromQuery] long ssomaProcessId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getListByProcessIdOperationsTeamSsoma.ExecuteAsync(businessId, ssomaProcessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetActiveAssignmentByWorkerId")]
        public async Task<IActionResult> GetActiveAssignmentByWorkerId([FromQuery] long workerId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _repository.GetActiveAssignmentByWorkerIdAsync(businessId, workerId);
            return Ok(result);
        }

        [HttpPost]
        [Route("CreateTeamSsoma")]
        public async Task<IActionResult> Create([FromBody] OperationsTeamSsomaCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _createOperationsTeamSsoma.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateTeamSsoma")]
        public async Task<IActionResult> Update([FromBody] OperationsTeamSsomaUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _updateOperationsTeamSsoma.ExecuteAsync(dto, userId, businessId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Route("ProcessAssignmentUpdate")]
        public async Task<IActionResult> ProcessAssignmentUpdate([FromBody] ProcessSsomaAssignmentChangeDto dto)
        {
            dto.ChangeType = SsomaAssignmentChangeType.Update;
            return await ProcessAssignmentChangeByType(dto);
        }

        [HttpPost]
        [Route("ProcessAssignmentRelocation")]
        public async Task<IActionResult> ProcessAssignmentRelocation([FromBody] ProcessSsomaAssignmentChangeDto dto)
        {
            dto.ChangeType = SsomaAssignmentChangeType.Relocation;
            return await ProcessAssignmentChangeByType(dto);
        }

        [HttpPost]
        [Route("ProcessAssignmentReplacement")]
        public async Task<IActionResult> ProcessAssignmentReplacement([FromBody] ProcessSsomaAssignmentChangeDto dto)
        {
            dto.ChangeType = SsomaAssignmentChangeType.Replacement;
            return await ProcessAssignmentChangeByType(dto);
        }

        [HttpDelete]
        [Route("DeleteTeamSsoma")]
        public async Task<IActionResult> Delete([FromQuery] long operationsTeamSsomaId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _deleteSsomaAssignment.ExecuteAsync(operationsTeamSsomaId, userId, businessId);
            return Ok(result);
        }

        private async Task<IActionResult> ProcessAssignmentChangeByType(ProcessSsomaAssignmentChangeDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _processSsomaAssignmentChange.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }
    }
}

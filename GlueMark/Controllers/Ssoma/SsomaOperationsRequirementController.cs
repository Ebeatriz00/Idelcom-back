using Application.DTOs.SsomaOperationsRequirement;
using Application.UseCases.SsomaOperationsRequirement;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Ssoma
{
    [Route("api/[controller]")]
    [ApiController]
    public class SsomaOperationsRequirementController(
        CreateSsomaOperationsRequirement createSsomaOperationsRequirement,
        GetAllSsomaOperationsRequirement getAllSsomaOperationsRequirement,
        GetListSsomaOperationsRequirementByWorker getListSsomaOperationsRequirementByWorker,
        ValidateSsomaOperationsRequirementByWorker validateSsomaOperationsRequirementByWorker,
        GetMissingSsomaOperationsRequirementByWorker getMissingSsomaOperationsRequirementByWorker,
        GetSelectOperationsSsomaOperationsRequirement getSelectOperationsSsomaOperationsRequirement,
        GetByIdSsomaOperationsRequirement getByIdSsomaOperationsRequirement,
        UpdateSsomaOperationsRequirement updateSsomaOperationsRequirement,
        DeleteSsomaOperationsRequirement deleteSsomaOperationsRequirement) : BaseController
    {
        private readonly CreateSsomaOperationsRequirement _createSsomaOperationsRequirement = createSsomaOperationsRequirement;
        private readonly GetAllSsomaOperationsRequirement _getAllSsomaOperationsRequirement = getAllSsomaOperationsRequirement;
        private readonly GetListSsomaOperationsRequirementByWorker _getListSsomaOperationsRequirementByWorker = getListSsomaOperationsRequirementByWorker;
        private readonly ValidateSsomaOperationsRequirementByWorker _validateSsomaOperationsRequirementByWorker = validateSsomaOperationsRequirementByWorker;
        private readonly GetMissingSsomaOperationsRequirementByWorker _getMissingSsomaOperationsRequirementByWorker = getMissingSsomaOperationsRequirementByWorker;
        private readonly GetSelectOperationsSsomaOperationsRequirement _getSelectOperationsSsomaOperationsRequirement = getSelectOperationsSsomaOperationsRequirement;
        private readonly GetByIdSsomaOperationsRequirement _getByIdSsomaOperationsRequirement = getByIdSsomaOperationsRequirement;
        private readonly UpdateSsomaOperationsRequirement _updateSsomaOperationsRequirement = updateSsomaOperationsRequirement;
        private readonly DeleteSsomaOperationsRequirement _deleteSsomaOperationsRequirement = deleteSsomaOperationsRequirement;

        [HttpGet]
        [Route("GetAllSsomaOperationsRequirement")]
        public async Task<IActionResult> GetAll(
            [FromQuery] long operationsId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllSsomaOperationsRequirement.ExecuteAsync(businessId, operationsId, page, pageSize, search);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetListSsomaOperationsRequirementByWorker")]
        public async Task<IActionResult> GetListByWorker(
            [FromQuery] long operationsId,
            [FromQuery] long workerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getListSsomaOperationsRequirementByWorker.ExecuteAsync(businessId, operationsId, workerId, page, pageSize, search);
            return Ok(result);
        }

        [HttpGet]
        [Route("ValidateSsomaOperationsRequirementByWorker")]
        public async Task<IActionResult> ValidateByWorker(
            [FromQuery] long operationsId,
            [FromQuery] long workerId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _validateSsomaOperationsRequirementByWorker.ExecuteAsync(businessId, operationsId, workerId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetMissingSsomaOperationsRequirementByWorker")]
        public async Task<IActionResult> GetMissingByWorker(
            [FromQuery] long operationsId,
            [FromQuery] long workerId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getMissingSsomaOperationsRequirementByWorker.ExecuteAsync(businessId, operationsId, workerId);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetSelectOperationsForHomologation")]
        public async Task<IActionResult> GetSelectOperations(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectOperationsSsomaOperationsRequirement.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetByIdSsomaOperationsRequirement")]
        public async Task<IActionResult> GetById([FromQuery] long ssomaOperationsRequirementId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getByIdSsomaOperationsRequirement.ExecuteAsync(ssomaOperationsRequirementId, businessId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Route("CreateSsomaOperationsRequirement")]
        public async Task<IActionResult> Create([FromBody] SsomaOperationsRequirementCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createSsomaOperationsRequirement.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateSsomaOperationsRequirement")]
        public async Task<IActionResult> Update([FromBody] SsomaOperationsRequirementUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updateSsomaOperationsRequirement.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("DeleteSsomaOperationsRequirement")]
        public async Task<IActionResult> Delete([FromQuery] long ssomaOperationsRequirementId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _deleteSsomaOperationsRequirement.ExecuteAsync(ssomaOperationsRequirementId, businessId, userId);
            return Ok(result);
        }
    }
}

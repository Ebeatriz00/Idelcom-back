using Application.DTOs.SsomaRequirement;
using Application.UseCases.SsomaRequirement;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Ssoma
{
    [Route("api/[controller]")]
    [ApiController]
    public class SsomaRequirementController(
        CreateSsomaRequirement createSsomaRequirement,
        GetAllSsomaRequirement getAllSsomaRequirement,
        GetByIdSsomaRequirement getByIdSsomaRequirement,
        GetGeneralRequirementById getGeneralRequirementById,
        UpdateSsomaRequirement updateSsomaRequirement,
        DeleteSsomaRequirement deleteSsomaRequirement,
        GetAllSsomaRequirementItem getAllSsomaRequirementItem,
        GetSelectSsomaRequirement getSelectSsomaRequirement) : BaseController
    {
        private readonly CreateSsomaRequirement _createSsomaRequirement = createSsomaRequirement;
        private readonly GetAllSsomaRequirement _getAllSsomaRequirement = getAllSsomaRequirement;
        private readonly GetByIdSsomaRequirement _getByIdSsomaRequirement = getByIdSsomaRequirement;
        private readonly GetGeneralRequirementById _getGeneralRequirementById = getGeneralRequirementById;
        private readonly UpdateSsomaRequirement _updateSsomaRequirement = updateSsomaRequirement;
        private readonly DeleteSsomaRequirement _deleteSsomaRequirement = deleteSsomaRequirement;
        private readonly GetAllSsomaRequirementItem _getAllSsomaRequirementItem = getAllSsomaRequirementItem;
        private readonly GetSelectSsomaRequirement _getSelectSsomaRequirement = getSelectSsomaRequirement;

        [HttpGet]
        [Route("GetAllSsomaRequirement")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int scopeId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllSsomaRequirement.ExecuteAsync(businessId, scopeId, page, pageSize, search);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetListSsomaRequirement")]
        public async Task<IActionResult> GetAllItem(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllSsomaRequirementItem.ExecuteAsync(businessId, page, pageSize, search);
            return Ok(result);
        }


        [HttpGet]
        [Route("GetByIdSsomaRequirement")]
        public async Task<IActionResult> GetById([FromQuery] long requirementId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getByIdSsomaRequirement.ExecuteAsync(requirementId, businessId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetGeneralRequirementById")]
        public async Task<IActionResult> GetGeneralById([FromQuery] long requirementId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getGeneralRequirementById.ExecuteAsync(requirementId, businessId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Route("CreateSsomaRequirement")]
        public async Task<IActionResult> Create([FromBody] SsomaRequirementCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createSsomaRequirement.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateSsomaRequirement")]
        public async Task<IActionResult> Update([FromBody] SsomaRequirementUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updateSsomaRequirement.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("DeleteSsomaRequirement")]
        public async Task<IActionResult> Delete([FromQuery] long requirementId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _deleteSsomaRequirement.ExecuteAsync(requirementId, businessId, userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetSelectRequirement")]
        public async Task<IActionResult> GetSelect(
            [FromQuery] int scopedId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();

            var result = await _getSelectSsomaRequirement.ExecuteAsync(businessId, scopedId, search, page, pageSize);
            return Ok(result);
        }
    }
}

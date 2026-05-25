using Application.DTOs.SsomaHomologationPersonnel;
using Application.UseCases.SsomaHomologationPersonnel;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Ssoma
{
    [Route("api/[controller]")]
    [ApiController]
    public class SsomaHomologationPersonnelController(
        CreateSsomaHomologationPersonnel createSsomaHomologationPersonnel,
        CreateSsomaHomologationPersonnelOrchestrated createSsomaHomologationPersonnelOrchestrated,
        GetAllSsomaHomologationPersonnel getAllSsomaHomologationPersonnel,
        GetListAllPersonnelOperations getListAllPersonnelOperations,
        GetDetailPersonnelOperations getDetailPersonnelOperations,
        GetByIdSsomaHomologationPersonnel getByIdSsomaHomologationPersonnel,
        UpdateSsomaHomologationPersonnel updateSsomaHomologationPersonnel,
        DeleteSsomaHomologationPersonnel deleteSsomaHomologationPersonnel) : BaseController
    {
        private readonly CreateSsomaHomologationPersonnel _createSsomaHomologationPersonnel = createSsomaHomologationPersonnel;
        private readonly CreateSsomaHomologationPersonnelOrchestrated _createSsomaHomologationPersonnelOrchestrated = createSsomaHomologationPersonnelOrchestrated;
        private readonly GetAllSsomaHomologationPersonnel _getAllSsomaHomologationPersonnel = getAllSsomaHomologationPersonnel;
        private readonly GetListAllPersonnelOperations _getListAllPersonnelOperations = getListAllPersonnelOperations;
        private readonly GetDetailPersonnelOperations _getDetailPersonnelOperations = getDetailPersonnelOperations;
        private readonly GetByIdSsomaHomologationPersonnel _getByIdSsomaHomologationPersonnel = getByIdSsomaHomologationPersonnel;
        private readonly UpdateSsomaHomologationPersonnel _updateSsomaHomologationPersonnel = updateSsomaHomologationPersonnel;
        private readonly DeleteSsomaHomologationPersonnel _deleteSsomaHomologationPersonnel = deleteSsomaHomologationPersonnel;

        [HttpGet]
        [Route("GetAllSsomaHomologationPersonnel")]
        public async Task<IActionResult> GetAll(
            [FromQuery] long? operationsId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllSsomaHomologationPersonnel.ExecuteAsync(businessId, operationsId, page, pageSize, search);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetListAllPersonnelOperations")]
        public async Task<IActionResult> GetListAllPersonnelOperations(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getListAllPersonnelOperations.ExecuteAsync(businessId, page, pageSize, search);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetDetailPersonnelOperations")]
        public async Task<IActionResult> GetDetailPersonnelOperations([FromQuery] long personnelOperationsId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getDetailPersonnelOperations.ExecuteAsync(personnelOperationsId, businessId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetByIdSsomaHomologationPersonnel")]
        public async Task<IActionResult> GetById([FromQuery] long homologationPersonnelId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getByIdSsomaHomologationPersonnel.ExecuteAsync(homologationPersonnelId, businessId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Route("CreateSsomaHomologationPersonnel")]
        public async Task<IActionResult> Create([FromBody] SsomaHomologationPersonnelCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createSsomaHomologationPersonnel.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPost]
        [Route("CreateSsomaHomologationPersonnelOrchestrated")]
        public async Task<IActionResult> CreateOrchestrated([FromBody] SsomaHomologationPersonnelCreateOrchestratedDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createSsomaHomologationPersonnelOrchestrated.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateSsomaHomologationPersonnel")]
        public async Task<IActionResult> Update([FromBody] SsomaHomologationPersonnelUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updateSsomaHomologationPersonnel.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("DeleteSsomaHomologationPersonnel")]
        public async Task<IActionResult> Delete([FromQuery] long homologationPersonnelId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _deleteSsomaHomologationPersonnel.ExecuteAsync(homologationPersonnelId, businessId, userId);
            return Ok(result);
        }
    }
}

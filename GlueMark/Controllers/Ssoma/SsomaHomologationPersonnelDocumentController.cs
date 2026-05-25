using Application.DTOs.SsomaHomologationPersonnelDocument;
using Application.UseCases.SsomaHomologationPersonnelDocument;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Ssoma
{
    [Route("api/[controller]")]
    [ApiController]
    public class SsomaHomologationPersonnelDocumentController(
        CreateSsomaHomologationPersonnelDocument createSsomaHomologationPersonnelDocument,
        GetAllSsomaHomologationPersonnelDocument getAllSsomaHomologationPersonnelDocument,
        GetByIdSsomaHomologationPersonnelDocument getByIdSsomaHomologationPersonnelDocument,
        UpdateSsomaHomologationPersonnelDocument updateSsomaHomologationPersonnelDocument,
        ReplaceSsomaHomologationPersonnelDocument replaceSsomaHomologationPersonnelDocument,
        DeleteSsomaHomologationPersonnelDocument deleteSsomaHomologationPersonnelDocument) : BaseController
    {
        private readonly CreateSsomaHomologationPersonnelDocument _createSsomaHomologationPersonnelDocument = createSsomaHomologationPersonnelDocument;
        private readonly GetAllSsomaHomologationPersonnelDocument _getAllSsomaHomologationPersonnelDocument = getAllSsomaHomologationPersonnelDocument;
        private readonly GetByIdSsomaHomologationPersonnelDocument _getByIdSsomaHomologationPersonnelDocument = getByIdSsomaHomologationPersonnelDocument;
        private readonly UpdateSsomaHomologationPersonnelDocument _updateSsomaHomologationPersonnelDocument = updateSsomaHomologationPersonnelDocument;
        private readonly ReplaceSsomaHomologationPersonnelDocument _replaceSsomaHomologationPersonnelDocument = replaceSsomaHomologationPersonnelDocument;
        private readonly DeleteSsomaHomologationPersonnelDocument _deleteSsomaHomologationPersonnelDocument = deleteSsomaHomologationPersonnelDocument;

        [HttpGet]
        [Route("GetAllSsomaHomologationPersonnelDocument")]
        public async Task<IActionResult> GetAll(
            [FromQuery] long? homologationPersonnelId = null,
            [FromQuery] int? requirementId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllSsomaHomologationPersonnelDocument.ExecuteAsync(
                businessId,
                homologationPersonnelId,
                requirementId,
                page,
                pageSize,
                search);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetByIdSsomaHomologationPersonnelDocument")]
        public async Task<IActionResult> GetById([FromQuery] long ssomaHomologationPersonnelDocumentId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getByIdSsomaHomologationPersonnelDocument.ExecuteAsync(
                ssomaHomologationPersonnelDocumentId,
                businessId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Route("CreateSsomaHomologationPersonnelDocument")]
        public async Task<IActionResult> Create([FromBody] SsomaHomologationPersonnelDocumentCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createSsomaHomologationPersonnelDocument.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateSsomaHomologationPersonnelDocument")]
        public async Task<IActionResult> Update([FromBody] SsomaHomologationPersonnelDocumentUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updateSsomaHomologationPersonnelDocument.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPut]
        [Route("ReplaceSsomaHomologationPersonnelDocument")]
        public async Task<IActionResult> Replace([FromBody] SsomaHomologationPersonnelDocumentReplaceRequestDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _replaceSsomaHomologationPersonnelDocument.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("DeleteSsomaHomologationPersonnelDocument")]
        public async Task<IActionResult> Delete([FromQuery] long ssomaHomologationPersonnelDocumentId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _deleteSsomaHomologationPersonnelDocument.ExecuteAsync(
                ssomaHomologationPersonnelDocumentId,
                businessId,
                userId);
            return Ok(result);
        }
    }
}

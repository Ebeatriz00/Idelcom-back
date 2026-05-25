using Application.DTOs.SomaDocumentType;
using Application.UseCases.SsomaDocumentType;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SsomaDocumentTypeController : ControllerBase
    {
        private readonly CreateSsomaDocumentType _createSsomaDocumentType;
        private readonly GetAllSsomaDocumentType _getAllSsomaDocumentType;
        private readonly GetByIdSsomaDocumentType _getByIdSsomaDocumentType;
        private readonly UpdateSsomaDocumentType _updateSsomaDocumentType;
        private readonly GetSelectSsomaDocumentType _getSelectSsomaDocumentType;
        private readonly PatchSsomaDocumentType _patchSsomaDocumentType;

        public SsomaDocumentTypeController(
            CreateSsomaDocumentType createSsomaDocumentType,
            GetAllSsomaDocumentType getAllSsomaDocumentType,
            GetByIdSsomaDocumentType getByIdSsomaDocumentType,
            UpdateSsomaDocumentType updateSsomaDocumentType,
            GetSelectSsomaDocumentType getSelectSsomaDocumentType,
            PatchSsomaDocumentType patchSsomaDocumentType)
        {
            _createSsomaDocumentType = createSsomaDocumentType;
            _getAllSsomaDocumentType = getAllSsomaDocumentType;
            _getByIdSsomaDocumentType = getByIdSsomaDocumentType;
            _updateSsomaDocumentType = updateSsomaDocumentType;
            _getSelectSsomaDocumentType = getSelectSsomaDocumentType;
            _patchSsomaDocumentType = patchSsomaDocumentType;
        }

        [HttpPost]
        [Route("SsomaDocumentTypeCreate")]
        public async Task<IActionResult> Create([FromBody] SsomaDocumentTypeCreateDto dto)
        {
            var result = await _createSsomaDocumentType.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("SsomaDocumentTypeList")]
        public async Task<IActionResult> GetList(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] long? usersBy = null)
        {
            var result = await _getAllSsomaDocumentType.ExecuteAsync(business_id, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de documento SSOMA." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("SsomaDocumentTypeSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectSsomaDocumentType.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("SsomaDocumentTypeIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long ssomaDocumentTypeId)
        {
            var result = await _getByIdSsomaDocumentType.ExecuteAsync(ssomaDocumentTypeId);
            if (result == null)
                return NotFound(new { message = "No se encontró el tipo de documento SSOMA." });

            return Ok(result);
        }

        [HttpPut]
        [Route("SsomaDocumentTypeUpdate")]
        public async Task<IActionResult> Update([FromBody] SsomaDocumentTypeUpdateDto dto)
        {
            var result = await _updateSsomaDocumentType.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("SsomaDocumentTypeStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] SsomaDocumentTypeStatusToogleDto dto)
        {
            var result = await _patchSsomaDocumentType.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

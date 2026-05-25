using Application.DTOs.DocumentType;
using Application.UseCases.Area;
using Application.UseCases.DocumentType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlueMark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentTypesController : ControllerBase
    {
        private readonly CreateDocumentType _createDocumentType;
        private readonly GetAllDocumentTypes _getAllDocumentTypes;
        private readonly GetByIdDocumentTypes _getByIdDocumentTypes;
        private readonly UpdateDocumentType _updateDocumentTypes;
        private readonly PatchDocumentTypeStatus _patchDocumentTypeStatus;
        private readonly GetSelectDocumentType _getSelectDocumentType;

        public DocumentTypesController(CreateDocumentType createDocumentType, GetAllDocumentTypes getAllDocumentTypes, GetByIdDocumentTypes getByIdDocumentTypes, UpdateDocumentType updateDocumentTypes, PatchDocumentTypeStatus patchDocumentTypeStatus, GetSelectDocumentType getSelectDocumentType)
        {
            _createDocumentType = createDocumentType;
            _getAllDocumentTypes = getAllDocumentTypes;
            _getByIdDocumentTypes = getByIdDocumentTypes;
            _updateDocumentTypes = updateDocumentTypes;
            _patchDocumentTypeStatus = patchDocumentTypeStatus;
            _getSelectDocumentType = getSelectDocumentType;
        }

        [HttpPost]
        [Route("DocumentTypesCreate")]
        public async Task<IActionResult> Create([FromBody] DocumentTypeCreateDto dto)
        {
            var result = await _createDocumentType.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("DocumentTypesList")]
        public async Task<IActionResult> GetList([FromQuery] int business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllDocumentTypes.ExecuteAsync(business_id,search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de documentos." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("DocumentTypeSelect")] 
        public async Task<IActionResult> DocumentTypeSelect(
        [FromQuery] long business_id,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectDocumentType.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }


        [HttpGet]
        [Route("DocumentTypeIdList")] 
        public async Task<IActionResult> GetListId([FromQuery] int documentTypeId)
        {
            var result = await _getByIdDocumentTypes.ExecuteAsync(documentTypeId);
            if (result == null)
                return NotFound(new { message = "No se encontraron tipos de documentos." });

            return Ok(result);
        }

        [HttpPut]
        [Route("DocumentTypesUpdate")]
        public async Task<IActionResult> Update([FromBody] DocumentTypeUpdateDto dto)
        {
            var result = await _updateDocumentTypes.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("DocumentTypesStatus")]
        public async Task<IActionResult> Patch([FromBody] DocumentTypeStatusToggleDto dto)
        {
            var result = await _patchDocumentTypeStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
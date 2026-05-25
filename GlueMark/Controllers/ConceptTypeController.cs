using Application.DTOs.ConceptType;
using Application.UseCases.ConceptType;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace GlueMark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConceptTypeController : ControllerBase
    {
        private readonly CreateConceptType _createConceptType;
        private readonly GetAllConceptType _getAllConceptTypes;
        private readonly GetByIdConceptType _getByIdConceptType;
        private readonly UpdateConceptType _updateConceptType;
        private readonly PatchConceptTypeStatus _patchConceptTypeStatus;
        private readonly GetSelectConceptType _getSelectConceptType;

        public ConceptTypeController(
            CreateConceptType createConceptType,
            GetAllConceptType getAllConceptTypes,
            GetByIdConceptType getByIdConceptType,
            UpdateConceptType updateConceptType,
            PatchConceptTypeStatus patchConceptTypeStatus,
            GetSelectConceptType getSelectConceptType)
        {
            _createConceptType = createConceptType;
            _getAllConceptTypes = getAllConceptTypes;
            _getByIdConceptType = getByIdConceptType;
            _updateConceptType = updateConceptType;
            _patchConceptTypeStatus = patchConceptTypeStatus;
            _getSelectConceptType = getSelectConceptType;
        }


        [HttpPost]
        [Route("ConceptTypeCreate")]
        public async Task<IActionResult> Create([FromBody] ConceptTypeCreateDto dto)
        {
            var result = await _createConceptType.ExecuteAsync(dto);
            return Ok(result);
        }


        [HttpGet]
        [Route("ConceptTypeList")]
        public async Task<IActionResult> GetList(
            [FromQuery] long business_id, 
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _getAllConceptTypes.ExecuteAsync(business_id, search, page, pageSize);

            
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de conceptos." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }


        [HttpGet]
        [Route("ConceptTypeSelect")]
        public async Task<IActionResult> ConceptTypeSelect(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectConceptType.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("ConceptTypeIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long ConceptTypeId) 
        {
            var result = await _getByIdConceptType.ExecuteAsync(ConceptTypeId);

            if (result == null)
                return NotFound(new { message = "No se encontró el tipo de concepto solicitado." });

            return Ok(result);
        }

        [HttpPut]
        [Route("ConceptTypeUpdate")]
        public async Task<IActionResult> Update([FromBody] ConceptTypeUpdateDto dto)
        {
            var result = await _updateConceptType.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("ConceptTypeStatus")]
        public async Task<IActionResult> Patch([FromBody] ConceptTypeStatusToggleDto dto)
        {
            var result = await _patchConceptTypeStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

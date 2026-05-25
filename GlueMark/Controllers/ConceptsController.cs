using Application.DTOs.Concepts;
using Application.UseCases.AccountPlan;
using Application.UseCases.Concepts;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
        [Authorize]
        [Route("api/[controller]")]
        [ApiController]
      
        public class ConceptsController : ControllerBase
        {
            private readonly CreateConcepts _createConcepts;
            private readonly GetAllConcepts _getAllConcepts;
            private readonly GetConceptsById _getConceptsById;
            private readonly UpdateConcepts _updateConcepts;
            private readonly PatchConceptsStatus _patchConceptsStatus;
            private readonly GetSelectConcepts _getSelectConcepts;

            public ConceptsController(
                CreateConcepts createConcepts,
                GetAllConcepts getAllConcepts,
                GetConceptsById getConceptsById,
                UpdateConcepts updateConcepts,
                PatchConceptsStatus patchConceptsStatus,
                GetSelectConcepts getSelectConcepts)
            {
                _createConcepts = createConcepts;
                _getAllConcepts = getAllConcepts;
                _getConceptsById = getConceptsById;
                _updateConcepts = updateConcepts;
                _patchConceptsStatus = patchConceptsStatus;
                _getSelectConcepts = getSelectConcepts;
            }

            [HttpPost]
            [Route("ConceptsCreate")]
            public async Task<IActionResult> Create([FromBody] ConceptsCreateDto dto)
            {
                var result = await _createConcepts.ExecuteAsync(dto);
                return Ok(result);
            }

            [Authorize]
            [HttpGet]
            [Route("ConceptsList")]
            public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
            {
                var result = await _getAllConcepts.ExecuteAsync(business_id, search, page, pageSize, usersBy);
                if (result == null || !result.Items.Any())
                    return NotFound(new { message = "No se encontraron conceptos." });

                Response.Headers["X-Total-Count"] = result.Total.ToString();
                Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

                return Ok(result);
            }

            [HttpGet]
            [Route("ConceptsSelect")]
            public async Task<IActionResult> ConceptsSelect(
                [FromQuery] long business_id,
                [FromQuery] string? search = null,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20)
            {
                var result = await _getSelectConcepts.ExecuteAsync(business_id, search, page, pageSize);
                return Ok(result);
            }

            [HttpGet]
            [Route("ConceptsById")]
            public async Task<IActionResult> GetById([FromQuery] int conceptsId)
            {
                var result = await _getConceptsById.ExecuteAsync(conceptsId);
                if (result == null)
                    return NotFound(new { message = "No se encontro el concepto." });
                return Ok(result);
            }

        [HttpPut]
            [Route("ConceptsUpdate")]
            public async Task<IActionResult> Update([FromBody] ConceptsUpdateDto dto)
            {
                var result = await _updateConcepts.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpPatch]
            [Route("ConceptsStatus")]
            public async Task<IActionResult> PatchStatus([FromBody] ConceptsStatusToggleDto dto)
            {
                var result = await _patchConceptsStatus.ExecuteAsync(dto);
                return Ok(result);
            }
        }
    }


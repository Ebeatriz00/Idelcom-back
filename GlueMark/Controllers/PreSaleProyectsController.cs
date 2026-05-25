using Application.DTOs.Opportunities;
using Application.DTOs.PreSaleProyects;
using Application.UseCases.Opportunities;
using Application.UseCases.PreSaleProyects;
using Application.UseCases.PreSaleProyects.Application.UseCases.PreSaleProyects;
using Azure;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    namespace Idelcom.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class PreSaleProyectsController : ControllerBase
        {
            private readonly CreatePreSaleProyects _createPreSaleProyects;
            private readonly GetAllPreSaleProyects _getAllPreSaleProyects;
            private readonly GetPreSaleProyectsById _getPreSaleProyectsById;
            private readonly UpdatePreSaleProyects _updatePreSaleProyects;
            private readonly PatchPreSaleProyectsStatus _patchPreSaleProyectsStatus;
            private readonly GetSelectPreSaleProyects _getSelectPreSaleProyects;
            private readonly GetNumPreSaleProyects _getNumPreSaleProyects;
            private readonly GetDetailPreSaleProyects _getDetailPreSaleProyects;
            private readonly UpdateResponsiblePreSaleProject _updateResponsiblePreSaleProject;
            private readonly UpdateStatePreSaleProyects _updateStatePreSaleProyects;
            private readonly ILinkTokenService _linkToken;

            public PreSaleProyectsController(
                CreatePreSaleProyects createPreSaleProyects,
                GetAllPreSaleProyects getAllPreSaleProyects,
                GetPreSaleProyectsById getPreSaleProyectsById,
                UpdatePreSaleProyects updatePreSaleProyects,
                PatchPreSaleProyectsStatus patchPreSaleProyectsStatus,
                GetSelectPreSaleProyects getSelectPreSaleProyects,
                GetNumPreSaleProyects getNumPreSaleProyects,
                GetDetailPreSaleProyects getDetailPreSaleProyects,
                UpdateResponsiblePreSaleProject updateResponsiblePreSaleProject,
                UpdateStatePreSaleProyects updateStatePreSaleProyects,
                ILinkTokenService linkToken
                )
            {
                _createPreSaleProyects = createPreSaleProyects;
                _getAllPreSaleProyects = getAllPreSaleProyects;
                _getPreSaleProyectsById = getPreSaleProyectsById;
                _updatePreSaleProyects = updatePreSaleProyects;
                _patchPreSaleProyectsStatus = patchPreSaleProyectsStatus;
                _getSelectPreSaleProyects = getSelectPreSaleProyects;
                _getNumPreSaleProyects = getNumPreSaleProyects;
                _getDetailPreSaleProyects = getDetailPreSaleProyects;
                _updateResponsiblePreSaleProject = updateResponsiblePreSaleProject;
                _updateStatePreSaleProyects = updateStatePreSaleProyects;
                _linkToken = linkToken;
            }

            [HttpPost]
            [Route("PreSaleProyectsCreate")]
            public async Task<IActionResult> Create([FromBody] PreSaleProyectsCreateDto dto)
            {
                var result = await _createPreSaleProyects.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpGet]
            [Route("PreSaleProyectsCode")]
            public async Task<IActionResult> GetExistsCode([FromQuery] int businessId)
            {
                var result = await _getNumPreSaleProyects.ExecuteAsync(businessId);
                return Ok(result);
            }





            [HttpGet]
            [Route("PreSaleProyectsList")]
            public async Task<IActionResult> GetList(
                [FromQuery] long business_id,
                [FromQuery] string? search = null,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10,
                [FromQuery] long? workerId = null,
                [FromQuery] string? filterCode = null,
                [FromQuery] string? filterProject = null,
                [FromQuery] string? filterClient = null,
                [FromQuery] string? filterSeller = null,
                [FromQuery] string? filterResponsible = null,
                [FromQuery] string? filterStatePreSale = null,
                [FromQuery] string? filterStateOpportunity = null,
                [FromQuery] string? filterFinishDate = null,
                [FromQuery] DateTime? filterDateFrom = null,
                [FromQuery] DateTime? filterDateTo = null,
                [FromQuery] string? opporNum = null,
                [FromQuery] long? usersId = null,
                [FromQuery] string? sortBy = null,
                [FromQuery] string? sortDirection = null,
                [FromQuery] long? stateId = null,
                [FromQuery] int? category = null,
                [FromQuery] string? quoDate = null

            )
            {
                var result = await _getAllPreSaleProyects.ExecuteAsync(
                    business_id,
                    search,
                    page,
                    pageSize,
                    workerId,
                    filterCode,
                    filterProject,
                    filterClient,
                    filterSeller,
                    filterResponsible,
                    filterStatePreSale,
                    filterStateOpportunity,
                    filterFinishDate,
                    filterDateFrom,
                    filterDateTo,
                    opporNum,
                    usersId, 
                    sortBy,
                    sortDirection,
                    stateId,
                    category,
                    quoDate
                );

                if (result == null || !result.Items.Any())
                    return NotFound(new { message = "No se encontraron proyectos." });

                Response.Headers["X-Total-Count"] = result.Total.ToString();
                Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

                return Ok(result);
            }

            [HttpGet]
            [Route("PreSaleProyectsById")]
            public async Task<IActionResult> GetById([FromQuery] string linkToken)
            {
                if (!_linkToken.TryValidate(linkToken, out var entity, out var resourceId))
                    return Unauthorized("Token inválido o expirado.");

                if (entity != "opportunity")
                    return BadRequest("Token no pertenece a proyecto.");

                var projectToken = Convert.ToString(resourceId);

                var result = await _getPreSaleProyectsById.ExecuteAsync(projectToken);
                if (result == null)
                    return NotFound(new { message = "No se encontró el proyecto." });
                return Ok(result);
            }

            [HttpPut]
            [Route("PreSaleProyectsUpdate")]
            public async Task<IActionResult> Update([FromBody] PreSaleProyectsUpdateDto dto)
            {
                if (string.IsNullOrWhiteSpace(dto.LinkToken))
                    return Unauthorized("Falta token.");

                if (!_linkToken.ValidateToken(dto.LinkToken, out var claims, out var entity, out var resourceId))
                    return Unauthorized("Token inválido o expirado.");

                if (!string.Equals(entity, "preSaleProject", StringComparison.OrdinalIgnoreCase))
                    return BadRequest("Token no pertenece a proyecto.");

                dto.LinkToken = resourceId;

                var result = await _updatePreSaleProyects.ExecuteAsync(dto);
                if (result == null)
                    return NotFound(new { message = "No se encontró el proyecto para actualizar." });
                return Ok(result);
            }

            [HttpPatch]
            [Route("PreSaleProyectsStatus")]
            public async Task<IActionResult> PatchStatus([FromBody] PreSaleProyectsStatusToggleDto dto)
            {
                if (string.IsNullOrWhiteSpace(dto.LinkToken))
                    return Unauthorized("Falta token.");

                if (!_linkToken.ValidateToken(dto.LinkToken, out var claims, out var entity, out var resourceId))
                    return Unauthorized("Token inválido o expirado.");

                if (!string.Equals(entity, "preSaleProject", StringComparison.OrdinalIgnoreCase))
                    return BadRequest("Token no pertenece a proyecto.");

                dto.LinkToken = resourceId;

                var result = await _patchPreSaleProyectsStatus.ExecuteAsync(dto);
                return Ok(result);
            }
        

            [HttpGet]
            [Route("PreSaleProyectsDetail")]
            public async Task<IActionResult> GetDetail([FromQuery] string linkToken, [FromQuery] long businessId, CancellationToken ct)
            {
                try
                {
                    if (!_linkToken.TryValidate(linkToken, out var entity, out var resourceId))
                        return Unauthorized("Token inválido o expirado.");

                    if (entity != "opportunity")
                        return BadRequest("Token no pertenece a proyectos.");

                    var projectToken = Convert.ToString(resourceId);
                    var result = await _getDetailPreSaleProyects.ExecuteAsync(projectToken, businessId, ct);
                    if (result == null)
                        return NotFound(new { message = "No se encontró el proyecto." });

                    return Ok(result);
                }
                catch (OperationCanceledException)
                {
                    return StatusCode(499, new { message = "La solicitud fue cancelada." });
                }
            }

            [HttpPut]
            [Route("UpdateResponsible")]
            public async Task<IActionResult> UpdateResponsible([FromBody] PreSaleProyectsUpdateResponsibleDto dto)
            {
                var result = await _updateResponsiblePreSaleProject.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpPut]
            [Route("ProjectsUpdateState")]
            public async Task<IActionResult> UpdateState([FromBody] PreSaleProjectsUpdateStateDto dto)
            {
                if (string.IsNullOrWhiteSpace(dto.LinkToken))
                    return Unauthorized("Falta token.");

                if (!_linkToken.ValidateToken(dto.LinkToken, out var claims, out var entity, out var resourceId))
                    return Unauthorized("Token inválido o expirado.");

                if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                    return BadRequest("Token no pertenece a oportunidad.");

                dto.LinkToken = resourceId;

                var result = await _updateStatePreSaleProyects.ExecuteAsync(dto);
                if (result == null)
                    return NotFound(new { message = "No se encontró el proyecto para actualizar el estado." });
                return Ok(result);
            }


        }

    }
} 

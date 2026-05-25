using Application.DTOs.Opportunities;
using Application.UseCases.Business;
using Application.UseCases.ConctactsCrm;
using Application.UseCases.Opportunities;
using Core.Interfaces.Services;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpportunitiesController : ControllerBase
    {
        private readonly CreateOpportunities _createOpportunities;
        private readonly GetAllOpportunities _getAllOpportunities;
        private readonly GetNumOpportunities _getNumOpportunities;
        private readonly GetDetailOpportunities _getDetailOpportunities;
        private readonly GetSelectOpportunities _getSelectOpportunities;
        private readonly GetByIdClientsOpportunities _getByIdClientsOpportunities;
        private readonly GetByIdOpportunities _getByIdOpportunities;
        private readonly GetByIdStateOpportunities _getByIdStateOpportunities;
        private readonly UpdateClientsOpportunities _updateClientsOpportunities;
        private readonly UpdateStateOpportunities _updateStateOpportunities;
        private readonly UpdateOpportunities _updateOpportunities;
        private readonly PatchOpportunities _patchOpportunities;
        private readonly GetSelectDeliverables _getSelectDeliverables;
        private readonly GetSelectDeliverablesHiring _getSelectDeliverablesHiring;
        private readonly UpdateDeliverablesOpportunities _updateDeliverablesOpportunities;
        private readonly UploadNewVerOpportunities _uploadNewVerOpportunities;
        private readonly GetSelectVerQuo _getSelectVerQuo;
        private readonly AttachHiringFiles _attachHiringFiles;
        private readonly GetSelectFlowType _getSelectFlowType;
        private readonly ILinkTokenService _linkToken;
        public OpportunitiesController(CreateOpportunities createOpportunities, GetAllOpportunities getAllOpportunities, GetNumOpportunities getNumOpportunities, GetDetailOpportunities getDetailOpportunities, GetSelectOpportunities getSelectOpportunities, ILinkTokenService linkToken, GetByIdClientsOpportunities getByIdClientsOpportunities, GetByIdOpportunities getByIdOpportunities, GetByIdStateOpportunities getByIdStateOpportunities, UpdateClientsOpportunities updateClientsOpportunities, UpdateStateOpportunities updateStateOpportunities, UpdateOpportunities updateOpportunities, PatchOpportunities patchOpportunities, GetSelectDeliverables getSelectDeliverables, UpdateDeliverablesOpportunities updateDeliverablesOpportunities, GetSelectDeliverablesHiring getSelectDeliverablesHiring, UploadNewVerOpportunities uploadNewVerOpportunities, GetSelectVerQuo getSelectVerQuo, AttachHiringFiles attachHiringFiles, GetSelectFlowType getSelectFlowType)
        {
            _createOpportunities = createOpportunities;
            _getAllOpportunities = getAllOpportunities;
            _getNumOpportunities = getNumOpportunities;
            _getDetailOpportunities = getDetailOpportunities;
            _getSelectOpportunities = getSelectOpportunities;
            _linkToken = linkToken;
            _getByIdClientsOpportunities = getByIdClientsOpportunities;
            _getByIdOpportunities = getByIdOpportunities;
            _getByIdStateOpportunities = getByIdStateOpportunities;
            _updateClientsOpportunities = updateClientsOpportunities;
            _updateStateOpportunities = updateStateOpportunities;
            _updateOpportunities = updateOpportunities;
            _patchOpportunities = patchOpportunities;
            _getSelectDeliverables = getSelectDeliverables;
            _updateDeliverablesOpportunities = updateDeliverablesOpportunities;
            _getSelectDeliverablesHiring = getSelectDeliverablesHiring;
            _uploadNewVerOpportunities = uploadNewVerOpportunities;
            _getSelectVerQuo = getSelectVerQuo;
            _attachHiringFiles = attachHiringFiles;
            _getSelectFlowType = getSelectFlowType;
        }

        [HttpPost]
        [Route("OpportunitiesCreate")]
        public async Task<IActionResult> Create([FromBody] OpportunitiesCreateDto dto)
        {
            var result = await _createOpportunities.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPut]
        [Route("Opportunities/{opporId}/HiringFiles")]
        public async Task<IActionResult> AttachHiringFiles(
        long opporId,
        [FromBody] AttachHiringFilesDto dto
    )
            {
                if (opporId != dto.OpporId)
                    return BadRequest("El OPPOR_ID no coincide.");

                var result = await _attachHiringFiles.ExecuteAsync(
                    dto.BusinessId,
                    dto.OpporId,
                    dto.UpdateUser,
                    dto.Files
                );

                return Ok(result);
            }

        [HttpGet]
        [Route("OpportunitiesCode")]
        public async Task<IActionResult> GetExistsCode([FromQuery] int businessId)
        {
            var result = await _getNumOpportunities.ExecuteAsync(businessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("OpportunitiesList")]
        public async Task<IActionResult> GetList([FromQuery] long businessId, [FromQuery] long? usersId = null, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? stateId = null, [FromQuery] long? workerId = null, [FromQuery] DateTime? filterStartDate = null, [FromQuery] DateTime? filterFinishDate = null, [FromQuery] int? filterYear = null)
        {
            var result = await _getAllOpportunities.ExecuteAsync(businessId, search, page, pageSize, usersId, stateId, workerId, filterStartDate, filterFinishDate, filterYear);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron oportunidades." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("OpportunitiesSelect")]
        public async Task<IActionResult> OpportunitiesSelect(
            [FromQuery] long businessId,
            [FromQuery] long clientsId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectOpportunities.ExecuteAsync(businessId, clientsId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("FlowTypeSelect")]
        public async Task<IActionResult> FlowTypeSelect(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectFlowType.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }


        [HttpGet]
        [Route("OpportunitiesById")]
        public async Task<IActionResult> GetById([FromQuery] string linkToken)
        {
            if (!_linkToken.TryValidate(linkToken, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (entity != "opportunity")
                return BadRequest("Token no pertenece a Oportunidades.");

            var opporToken = Convert.ToString(resourceId);

            var result = await _getByIdOpportunities.ExecuteAsync(opporToken);
            if (result == null)
                return NotFound(new { message = "No se encontró la oportunidad." });
            return Ok(result);
        }

        [HttpGet]
        [Route("OpportunitiesByIdClient")]
        public async Task<IActionResult> GetByIdClient([FromQuery] string linkToken)
        {
            if (!_linkToken.TryValidate(linkToken, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (entity != "opportunity")
                return BadRequest("Token no pertenece a Oportunidades.");

            var opporToken = Convert.ToString(resourceId);

            var result = await _getByIdClientsOpportunities.ExecuteAsync(opporToken);
            if (result == null)
                return NotFound(new { message = "No se encontraron oportunidades para el cliente." });
            return Ok(result);
        }

        [HttpGet]
        [Route("OpportunitiesByIdState")]
        public async Task<IActionResult> GetByIdState([FromQuery] string linkToken)
        {
            if (!_linkToken.TryValidate(linkToken, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (entity != "opportunity")
                return BadRequest("Token no pertenece a Oportunidades.");

            var opporToken = Convert.ToString(resourceId);
            var result = await _getByIdStateOpportunities.ExecuteAsync(opporToken);
            if (result == null)
                return NotFound(new { message = "No se encontraron oportunidades para el estado." });
            return Ok(result);
        }

        [HttpPut]
        [Route("OpportunitiesUpdate")]
        public async Task<IActionResult> Update([FromBody] OpportunitiesUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.LinkToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a oportunidad.");

            dto.LinkToken = resourceId;

            var result = await _updateOpportunities.ExecuteAsync(dto);
            if (result == null)
                return NotFound(new { message = "No se encontró la oportunidad para actualizar." });
            return Ok(result);
        }
        [HttpPut]
        [Route("OpportunitiesUpdateClient")]
        public async Task<IActionResult> UpdateClient([FromBody] OpportunitiesClientsUpdateDto dto)
        {
            var result = await _updateClientsOpportunities.ExecuteAsync(dto);
            if (result == null)
                return NotFound(new { message = "No se encontró la oportunidad para actualizar el cliente." });
            return Ok(result);
        }
        [HttpPut]
        [Route("OpportunitiesUpdateState")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(30_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 30_000_000)]
        public async Task<IActionResult> UpdateState([FromForm] OpportunitiesStateUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.LinkToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a oportunidad.");

            dto.LinkToken = resourceId;

            var result = await _updateStateOpportunities.ExecuteAsync(dto);
            if (result == null)
                return NotFound(new { message = "No se encontró la oportunidad para actualizar el estado." });
            return Ok(result);
        }

        [HttpPut]
        [Route("UploadQuotationNewVer")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(30_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 30_000_000)]
        public async Task<IActionResult> UploadQuotationNewVer([FromForm] OpportunitiesUploadNewVerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.LinkToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a oportunidad.");

            dto.LinkToken = resourceId;

            var result = await _uploadNewVerOpportunities.ExecuteAsync(dto);
            if (result == null)
                return NotFound(new { message = "No se encontró la oportunidad para actualizar el estado." });
            return Ok(result);
        }

        [HttpGet]
        [Route("SelectQuotatationVerNo")]
        public async Task<IActionResult> SelectQuotatationVerNo(
            [FromQuery] long businessId,
            [FromQuery] string linkToken,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(linkToken))
                return Unauthorized("Falta token.");
            if (!_linkToken.ValidateToken(linkToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a oportunidad.");

            linkToken = resourceId;

            var result = await _getSelectVerQuo.ExecuteAsync(businessId, linkToken, search, page, pageSize);
            return Ok(result);
        }

        [HttpPatch]
        [Route("OpportunitiesStatus")]
        public async Task<IActionResult> Patch([FromBody] OpportunitiesStatusToggleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.LinkToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a oportunidad.");

            dto.LinkToken = resourceId;

            var result = await _patchOpportunities.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("OpportunitiesDetail")]
        public async Task<IActionResult> GetDetail([FromQuery] string linkToken, [FromQuery] long businessId, CancellationToken ct, [FromQuery] long? usersBy = null)
        {
            try
            {
                if (!_linkToken.TryValidate(linkToken, out var entity, out var resourceId))
                    return Unauthorized("Token inválido o expirado.");

                if (entity != "opportunity")
                    return BadRequest("Token no pertenece a Oportunidades.");

                var opporToken = Convert.ToString(resourceId);
                var result = await _getDetailOpportunities.ExecuteAsync(opporToken, businessId, usersBy, ct);
                if (result == null)
                    return NotFound(new { message = "No se encontró la oportunidad." });

                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, new { message = "La solicitud fue cancelada." });
            }
        }
        //*=============================DELIVERABLES=================================*//

        [HttpGet]
        [Route("OpportunitiesSelectDeliverables")]
        public async Task<IActionResult> OpportunitiesSelectDeliverables(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectDeliverables.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("OpportunitiesSelectDeliverablesHiring")]
        public async Task<IActionResult> OpportunitiesSelectDeliverablesHiring(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectDeliverablesHiring.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpPut]
        [Route("OpportunitiesUpdateDeliverables")]
        public async Task<IActionResult> UpdateDeliverables([FromBody] OpportunitiesStateUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.LinkToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a oportunidad.");

            dto.LinkToken = resourceId;

            var result = await _updateDeliverablesOpportunities.ExecuteAsync(dto);
            if (result == null)
            {
                return NotFound(new { message = "No se encontró la oportunidad para actualizar los entregables." });
            }

            if (result.Status == 0)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}

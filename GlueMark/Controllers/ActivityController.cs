using Application.DTOs.Activity;
using Application.DTOs.Tasks;
using Application.UseCases.Activity;
using Application.UseCases.Concepts;
using Application.UseCases.FileTracking;
using Application.UseCases.Tasks;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly CreateActivityOppor _createActivityOppor;
        private readonly DeleteActivityOppor _deleteActivityOppor;
        private readonly PatchChangeActivityPriorityOppor _patchChangeActivityPriorityOppor;
        private readonly PatchChangeActivityStateOppor _patchChangeActivityStateppor;
        private readonly CreateActivityProject _createActivityProject;
        private readonly DeleteActivityProject _deleteActivityProject;  
        private readonly ILinkTokenService _linkToken;

        public ActivityController(CreateActivityOppor createActivityOppor, DeleteActivityOppor deleteActivityOppor, PatchChangeActivityPriorityOppor patchChangeActivityPriorityOppor, PatchChangeActivityStateOppor patchChangeActivityStateppor, ILinkTokenService linkToken, CreateActivityProject createActivityProject, DeleteActivityProject deleteActivityProject)
        {
            _createActivityOppor = createActivityOppor;
            _deleteActivityOppor = deleteActivityOppor;
            _patchChangeActivityPriorityOppor = patchChangeActivityPriorityOppor;
            _patchChangeActivityStateppor = patchChangeActivityStateppor;
            _linkToken = linkToken;
            _createActivityProject = createActivityProject;
            _deleteActivityProject = deleteActivityProject;
        }
        [HttpPost]
        [Route("ActivityOpporCreate")]
        public async Task<IActionResult> CreateOppor([FromBody] ActivityOpporCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.OpporToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.OpporToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");


            if (!string.Equals(entity, "opportunityDetail", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a Tareas.");

            dto.OpporToken = resourceId;
            var result = await _createActivityOppor.ExecuteAsync(dto);
            return Ok(result);

        }

        [HttpPost]
        [Route("ActivityProjectCreate")]
        public async Task<IActionResult> Create([FromBody] ActivityProjectCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ProjectToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.ProjectToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");


            if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a Tareas.");

            dto.ProjectToken = resourceId;
            var result = await _createActivityProject.ExecuteAsync(dto);
            return Ok(result);
        }



        [HttpDelete]
        [Route("ActivityOpporDelete")]
        public async Task<IActionResult> DeleteOppor([FromBody] ActivityOpporDeleteDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.OpporToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.OpporToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");


            if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a oportunidad.");


            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");


            if (!_linkToken.ValidateToken(dto.LinkToken, out var claimsL, out var entityL, out var resourceIdL))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entityL, "activity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a actividad.");

            dto.OpporToken = resourceId;
            dto.LinkToken = resourceIdL;
            var result = await _deleteActivityOppor.ExecuteAsync(dto);
            return Ok(result);
        }


        [HttpDelete]
        [Route("ActivityProjectDelete")]
        public async Task<IActionResult> DeleteProject([FromBody] ActivityDeleteProjectDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ProjectToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.ProjectToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválid o expirado.");


            if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a oportunidad.");


            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");


            if (!_linkToken.ValidateToken(dto.LinkToken, out var claimsL, out var entityL, out var resourceIdL))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entityL, "activity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a actividad.");

            dto.ProjectToken = resourceId;
            dto.LinkToken = resourceIdL;
            var result = await _deleteActivityProject.ExecuteAsync(dto);
            return Ok(result);
        }


        [HttpPatch]
        [Route("ActivityPriorityStateChange")]
        public async Task<IActionResult> PatchTasksPriorityStateChange([FromBody] ActivityPriorityOpporDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");


            if (!_linkToken.ValidateToken(dto.LinkToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");


            if (!string.Equals(entity, "activity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a Actividades.");

            if (string.IsNullOrWhiteSpace(dto.Status))
                return Unauthorized("Falta token.");


            if (!_linkToken.ValidateToken(dto.Status, out var claimsTtus, out var entityStatus, out var resourcesId))
                return Unauthorized("Token inválido o expirado.");





            dto.LinkToken = resourceId;
            dto.Status = resourcesId;
            var result = await _patchChangeActivityPriorityOppor.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPatch]
        [Route("ActivityStateChange")]
        public async Task<IActionResult> PatchTasksStateChange([FromBody] ActivityStateOpporDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");


            if (!_linkToken.ValidateToken(dto.LinkToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");


            if (string.IsNullOrWhiteSpace(dto.Status))
                return Unauthorized("Falta token.");


            if (!_linkToken.ValidateToken(dto.Status, out var claimsTtus, out var entityStatus, out var resourcesId))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entity, "activity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a Tareas.");


            dto.LinkToken = resourceId;
            dto.Status = resourcesId;

            var result = await _patchChangeActivityStateppor.ExecuteAsync(dto);
            return Ok(result);
        }

    }
}

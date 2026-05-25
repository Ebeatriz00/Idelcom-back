using Application.DTOs.FileTracking;
using Application.DTOs.Tasks;
using Application.UseCases.FileTracking;
using Application.UseCases.Tasks;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileTrackingController : ControllerBase
    {
        private readonly CreateFileTrackingOpper _createFileTrackingOpper;
        private readonly DeleteFileTrackingOpper _deleteFileTrackingOpper;
        private readonly CreateFileTrackingProject _createFileTrackingProject;
        private readonly DeleteFileTrackingProject _deleteFileTrackingProject;
        private readonly ILinkTokenService _linkToken;

        public FileTrackingController(CreateFileTrackingOpper createFileTrackingOpper, DeleteFileTrackingOpper deleteFileTrackingOpper, ILinkTokenService linkToken, CreateFileTrackingProject createFileTrackingProject, DeleteFileTrackingProject deleteFileTrackingProject)
        {
            _createFileTrackingOpper = createFileTrackingOpper;
            _deleteFileTrackingOpper = deleteFileTrackingOpper;
            _linkToken = linkToken;
            _createFileTrackingProject = createFileTrackingProject;
            _deleteFileTrackingProject = deleteFileTrackingProject;
        }
        [HttpPost]
        [Route("FileTrackingOpporCreate")]
        public async Task<IActionResult> CreateOppor([FromBody] FileTrackingOpporCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.OpporToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.OpporToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            var allowedEntities = new[] { "opportunityDetail", "projectsDetail" };

            if (!allowedEntities.Contains(entity, StringComparer.OrdinalIgnoreCase))


                return BadRequest($"Token no válido para subir archivos. Entidad recibida: {entity}");
            dto.OpporToken = resourceId;
            var result = await _createFileTrackingOpper.ExecuteAsync(dto);

            return Ok(result);
        }


        [HttpPost]
        [Route("FileTrackingProjectCreate")]
        public async Task<IActionResult> CreateProject([FromBody] FileTrackingProjectCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ProjectToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.ProjectToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");


            if (!string.Equals(entity, "projectsDetail", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a Tareas.");

            dto.ProjectToken = resourceId;
            var result = await _createFileTrackingProject.ExecuteAsync(dto);
            return Ok(result);
        }



        [HttpDelete]
        [Route("FileTrackingOpporDelete")] 
        public async Task<IActionResult> DeleteOppor([FromBody] FileTrackingOpperDeleteDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.OpporToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.OpporToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            var isOpportunity = string.Equals(entity, "opportunityDetail", StringComparison.OrdinalIgnoreCase);
            var isProject = string.Equals(entity, "projectsDetail", StringComparison.OrdinalIgnoreCase);

            if (!isOpportunity && !isProject)
            {
                return BadRequest($"Token no válido para esta operación (Entidad recibida: {entity}).");
            }
            // 

            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token del archivo.");

            if (!_linkToken.ValidateToken(dto.LinkToken, out var claimsL, out var entityL, out var resourceIdL))
                return Unauthorized("Token de archivo inválido.");

            if (!string.Equals(entityL, "fileTracking", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a archivos.");

            dto.OpporToken = resourceId;
            dto.LinkToken = resourceIdL;

            var result = await _deleteFileTrackingOpper.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpDelete]
        [Route("FileTrackingProjectDelete")]
        public async Task<IActionResult> DeleteProject([FromBody] FileTrackingProjectDeleteDto dto)
        {


            if (string.IsNullOrWhiteSpace(dto.ProjectToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.ProjectToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");


            if (!string.Equals(entity, "projectsDetail", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a Tareas.");


            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");


            if (!_linkToken.ValidateToken(dto.LinkToken, out var claimsL, out var entityL, out var resourceIdL))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entityL, "fileTracking", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a archivos.");

            dto.ProjectToken = resourceId;
            dto.LinkToken = resourceIdL;
            var result = await _deleteFileTrackingProject.ExecuteAsync(dto);
            return Ok(result);
        }


    }
}

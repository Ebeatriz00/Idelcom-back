using Core.Interfaces;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace GlueMark.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController(IStorageService storageService) : AppBaseController
    {
        private readonly IStorageService _storageService = storageService;

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetFile(Guid id)
        {
            try
            {
                var result = await _storageService.GetStreamAsync(id);
                
                // Solo guardamos en caché si la descarga fue exitosa
                Response.Headers.Append("Cache-Control", "public,max-age=86400");
                
                return File(result.Stream, result.MimeType, result.FileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new GlobalResponse { Status = 0, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new GlobalResponse { Status = 0, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string suggestedPath = "General")
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new GlobalResponse { Status = 0, Message = "No se recibió ningún archivo." });

                var userId = GetCurrentAppUserId();

                using var stream = file.OpenReadStream();
                var fileId = await _storageService.UploadAsync(stream, file.FileName, suggestedPath, userId);

                return Ok(new GlobalResponse<Guid>
                {
                    Status = 1,
                    Message = "Archivo subido exitosamente.",
                    Data = fileId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GlobalResponse { Status = 0, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            try
            {
                var success = await _storageService.CleanupAsync(id);
                if (!success)
                    return NotFound(new GlobalResponse { Status = 0, Message = "Archivo no encontrado o no se pudo eliminar." });

                return Ok(new GlobalResponse { Status = 1, Message = "Archivo eliminado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new GlobalResponse { Status = 0, Message = ex.Message });
            }
        }
    }
}

using Application.DTOs.ProfilesPermissions;
using Application.UseCases.ProfilesPermissions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesPermissionsController : ControllerBase
    {
        private readonly CreateProfilesPermissions _createProfilesPermissions;
        private readonly GetAllProfilesPermissions _getAllProfilesPermissions;
        private readonly GetByIdProfilesPermissions _getByIdProfilesPermissions;
        private readonly UpdateProfilesPermissions _updateProfilesPermissions;
        private readonly PatchProfilesPermissionsStatus _patchProfilesPermissionsStatus;
        public ProfilesPermissionsController(
            CreateProfilesPermissions createProfilesPermissions,
            GetAllProfilesPermissions getAllProfilesPermissions,
            GetByIdProfilesPermissions getByIdProfilesPermissions,
            UpdateProfilesPermissions updateProfilesPermissions,
            PatchProfilesPermissionsStatus patchProfilesPermissionsStatus)
        {
            _createProfilesPermissions = createProfilesPermissions;
            _getAllProfilesPermissions = getAllProfilesPermissions;
            _getByIdProfilesPermissions = getByIdProfilesPermissions;
            _updateProfilesPermissions = updateProfilesPermissions;
            _patchProfilesPermissionsStatus = patchProfilesPermissionsStatus;
        }
        [HttpPost]
        [Route("ProfilesPermissionsCreate")]
        public async Task<IActionResult> Create([FromBody] ProfilesPermissionsCreateDto dto)
        {
            var result = await _createProfilesPermissions.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpGet]
        [Route("ProfilesPermissionsList")]
        public async Task<IActionResult> GetList([FromQuery] long profilesId, [FromQuery] long businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllProfilesPermissions.ExecuteAsync(profilesId, businessId, page, pageSize, search);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron permisos de perfiles." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }
        [HttpGet]
        [Route("ProfilesPermissionsById")]
        public async Task<IActionResult> GetById([FromQuery] long profilesPermissionsId)
        {
            var result = await _getByIdProfilesPermissions.ExecuteAsync(profilesPermissionsId);
            if (result == null)
                return NotFound(new { message = "Permiso de perfil no encontrado." });
            return Ok(result);
        }
        [HttpPut]
        [Route("ProfilesPermissionsUpdate")]
        public async Task<IActionResult> Update([FromBody] ProfilesPermissionsUpdateDto dto)
        {
            var result = await _updateProfilesPermissions.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPatch]
        [Route("ProfilesPermissionsStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] ProfilesPermissionsStatusToggleDto dto)
        {
            var result = await _patchProfilesPermissionsStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

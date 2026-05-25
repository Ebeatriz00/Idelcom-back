using Application.DTOs.ModulePermission;
using Application.UseCases.ModulePermission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesPermissionsController : ControllerBase
    {
        private readonly CreateModulesPermissions _createModulePermission;
        private readonly GetAllModulesPermissions _getAllModulePermission;
        private readonly GetByIdModulesPermissions _getByIdModulePermission;
        private readonly UpdateModulesPermissions _updateModulePermission;
        private readonly PatchModulesPermissionsStatus _patchModulePermissionStatus;

        public ModulesPermissionsController(
            CreateModulesPermissions createModulePermission,
            GetAllModulesPermissions getAllModulePermission,
            GetByIdModulesPermissions getByIdModulePermission,
            UpdateModulesPermissions updateModulePermission,
            PatchModulesPermissionsStatus patchModulePermissionStatus)
        {
            _createModulePermission = createModulePermission;
            _getAllModulePermission = getAllModulePermission;
            _getByIdModulePermission = getByIdModulePermission;
            _updateModulePermission = updateModulePermission;
            _patchModulePermissionStatus = patchModulePermissionStatus;
        }

        [HttpPost]
        [Route("ModulesPermissionsCreate")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ModulesPermissionsCreateDto dto)
        {
            var result = await _createModulePermission.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("ModulesPermissionsList")]
        public async Task<IActionResult> GetList([FromQuery] int business_id, [FromQuery] string? search = null,[FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllModulePermission.ExecuteAsync(business_id, page, pageSize, search);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron permisos de módulos." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }

        [HttpGet]
        [Route("ModulesPermissionsById")]
        [Authorize]
        public async Task<IActionResult> GetById([FromQuery] int modulesPermissionsId)
        {
            var result = await _getByIdModulePermission.ExecuteAsync(modulesPermissionsId);
            if (result == null)
                return NotFound(new { message = "No se encontró el permiso de módulo." });

            return Ok(result);
        }

        [HttpPut]
        [Route("ModulesPermissionsUpdate")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] ModulesPermissionsUpdateDto dto)
        {
            var result = await _updateModulePermission.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("ModulesPermissionsStatus")]
        [Authorize]
        public async Task<IActionResult> PatchStatus([FromBody] ModulesPermissionsStatusToggleDto dto)
        {
            var result = await _patchModulePermissionStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

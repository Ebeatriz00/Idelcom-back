using Application.DTOs.Permissions;
using Application.UseCases.Modules;
using Application.UseCases.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
        namespace Idelcom.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class PermissionsController : ControllerBase
        {
            private readonly CreatePermissions _createPermissions;
            private readonly GetAllPermissions _getAllPermissions;
            private readonly GetSelectPermissions _getSelectPermissions;
            private readonly GetByIdPermissions _getByIdPermissions;
            private readonly UpdatePermissions _updatePermissions;
            private readonly PatchPermissionsStatus _patchPermissionsStatus;

            public PermissionsController(
                CreatePermissions createPermissions,
                GetAllPermissions getAllPermissions,
                GetByIdPermissions getByIdPermissions,
                UpdatePermissions updatePermissions,
                PatchPermissionsStatus patchPermissionsStatus,
                GetSelectPermissions getSelectPermissions)
            {
                _createPermissions = createPermissions;
                _getAllPermissions = getAllPermissions;
                _getByIdPermissions = getByIdPermissions;
                _updatePermissions = updatePermissions;
                _patchPermissionsStatus = patchPermissionsStatus;
                _getSelectPermissions = getSelectPermissions;
            }

            [HttpPost]
            [Route("PermissionsCreate")]
            [Authorize]
            public async Task<IActionResult> Create([FromBody] PermissionsCreateDto dto)
            {
                var result = await _createPermissions.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpGet]
            [Route("PermissionsList")]
            [Authorize]
            public async Task<IActionResult> GetList([FromQuery] int business_id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            {
                var result = await _getAllPermissions.ExecuteAsync(business_id, page, pageSize);
                if (result == null || !result.Items.Any())
                    return NotFound(new { message = "No se encontraron permisos." });
                Response.Headers["X-Total-Count"] = result.Total.ToString();
                Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
                return Ok(result);
            }

            [HttpGet]
            [Route("PermissionsSelect")]
            public async Task<IActionResult> PermissionsSelect(
        [FromQuery] long business_id,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
            {
                var result = await _getSelectPermissions.ExecuteAsync(business_id, search, page, pageSize);
                return Ok(result);
            }


            [HttpGet]
            [Route("PermissionsIdList")]
            [Authorize]
            public async Task<IActionResult> GetListId([FromQuery] int permissionId)
            {
                var result = await _getByIdPermissions.ExecuteAsync(permissionId);
                if (result == null)
                    return NotFound(new { message = "No se encontró el permiso." });
                return Ok(result);
            }

            [HttpPut]
            [Route("PermissionsUpdate")]
            [Authorize]
            public async Task<IActionResult> Update([FromBody] PermissionsUpdateDto dto)
            {
                var result = await _updatePermissions.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpPatch]
            [Route("PermissionsStatus")]
            [Authorize]
            public async Task<IActionResult> PatchStatus([FromBody] PermissionsStatusToggleDto dto)
            {
                var result = await _patchPermissionsStatus.ExecuteAsync(dto);
                return Ok(result);
            }
        }
    }

}

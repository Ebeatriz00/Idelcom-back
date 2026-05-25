using Application.DTOs.ParentModules;
using Application.UseCases.ParentModules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParentModulesController : ControllerBase
    {
        private readonly CreateParentModules _createParentModules;
        private readonly GetAllParentModules _getAllParentModules;
        private readonly GetByIdParentModules _getByIdParentModules;
        private readonly UpdateParentModules _updateParentModules;
        private readonly PatchParentModulesStatus _patchParentModulesStatus;

        public ParentModulesController(CreateParentModules createParentModules, GetAllParentModules getAllParentModules, GetByIdParentModules getByIdParentModules, UpdateParentModules updateParentModules, PatchParentModulesStatus patchParentModulesStatus)
        {
            _createParentModules = createParentModules;
            _getAllParentModules = getAllParentModules;
            _getByIdParentModules = getByIdParentModules;
            _updateParentModules = updateParentModules;
            _patchParentModulesStatus = patchParentModulesStatus;
        }
        [HttpPost]
        [Route("ParentModulesCreate")]
        public async Task<IActionResult> Create([FromBody] ParentModulesCreateDto dto)
        {
            var result = await _createParentModules.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("ParentModulesList")]
        public async Task<IActionResult> GetList([FromQuery] int businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllParentModules.ExecuteAsync(businessId, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron módulos padres." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }
        [HttpGet]
        [Route("ParentIdList")]
            public async Task<IActionResult> GetByBusinessId([FromQuery] int parentModulesId)
        {
            var result = await _getByIdParentModules.ExecuteAsync(parentModulesId);
            if (result == null)
                return NotFound(new { message = "No se encontraron módulos padres." });
            return Ok(result);
        }
        [HttpPut]
        [Route("ParentModulesUpdate")]
        public async Task<IActionResult> Update([FromBody] ParentModulesUpdateDto dto)
        {
            var result = await _updateParentModules.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("ParentModulesStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] ParentModulesStatusToogleDto dto)
        {
            var result = await _patchParentModulesStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

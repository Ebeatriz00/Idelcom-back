using Application.DTOs.Modules;
using Application.UseCases.Currency;
using Application.UseCases.Modules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly CreateModules _createModules;
        private readonly GetAllModules _getAllModules;
        private readonly GetSelectModules _getSelectModules;
        private readonly GetByIdModules _getByIdModules;
        private readonly UpdateModules _updateModules;
        private readonly PatchModulesStatus _patchModulesStatus;

        public ModulesController(
            CreateModules createModules,
            GetAllModules getAllModules,
            GetByIdModules getByIdModules,
            UpdateModules updateModules,
            PatchModulesStatus patchModulesStatus,
            GetSelectModules getSelectModules)
        {
            _createModules = createModules;
            _getAllModules = getAllModules;
            _getByIdModules = getByIdModules;
            _updateModules = updateModules;
            _patchModulesStatus = patchModulesStatus;
            _getSelectModules = getSelectModules;
        }

        [HttpPost]
        [Route("ModulesCreate")]
        public async Task<IActionResult> Create([FromBody] ModulesCreateDto dto)
        {
            var result = await _createModules.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("ModulesList")]
        public async Task<IActionResult> GetList([FromQuery] long businessId, [FromQuery] long parentModulesId , [FromQuery] string? search = null, [FromQuery] long? usersId = null)
        {
            var result = await _getAllModules.ExecuteAsync(businessId, parentModulesId, search, usersId);
            if (result == null || !result.Any())
                return NotFound(new { message = "No se encontraron tipos de documentos." });

            return Ok(result);
        }
        


        [HttpGet]
        [Route("ModulesSelect")]
        public async Task<IActionResult> ModulesSelect(
        [FromQuery] long business_id,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectModules.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result); 
        }

        [HttpGet]
        [Route("ModulesIdList")]
        public async Task<IActionResult> GetListId([FromQuery] int ModulesId)
        {
            var result = await _getByIdModules.ExecuteAsync(ModulesId);
            if (result == null)
                return NotFound(new { message = "No se encontró el módulo." });
            return Ok(result);
        }

        [HttpPut]
        [Route("ModulesUpdate")]
        public async Task<IActionResult> Update([FromBody] ModulesUpdateDto dto)
        {
            var result = await _updateModules.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("ModulesStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] ModulesStatusToogleDto dto)
        {
            var result = await _patchModulesStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}


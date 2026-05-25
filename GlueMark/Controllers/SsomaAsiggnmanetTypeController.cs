using Application.DTOs.Ssoma;
using Application.DTOs.SsomaAssignmanetType;
using Application.UseCases.SsomaAssignmanetType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SsomaAsiggnmanetTypeController : ControllerBase
    {
        private readonly CreateSsomaAssignmanetType _createSsomaAssignmanetType;
        private readonly GetAllSsomaAssignmanetType _getAllSsomaAssignmanetType;
        private readonly GetByIdSsomaAssignmanetType _getByIdSsomaAssignmanetType;
        private readonly UpdateSsomaAssignmanetType _updateSsomaAssignmanetType;
        private readonly PatchSsomaAssignmanetType _patchSsomaAssignmanetType;
        private readonly GetSelectSsomaAssignmanetType _getSelectSsomaAssignmanetType;

        public SsomaAsiggnmanetTypeController(CreateSsomaAssignmanetType createSsomaAssignmanetType, GetAllSsomaAssignmanetType getAllSsomaAssignmanetType, GetByIdSsomaAssignmanetType getByIdSsomaAssignmanetType, UpdateSsomaAssignmanetType updateSsomaAssignmanetType, PatchSsomaAssignmanetType patchSsomaAssignmanetType, GetSelectSsomaAssignmanetType getSelectSsomaAssignmanetType)
        {
            _createSsomaAssignmanetType = createSsomaAssignmanetType;
            _getAllSsomaAssignmanetType = getAllSsomaAssignmanetType;
            _getByIdSsomaAssignmanetType = getByIdSsomaAssignmanetType;
            _updateSsomaAssignmanetType = updateSsomaAssignmanetType;
            _patchSsomaAssignmanetType = patchSsomaAssignmanetType;
            _getSelectSsomaAssignmanetType = getSelectSsomaAssignmanetType;
        }

        [HttpPost]
        [Route("SsomaAssignmanetTypeCreate")]
        public async Task<IActionResult> Create([FromBody] SsomaAssignmanetTypeCreateDto dto)
        {
            var result = await _createSsomaAssignmanetType.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("SsomaAssignmanetTypeList")]
        public async Task<IActionResult> GetList([FromQuery] int businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllSsomaAssignmanetType.ExecuteAsync(businessId, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de asignación." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);

        }
        [HttpGet]
        [Route("SsomaAssignmanetTypeById")]
        public async Task<IActionResult> GetById([FromQuery] long ssomaAssignmentTypeId)
        {
            var result = await _getByIdSsomaAssignmanetType.ExecuteAsync(ssomaAssignmentTypeId);
            if (result == null)
                return NotFound(new { message = "Estado de oportunidades no encontrado." });
            return Ok(result);
        }
        [HttpPut]
        [Route("SsomaAssignmanetTypeUpdate")]
        public async Task<IActionResult> Update([FromBody] SsomaAsiggnmanetTypeUpdateDto dto)
        {
            var result = await _updateSsomaAssignmanetType.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

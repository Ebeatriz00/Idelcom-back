using Application.DTOs.SsomaMovementType;
using Application.UseCases.SsomaMovementTypes;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SsomaMovementTypeController : ControllerBase
    {
        private readonly CreateSsomaMovementType _createSsomaMovementType;
        private readonly GetAllSsomaMovementType _getAllSsomaMovementType;
        private readonly GetByIdSsomaMovementType _getByIdSsomaMovementType;
        private readonly UpdateSsomaMovementType _updateSsomaMovementType;
        private readonly GetSelectSsomaMovementType _getSelectSsomaMovementType;
        private readonly PatchSsomaMovementType _patchSsomaMovementType;

        public SsomaMovementTypeController(
            CreateSsomaMovementType createSsomaMovementType,
            GetAllSsomaMovementType getAllSsomaMovementType,
            GetByIdSsomaMovementType getByIdSsomaMovementType,
            UpdateSsomaMovementType updateSsomaMovementType,
            GetSelectSsomaMovementType getSelectSsomaMovementType,
            PatchSsomaMovementType patchSsomaMovementType)
        {
            _createSsomaMovementType = createSsomaMovementType;
            _getAllSsomaMovementType = getAllSsomaMovementType;
            _getByIdSsomaMovementType = getByIdSsomaMovementType;
            _updateSsomaMovementType = updateSsomaMovementType;
            _getSelectSsomaMovementType = getSelectSsomaMovementType;
            _patchSsomaMovementType = patchSsomaMovementType;
        }

        [HttpPost]
        [Route("SsomaMovementTypeCreate")]
        public async Task<IActionResult> Create([FromBody] SsomaMovementTypeCreateDto dto)
        {
            var result = await _createSsomaMovementType.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("SsomaMovementTypeList")]
        public async Task<IActionResult> GetList(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] long? usersBy = null)
        {
            var result = await _getAllSsomaMovementType.ExecuteAsync(business_id, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de movimiento SSOMA." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("SsomaMovementTypeSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectSsomaMovementType.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("SsomaMovementTypeIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long ssomaMovementTypeId)
        {
            var result = await _getByIdSsomaMovementType.ExecuteAsync(ssomaMovementTypeId);
            if (result == null)
                return NotFound(new { message = "No se encontró el tipo de movimiento SSOMA." });

            return Ok(result);
        }

        [HttpPut]
        [Route("SsomaMovementTypeUpdate")]
        public async Task<IActionResult> Update([FromBody] SsomaMovementTypeUpdateDto dto)
        {
            var result = await _updateSsomaMovementType.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("SsomaMovementTypeStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] SsomaMovementTypeStatusToogleDto dto)
        {
            var result = await _patchSsomaMovementType.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
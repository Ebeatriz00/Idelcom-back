using Application.DTOs.Operations.Support;
using Application.UseCases.Operations.Support;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SupportController(
        CreateSupport createSupport,
        GetAllSupport getAllSupport,
        GetByIdSupport getByIdSupport,
        UpdateSupport updateSupport,
        DeleteSupport deleteSupport) : BaseController
    {
        private readonly CreateSupport _createSupport = createSupport;
        private readonly GetAllSupport _getAllSupport = getAllSupport;
        private readonly GetByIdSupport _getByIdSupport = getByIdSupport;
        private readonly UpdateSupport _updateSupport = updateSupport;
        private readonly DeleteSupport _deleteSupport = deleteSupport;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllSupport.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SupportCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createSupport.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] long supportId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getByIdSupport.ExecuteAsync(supportId, businessId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SupportUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updateSupport.ExecuteAsync(dto, userId, businessId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long supportId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _deleteSupport.ExecuteAsync(supportId, userId, businessId);

            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}

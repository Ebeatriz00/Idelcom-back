using Application.DTOs.SsomaProcess;
using Application.UseCases.SsomaProcess;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Ssoma
{
    [Route("api/[controller]")]
    [ApiController]
    public class SsomaProcessController(
        CreateSsomaProcess createSsomaProcess,
        GetAllSsomaProcess getAllSsomaProcess,
        GetByIdSsomaProcess getByIdSsomaProcess,
        UpdateSsomaProcess updateSsomaProcess,
        DeleteSsomaProcess deleteSsomaProcess
        ) : BaseController
    {
        private readonly CreateSsomaProcess _createSsomaProcess = createSsomaProcess;
        private readonly GetAllSsomaProcess _getAllSsomaProcess = getAllSsomaProcess;
        private readonly GetByIdSsomaProcess _getByIdSsomaProcess = getByIdSsomaProcess;
        private readonly UpdateSsomaProcess _updateSsomaProcess = updateSsomaProcess;
        private readonly DeleteSsomaProcess _deleteSsomaProcess = deleteSsomaProcess;

        [HttpGet]
        [Route("GetAllSsomaProcess")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] long? operationsId = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllSsomaProcess.ExecuteAsync(businessId, page, pageSize, search, operationsId);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetByIdSsomaProcess")]
        public async Task<IActionResult> GetById([FromQuery] long ssomaProcessId, [FromQuery] long operationsId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getByIdSsomaProcess.ExecuteAsync(ssomaProcessId, operationsId, businessId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        [HttpPost]
        [Route("CreateSsomaProcess")]
        public async Task<IActionResult> Create([FromBody] SsomaProcessCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createSsomaProcess.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }
        [HttpPut]
        [Route("UpdateSsomaProcess")]
        public async Task<IActionResult> Update([FromBody] SsomaProcessUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _updateSsomaProcess.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("DeleteSsomaProcess")]
        public async Task<IActionResult> Delete([FromQuery] long ssomaProcessId, [FromQuery]  long operationsId)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _deleteSsomaProcess.ExecuteAsync(ssomaProcessId, operationsId, businessId, userId);
            return Ok(result);
        }
    }
}

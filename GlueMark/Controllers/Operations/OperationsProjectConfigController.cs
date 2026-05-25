using Application.DTOs.Operations;
using Application.DTOs.OperationsProjectConfing;
using Application.UseCases.OperationsProjectConfig;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsProjectConfigController(
        CreateOperationsProjectConfig createOperationsProjectConfig,
        GetAllOperationsProjectConfig getAllOperationsProjectConfig,
        GetByIdOperationsProjectConfig getByIdOperationsProjectConfig,
        UpdateOperationsProjectConfig updateOperationsProjectConfig
        ) : BaseController
    {
        private readonly CreateOperationsProjectConfig _createOperationsProjectConfig = createOperationsProjectConfig;
        private readonly GetAllOperationsProjectConfig _getAllOperationsProjectConfig = getAllOperationsProjectConfig;
        private readonly GetByIdOperationsProjectConfig _getByIdOperationsProjectConfig = getByIdOperationsProjectConfig;
        private readonly UpdateOperationsProjectConfig _updateOperationsProjectConfig = updateOperationsProjectConfig;


        [HttpPost]
        [Route("CreateOperationsProjectConfig")]
        public async Task<IActionResult> Create([FromBody] OperationsProjectConfigCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _createOperationsProjectConfig.ExecuteAsync(dto, userId, businessId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllOperationsProjectConfig")]
        public async Task<IActionResult> GetAll([FromQuery] long operationsId)
        {
            var result = await _getAllOperationsProjectConfig.ExecuteAsync(operationsId);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetByIdOperationsProjectConfig")]
        public async Task<IActionResult> GetById([FromQuery] long operationsProjectConfigId, long operationsId)
        {
            var result = await _getByIdOperationsProjectConfig.ExecuteAsync(operationsProjectConfigId, operationsId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        [HttpPut]
        [Route("UpdateOperationsProjectConfig")]
        public async Task<IActionResult> Update([FromBody] OperationsProjectConfigUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _updateOperationsProjectConfig.ExecuteAsync(dto, userId, businessId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

    }
}

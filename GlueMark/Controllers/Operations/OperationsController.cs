using Application.DTOs.Operations.Operations;
using Application.UseCases.Operations.Operations;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OperationsController(
        CreateOperations createOperations,
        GetAllOperations getAllOperations,
        GetByIdOperations getByIdOperations,
        UpdateOperations updateOperations,
        DeleteOperations deleteOperations
        ) : BaseController
    {
        private readonly CreateOperations _createOperations = createOperations;
        private readonly GetAllOperations _getAllOperations = getAllOperations;
        private readonly GetByIdOperations _getByIdOperations = getByIdOperations;
        private readonly UpdateOperations _updateOperations = updateOperations;
        private readonly DeleteOperations _deleteOperations = deleteOperations;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();

            var result = await _getAllOperations.ExecuteAsync(businessId, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] long operationsId)
        {
            var result = await _getByIdOperations.ExecuteAsync(operationsId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OperationsCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _createOperations.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] OperationsUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _updateOperations.ExecuteAsync(dto, userId, businessId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long operationsId)
        {

            var userId = GetCurrentUserId();
            var result = await _deleteOperations.ExecuteAsync(operationsId, userId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}

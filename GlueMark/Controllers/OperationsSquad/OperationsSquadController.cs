using Application.DTOs.OperationsSquad;
using Application.UseCases.OperationsSquad;
using Application.UseCases.OperationsSquad.Application.UseCases.OperationsSquad;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.OperationsSquad
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OperationsSquadController(
        CreateOperationSquad createOperationSquad,
        GetAllOperationsSquad getAllOperationsSquad,
        GetByIdOperationsSquad getByIdOperationsSquad,
        UpdateOperationsSquad updateOperationsSquad,
        DeleteOperationsSquad deleteOperationsSquad) : BaseController 
    {
        private readonly CreateOperationSquad _createOperationSquad = createOperationSquad;
        private readonly GetAllOperationsSquad _getAllOperationsSquad = getAllOperationsSquad;
        private readonly GetByIdOperationsSquad _getByIdOperationsSquad = getByIdOperationsSquad;
        private readonly UpdateOperationsSquad _updateOperationsSquad = updateOperationsSquad;
        private readonly DeleteOperationsSquad _deleteOperationsSquad = deleteOperationsSquad; 

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] long businessId,
            [FromQuery] string? search, 
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] long? workOrderId = null)
        {
            var result = await _getAllOperationsSquad.ExecuteAsync(businessId, search, page, pageSize, workOrderId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OperationsSquadCreateDto dto)
        {
            var result = await _createOperationSquad.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] long squadId)
        {
            var result = await _getByIdOperationsSquad.ExecuteAsync(squadId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] OperationsSquadUpdateDto dto)
        {
            var result = await _updateOperationsSquad.ExecuteAsync(dto);
            if (result == null)
                return NotFound();

            return Ok(result);
        }  


        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long squadId)
        {
            var userId = GetCurrentUserId();
            var result = await _deleteOperationsSquad.ExecuteAsync(squadId, userId);

            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}

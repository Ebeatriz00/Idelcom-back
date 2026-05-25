using Application.DTOs.OperationsPersonnelMovement;
using Application.UseCases.OperationsPersonnelMovement;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Idelcom.Controllers.OperationPersonnelMovement
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OperationPersonnelMovementController(
        CreateOperationPersonnelMovement createUseCase,
        GetAllOperationPersonnelMovement getAllUseCase
    ) : BaseController
    {
        private readonly CreateOperationPersonnelMovement _createUseCase = createUseCase;
        private readonly GetAllOperationPersonnelMovement _getAllUseCase = getAllUseCase;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OperationPersonnelMovementCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createUseCase.ExecuteAsync(dto, businessId, userId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllUseCase.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }
    }
}

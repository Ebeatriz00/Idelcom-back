using Application.DTOs.Operations.OperationsWorkOrderProgress;
using Application.UseCases.Operations.OperationsWorkOrderProgress;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.App.WorkOrderProgress
{
    [Route("api/app/work-order-progress")]
    [ApiController]
    public class AppWorkOrderProgressController(
        CreateAppOperationsWorkOrderProgress createUseCase,
        SyncAppOperationsWorkOrderProgress syncUseCase) : AppBaseController
    {
        private readonly CreateAppOperationsWorkOrderProgress _createUseCase = createUseCase;
        private readonly SyncAppOperationsWorkOrderProgress _syncUseCase = syncUseCase;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OperationsWorkOrderProgressCreateDto dto)
        {
            var userId = GetCurrentAppUserId();
            var businessId = GetCurrentAppBusinessId();

            var result = await _createUseCase.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync([FromForm] OperationsWorkOrderProgressSyncDto dto)
        {
            var userId = GetCurrentAppUserId();
            var businessId = GetCurrentAppBusinessId();

            var result = await _syncUseCase.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }
    }
}

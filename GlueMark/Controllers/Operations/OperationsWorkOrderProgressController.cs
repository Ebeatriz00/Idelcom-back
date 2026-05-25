using Application.DTOs.Operations.OperationsWorkOrderProgress;
using Application.UseCases.Operations.OperationsWorkOrderProgress;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsWorkOrderProgressController(
        GetOperationsWorkOrderProgressList getListUseCase) : BaseController
    {
        private readonly GetOperationsWorkOrderProgressList _getListUseCase = getListUseCase;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] long? activityId = null,
            [FromQuery] string? date = null,
            [FromQuery] long? operationsId = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getListUseCase.ExecuteAsync(businessId, search, page, pageSize, activityId, date, operationsId);
            return Ok(result);
        }

        [HttpGet("{progressId}/photos")]
        public async Task<IActionResult> GetPhotos(long progressId, [FromServices] GetAppOperationsWorkOrderProgressPhotos useCase)
        {
            var result = await useCase.ExecuteAsync(progressId);
            return Ok(new SharedKernel.GlobalResponse<IEnumerable<OperationsWorkOrderProgressPhotoDto>>
            {
                Status = 1,
                Message = "Fotos obtenidas exitosamente.",
                Data = result
            });
        }
    }
}

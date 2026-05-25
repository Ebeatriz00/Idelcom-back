using Application.UseCases.Operations.OperationsWorkOrderStatus;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OperationsWorkOrderStatusController(
        GetSelectOperationsWorkOrderStatus getAllUseCase,
        GetByIdOperationsWorkOrderStatus getByIdUseCase) : BaseController
    {
        private readonly GetSelectOperationsWorkOrderStatus _getAllUseCase = getAllUseCase;
        private readonly GetByIdOperationsWorkOrderStatus _getByIdUseCase = getByIdUseCase;

        [HttpGet]
        public async Task<IActionResult> GetSelect(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var businessId = GetCurrentBusinessId();

            var result = await _getAllUseCase.ExecuteAsync(businessId, page, pageSize, search);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] long workOrderStatusId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getByIdUseCase.ExecuteAsync(workOrderStatusId, businessId);
            return Ok(result);
        }
    }


}

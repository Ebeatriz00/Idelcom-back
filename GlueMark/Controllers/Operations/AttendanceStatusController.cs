using Application.UseCases.Operations.AttendanceStatus;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Operations
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttendanceStatusController(
        GetSelectAttendanceStatus getAllUseCase,
        GetByIdAttendanceStatus getByIdUseCase) : BaseController
    {
        private readonly GetSelectAttendanceStatus _getAllUseCase = getAllUseCase;
        private readonly GetByIdAttendanceStatus _getByIdUseCase = getByIdUseCase;

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
        public async Task<IActionResult> GetById([FromQuery] long attendanceStatusId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getByIdUseCase.ExecuteAsync(attendanceStatusId, businessId);
            return Ok(result);
        }
    }


}

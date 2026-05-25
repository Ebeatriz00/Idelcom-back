using Application.DTOs.Operations.OperationsAttendance;
using Application.UseCases.Operations.OperationsAttendance;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Idelcom.Controllers.Operations
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OperationsAttendanceController(
        GetAllAttendanceMatrixUseCase getAllMatrixUseCase
        ) : BaseController
    {
        private readonly GetAllAttendanceMatrixUseCase _getAllMatrixUseCase = getAllMatrixUseCase;

        [HttpGet]
        public async Task<IActionResult> GetAllMatrix([FromQuery] AttendanceMatrixQueryDto query)
        {
            var businessId = GetCurrentBusinessId();
            var response = await _getAllMatrixUseCase.ExecuteAsync(businessId, query);

            return Ok(new GlobalResponse<AttendanceMatrixResponseDto>
            {
                Status = 1,
                Message = "Matriz de asistencia obtenida correctamente.",
                Data = response
            });
        }
    }
}

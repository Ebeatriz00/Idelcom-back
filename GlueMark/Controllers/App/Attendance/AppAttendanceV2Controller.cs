using Application.DTOs.Operations.OperationsAttendance;
using Application.UseCases.Operations.OperationsAttendance;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Idelcom.Controllers.App.Attendance
{
    [Route("api/app/attendance/v2")]
    public class AppAttendanceV2Controller(
        GetAppAttendanceDailyV2UseCase getDailyUseCase,
        CreateAppAttendanceBatchV2UseCase createBatchUseCase,
        SyncAppAttendanceBatchV2UseCase syncBatchUseCase) : AppBaseController
    {
        private readonly GetAppAttendanceDailyV2UseCase _getDailyUseCase = getDailyUseCase;
        private readonly CreateAppAttendanceBatchV2UseCase _createBatchUseCase = createBatchUseCase;
        private readonly SyncAppAttendanceBatchV2UseCase _syncBatchUseCase = syncBatchUseCase;

        [HttpGet("daily")]
        public async Task<IActionResult> GetDaily([FromQuery] DateTime attendanceDate)
        {
            var businessId = GetCurrentAppBusinessId();
            var userId = GetCurrentAppUserId();

            var data = await _getDailyUseCase.ExecuteAsync(businessId, userId, attendanceDate);

            return Ok(new GlobalResponse<AppAttendanceDailyV2ResponseDto>
            {
                Status = 1,
                Message = "Consulta diaria de asistencia obtenida correctamente.",
                Data = data
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AppAttendanceV2CreateDto dto)
        {
            var businessId = GetCurrentAppBusinessId();
            var userId = GetCurrentAppUserId();

            var processedWorkers = await _createBatchUseCase.ExecuteAsync(dto, businessId, userId);

            return Ok(new GlobalResponse<object>
            {
                Status = 1,
                Message = "Asistencia registrada correctamente.",
                Data = new
                {
                    attendanceDate = dto.AttendanceDate.ToString("yyyy-MM-dd"),
                    operationsId = dto.OperationsId,
                    sessionType = dto.SessionType,
                    processedWorkers
                }
            });
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync([FromForm] AppAttendanceV2SyncDto dto)
        {
            var businessId = GetCurrentAppBusinessId();
            var userId = GetCurrentAppUserId();

            var processedWorkers = await _syncBatchUseCase.ExecuteAsync(dto, businessId, userId);

            return Ok(new GlobalResponse<object>
            {
                Status = 1,
                Message = "Asistencia y fotos registradas correctamente.",
                Data = new
                {
                    attendanceDate = dto.AttendanceDate.ToString("yyyy-MM-dd"),
                    operationsId = dto.OperationsId,
                    sessionType = dto.SessionType,
                    processedWorkers
                }
            });
        }
    }
}

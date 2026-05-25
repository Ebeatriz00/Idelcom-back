using Application.DTOs.Operations.OperationsAttendance;
using Application.UseCases.Operations.OperationsAttendance;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Idelcom.Controllers.App.Attendance
{
    [Route("api/app/attendance")]
    public class AppAttendanceController(
        GetAppAttendanceDailyUseCase getDailyUseCase,
        CreateAppAttendanceBatchUseCase createBatchUseCase,
        SyncAppAttendanceBatchUseCase syncBatchUseCase) : AppBaseController
    {
        private readonly GetAppAttendanceDailyUseCase _getDailyUseCase = getDailyUseCase;
        private readonly CreateAppAttendanceBatchUseCase _createBatchUseCase = createBatchUseCase;
        private readonly SyncAppAttendanceBatchUseCase _syncBatchUseCase = syncBatchUseCase;

        [HttpGet("daily")]
        public async Task<IActionResult> GetDaily([FromQuery] DateTime attendanceDate)
        {
            var businessId = GetCurrentAppBusinessId();
            var userId = GetCurrentAppUserId();

            var data = await _getDailyUseCase.ExecuteAsync(businessId, userId, attendanceDate);

            return Ok(new GlobalResponse<AppAttendanceDailyResponseDto>
            {
                Status = 1,
                Message = "Consulta diaria de asistencia obtenida correctamente.",
                Data = data
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AppAttendanceCreateDto dto)
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
                    workOrderId = dto.WorkOrderId,
                    squadId = dto.SquadId,
                    sessionType = dto.SessionType,
                    processedWorkers
                }

            });
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync([FromForm] AppAttendanceSyncDto dto)
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
                    workOrderId = dto.WorkOrderId,
                    squadId = dto.SquadId,
                    sessionType = dto.SessionType,
                    processedWorkers
                }
            });
        }
    }
}

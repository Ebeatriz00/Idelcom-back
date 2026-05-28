using Application.DTOs.Operations.OperationsAttendance;
using AutoMapper;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.OperationsAttendance
{
    public class GetAppAttendanceDailyUseCase(
        IOperationsAttendanceRepository repository,
        IMapper mapper)
    {
        private readonly IOperationsAttendanceRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<AppAttendanceDailyResponseDto> ExecuteAsync(long businessId, long userId, DateTime attendanceDate)
        {
            // Consultar datos al repositorio
            var result = await _repository.GetDailyAsync(businessId, userId, attendanceDate);

            // Preparar respuesta raíz
            var response = new AppAttendanceDailyResponseDto
            {
                AttendanceDate = attendanceDate.ToString("yyyy-MM-dd"),
                AttendanceStatuses = _mapper.Map<List<AppAttendanceStatusDto>>(result.Statuses)
            };

            // Construir jerarquía: Operations -> ProjectConfigs -> WorkOrders -> Squads -> Workers/Sessions

            foreach (var opProj in result.Operations)
            {
                var opDto = _mapper.Map<AppAttendanceOperationDto>(opProj);

                // Mapear configuraciones (turnos) de esta operación
                var configProjs = result.Configs.Where(c => c.OperationsId == opProj.OperationsId);
                opDto.ProjectConfigs = _mapper.Map<List<AppAttendanceProjectConfigDto>>(configProjs);

                // Filtrar OTs de esta operación
                var workOrderProjs = result.WorkOrders.Where(wo => wo.OperationsId == opProj.OperationsId);
                foreach (var woProj in workOrderProjs)
                {
                    var woDto = _mapper.Map<AppAttendanceWorkOrderDto>(woProj);

                    // Filtrar cuadrillas de esta OT
                    var squadProjs = result.Squads.Where(s => s.WorkOrderId == woProj.WorkOrderId);
                    foreach (var squadProj in squadProjs)
                    {
                        var workerProjs = result.Workers.Where(w => w.SquadId == squadProj.SquadId).ToList();

                        // REGLA: No listar cuadrillas que no tengan trabajadores asignados
                        if (workerProjs.Count == 0) continue;

                        var squadDto = _mapper.Map<AppAttendanceSquadDto>(squadProj);

                        // Mapear sesiones de esta cuadrilla
                        var squadSessions = result.Sessions.Where(s => s.SquadId == squadProj.SquadId).ToList();
                        var entrada = squadSessions.FirstOrDefault(s => s.SessionType == "ENTRADA");
                        if (entrada != null)
                            squadDto.Sessions.Entrada = _mapper.Map<AppAttendanceSessionInfoDto>(entrada);

                        var salida = squadSessions.FirstOrDefault(s => s.SessionType == "SALIDA");
                        if (salida != null)
                            squadDto.Sessions.Salida = _mapper.Map<AppAttendanceSessionInfoDto>(salida);

                        // Mapear trabajadores y su asistencia
                        foreach (var workerProj in workerProjs)
                        {
                            var workerDto = _mapper.Map<AppAttendanceWorkerDto>(workerProj);

                            // Buscar asistencia por AssignmentId
                            var detailProj = result.Details.FirstOrDefault(d => d.AssignmentId == workerProj.AssignmentId);
                            if (detailProj != null)
                            {
                                workerDto.Attendance = _mapper.Map<AppAttendanceDetailDto>(detailProj);
                            }

                            squadDto.Workers.Add(workerDto);
                        }

                        woDto.Squads.Add(squadDto);
                    }

                    // Solo añadir OT si tiene cuadrillas con personal
                    if (woDto.Squads.Count > 0)
                    {
                        opDto.WorkOrders.Add(woDto);
                    }
                }

                // Solo añadir operación si tiene OTs válidas
                if (opDto.WorkOrders.Count > 0)
                {
                    response.Operations.Add(opDto);
                }
            }

            return response;
        }
    }
}

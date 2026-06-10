using Application.DTOs.Operations.OperationsAttendance;
using AutoMapper;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.OperationsAttendance
{
    public class GetAppAttendanceDailyV2UseCase(
        IOperationsAttendanceV2Repository repository,
        IMapper mapper)
    {
        private readonly IOperationsAttendanceV2Repository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<AppAttendanceDailyV2ResponseDto> ExecuteAsync(long businessId, long userId, DateTime attendanceDate)
        {
            var result = await _repository.GetDailyAsync(businessId, userId, attendanceDate);

            var response = new AppAttendanceDailyV2ResponseDto
            {
                AttendanceDate = attendanceDate.ToString("yyyy-MM-dd"),
                AttendanceStatuses = _mapper.Map<List<AppAttendanceStatusDto>>(result.Statuses)
            };

            foreach (var opProj in result.Operations)
            {
                var opDto = _mapper.Map<AppAttendanceOperationV2Dto>(opProj);

                var configs = result.Configs.Where(c => c.OperationsId == opProj.OperationsId);
                opDto.ProjectConfigs = _mapper.Map<List<AppAttendanceProjectConfigDto>>(configs);

                var opSessions = result.Sessions.Where(s => s.OperationsId == opProj.OperationsId).ToList();
                var entrada = opSessions.FirstOrDefault(s => s.SessionType == "ENTRADA");
                if (entrada != null)
                    opDto.Sessions.Entrada = _mapper.Map<AppAttendanceSessionInfoDto>(entrada);

                var salida = opSessions.FirstOrDefault(s => s.SessionType == "SALIDA");
                if (salida != null)
                    opDto.Sessions.Salida = _mapper.Map<AppAttendanceSessionInfoDto>(salida);

                var workers = result.Workers.Where(w => w.OperationsId == opProj.OperationsId).ToList();
                foreach (var workerProj in workers)
                {
                    var workerDto = _mapper.Map<AppAttendanceWorkerV2Dto>(workerProj);

                    var detail = workerProj.AssignmentId.HasValue
                        ? result.Details.FirstOrDefault(d => d.AssignmentId == workerProj.AssignmentId.Value)
                        : result.Details.FirstOrDefault(d => d.WorkerId == workerProj.WorkerId);

                    if (detail != null)
                        workerDto.Attendance = _mapper.Map<AppAttendanceDetailDto>(detail);

                    opDto.Workers.Add(workerDto);
                }

                if (opDto.Workers.Count > 0)
                    response.Operations.Add(opDto);
            }

            return response;
        }
    }
}

using Application.DTOs.Operations.AttendanceStatus;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.AttendanceStatus
{
    public class GetByIdAttendanceStatus(IAttendanceStatusRepository repository, IMapper mapper)
    {
        private readonly IAttendanceStatusRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<AttendanceStatusGetByIdDto?> ExecuteAsync(
            long attendanceStatusId,
            long businessId)
        {
            var result = await _repository.GetByIdAsync(attendanceStatusId, businessId);

            if (result == null)
                throw new NotFoundException("AttendanceStatus Not Found", attendanceStatusId);

            return _mapper.Map<AttendanceStatusGetByIdDto>(result);
        }
    }

}

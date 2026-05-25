using Application.DTOs.Operations.AttendanceStatus;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.AttendanceStatus
{
    public class GetSelectAttendanceStatus(IAttendanceStatusRepository repository, IMapper mapper)
    {
        private readonly IAttendanceStatusRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedSelect<AttendanceStatusGetSelectDto>> ExecuteAsync(
            long businessId,
            int page,
            int pageSize,
            string? search)
        {
            var result = await _repository.GetAllAsync(businessId, page, pageSize, search);
            return _mapper.Map<PagedSelect<AttendanceStatusGetSelectDto>>(result);
        }
    }

}

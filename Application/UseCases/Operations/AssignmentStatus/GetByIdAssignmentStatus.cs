using Application.DTOs.Operations.AssignmentStatus;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.AssignmentStatus
{
    public class GetByIdAssignmentStatus(IAssignmentStatusRepository repository, IMapper mapper)
    {
        private readonly IAssignmentStatusRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<AssignmentStatusGetByIdDto?> ExecuteAsync(long assignmentStatusId, long businessId)
        {
            var result = await _repository.GetByIdAsync(assignmentStatusId, businessId);

            if (result == null)
                throw new NotFoundException("AssignmentStatus not found", assignmentStatusId);

            return _mapper.Map<AssignmentStatusGetByIdDto>(result);
        }
    }

}

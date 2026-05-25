using Application.DTOs.OperationsPersonnelAssignment;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.OperationsPersonnelAssignment;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsPersonnelAssignment
{
    public class GetAllOperationsPersonnelAssignment(IOperationsPersonnelAssignmentRepository repository, IMapper mapper)
    {
        private readonly IOperationsPersonnelAssignmentRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<OperationsPersonnelAssignmentResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var pagedEntity = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<OperationsPersonnelAssignmentResponseDto>>(pagedEntity);
        }
    }
}

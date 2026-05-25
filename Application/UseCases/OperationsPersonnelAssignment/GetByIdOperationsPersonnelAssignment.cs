using Application.DTOs.OperationsPersonnelAssignment;
using AutoMapper;
using Core.Interfaces.OperationsPersonnelAssignment;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsPersonnelAssignment
{
    public class GetByIdOperationsPersonnelAssignment(IOperationsPersonnelAssignmentRepository repository, IMapper mapper)
    {
        private readonly IOperationsPersonnelAssignmentRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<OperationsPersonnelAssignmentResponseDto?> ExecuteAsync(long assignmentId)
        {
            var entity = await _repository.GetByIdAsync(assignmentId);
            return _mapper.Map<OperationsPersonnelAssignmentResponseDto?>(entity);
        }
    }
}

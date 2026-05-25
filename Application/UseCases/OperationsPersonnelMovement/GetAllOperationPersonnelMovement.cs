using Application.DTOs.OperationsPersonnelMovement;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.OperationsPersonnelMovement;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsPersonnelMovement
{
    public class GetAllOperationPersonnelMovement(IOperationsPersonnelMovementRepository repository, IMapper mapper)
    {
        private readonly IOperationsPersonnelMovementRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<OperationPersonnelMovementResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var pagedEntity = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<OperationPersonnelMovementResponseDto>>(pagedEntity);
        }
    }
}

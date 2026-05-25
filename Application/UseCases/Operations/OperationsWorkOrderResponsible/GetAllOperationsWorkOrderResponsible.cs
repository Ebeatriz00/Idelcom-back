using Application.DTOs.Operations.OperationsWorkOrderResponsible;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.OperationsWorkOrderResponsible
{
    public class GetAllOperationsWorkOrderResponsible(IOperationsWorkOrderResponsibleRepository repository, IMapper mapper)
    {
        private readonly IOperationsWorkOrderResponsibleRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<OperationsWorkOrderResponsibleResponseDto>> ExecuteAsync(long businessId, int page, int pageSize, string? search)
        {
            var result = await _repository.GetAllAsync(businessId, page, pageSize, search);
            return _mapper.Map<PagedResult<OperationsWorkOrderResponsibleResponseDto>>(result);
        }
    }

}

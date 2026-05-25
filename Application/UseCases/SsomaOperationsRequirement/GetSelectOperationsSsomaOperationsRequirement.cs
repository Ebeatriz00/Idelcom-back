using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Ssoma;

namespace Application.UseCases.SsomaOperationsRequirement
{
    public class GetSelectOperationsSsomaOperationsRequirement(ISsomaOperationsRequirementRepository repository, IMapper mapper)
    {
        private readonly ISsomaOperationsRequirementRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var result = await _repository.GetForSelectOperationsAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(result);
        }
    }
}

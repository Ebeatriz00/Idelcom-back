using Application.DTOs.Operations.ActivityComplexity;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.ActivityComplexity
{
    public class GetSelectActivityComplexity(IActivityComplexityRepository repository, IMapper mapper)
    {
        private readonly IActivityComplexityRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedSelect<ActivityComplexitySelectDto>> ExecuteAsync(
            long businessId,
            int page,
            int pageSize,
            string? search)
        {
            var result = await _repository.GetForSelectAsync(businessId, page, pageSize, search);
            return _mapper.Map<PagedSelect<ActivityComplexitySelectDto>>(result);
        }
    }
}

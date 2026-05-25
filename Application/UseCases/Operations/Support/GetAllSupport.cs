using Application.DTOs.Operations.Support;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.Support
{
    public class GetAllSupport(ISupportRepository repository, IMapper mapper)
    {
        private readonly ISupportRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<SupportResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var pagedEntity = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<SupportResponseDto>>(pagedEntity);
        }
    }
}

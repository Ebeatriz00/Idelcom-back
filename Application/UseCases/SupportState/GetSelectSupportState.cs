using Application.DTOs.SupportState;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using Core.Projections;

namespace Application.UseCases.SupportState
{
    public class GetSelectSupportState(ISupportStateRepository repository, IMapper mapper)
    {
        private readonly ISupportStateRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedSelect<SupportStateSelectDto?>> ExecuteAsync(
            long businessId, int page, int pageSize, string? search)
        {
            var result = await _repository.GetForSelectAsync(businessId, page, pageSize, search ?? string.Empty);
            return _mapper.Map<PagedSelect<SupportStateSelectDto?>>(result);
        }
    }
}

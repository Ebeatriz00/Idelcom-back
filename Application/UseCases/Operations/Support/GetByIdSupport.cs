using Application.DTOs.Operations.Support;
using AutoMapper;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.Support
{
    public class GetByIdSupport(ISupportRepository repository, IMapper mapper)
    {
        private readonly ISupportRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<SupportResponseDto?> ExecuteAsync(long supportId, long businessId)
        {
            var entity = await _repository.GetByIdAsync(supportId, businessId);
            return entity == null ? null : _mapper.Map<SupportResponseDto>(entity);
        }
    }
}

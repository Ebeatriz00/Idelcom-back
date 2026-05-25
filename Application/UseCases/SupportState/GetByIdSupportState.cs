using Application.DTOs.SupportState;
using AutoMapper;
using Core.Interfaces;

namespace Application.UseCases.SupportState
{
    public class GetByIdSupportState(ISupportStateRepository repository, IMapper mapper)
    {
        private readonly ISupportStateRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<SupportStateGetByIdDto?> ExecuteAsync(int supportStateId, long businessId)
        {
            var result = await _repository.GetByIdAsync(supportStateId, businessId);
            if (result == null) return null;
            return _mapper.Map<SupportStateGetByIdDto>(result);
        }
    }
}

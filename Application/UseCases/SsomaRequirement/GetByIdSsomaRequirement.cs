using Application.DTOs.SsomaRequirement;
using AutoMapper;
using Core.Interfaces.Ssoma;

namespace Application.UseCases.SsomaRequirement
{
    public class GetByIdSsomaRequirement(
        ISsomaRequirementRepository repository,
        IMapper mapper)
    {
        private readonly ISsomaRequirementRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<SsomaRequirementByIdDto?> ExecuteAsync(long requirementId, long businessId)
        {
            var entity = await _repository.GetByIdAsync(requirementId, businessId);
            return _mapper.Map<SsomaRequirementByIdDto?>(entity);
        }
    }
}

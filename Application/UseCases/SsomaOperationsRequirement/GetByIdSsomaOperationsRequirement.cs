using Application.DTOs.SsomaOperationsRequirement;
using AutoMapper;
using Core.Interfaces.Ssoma;

namespace Application.UseCases.SsomaOperationsRequirement
{
    public class GetByIdSsomaOperationsRequirement(
        ISsomaOperationsRequirementRepository repository,
        IMapper mapper)
    {
        private readonly ISsomaOperationsRequirementRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<SsomaOperationsRequirementByIdDto?> ExecuteAsync(long ssomaOperationsRequirementId, long businessId)
        {
            var entity = await _repository.GetByIdAsync(ssomaOperationsRequirementId, businessId);
            return _mapper.Map<SsomaOperationsRequirementByIdDto?>(entity);
        }
    }
}

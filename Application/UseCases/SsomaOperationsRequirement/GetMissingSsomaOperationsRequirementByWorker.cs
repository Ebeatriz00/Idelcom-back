using Application.DTOs.SsomaOperationsRequirement;
using AutoMapper;
using Core.Interfaces.Ssoma;

namespace Application.UseCases.SsomaOperationsRequirement
{
    public class GetMissingSsomaOperationsRequirementByWorker(
        ISsomaOperationsRequirementRepository repository,
        IMapper mapper)
    {
        private readonly ISsomaOperationsRequirementRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<SsomaOperationsRequirementMissingByWorkerResponseDto>> ExecuteAsync(
            long businessId,
            long operationsId,
            long workerId)
        {
            var entities = await _repository.GetMissingByWorkerAsync(businessId, operationsId, workerId);
            return _mapper.Map<IEnumerable<SsomaOperationsRequirementMissingByWorkerResponseDto>>(entities);
        }
    }
}

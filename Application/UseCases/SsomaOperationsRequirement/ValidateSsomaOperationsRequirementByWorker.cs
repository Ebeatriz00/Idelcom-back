using Application.DTOs.SsomaOperationsRequirement;
using AutoMapper;
using Core.Interfaces.Ssoma;

namespace Application.UseCases.SsomaOperationsRequirement
{
    public class ValidateSsomaOperationsRequirementByWorker(
        ISsomaOperationsRequirementRepository repository,
        IMapper mapper)
    {
        private readonly ISsomaOperationsRequirementRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<ValidateSsomaOperationsRequirementByWorkerResponseDto?> ExecuteAsync(
            long businessId,
            long operationsId,
            long workerId)
        {
            var entity = await _repository.ValidateByWorkerAsync(businessId, operationsId, workerId);
            return _mapper.Map<ValidateSsomaOperationsRequirementByWorkerResponseDto?>(entity);
        }
    }
}

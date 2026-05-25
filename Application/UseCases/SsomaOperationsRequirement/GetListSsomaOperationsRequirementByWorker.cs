using Application.DTOs.SsomaOperationsRequirement;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Ssoma;

namespace Application.UseCases.SsomaOperationsRequirement
{
    public class GetListSsomaOperationsRequirementByWorker(
        ISsomaOperationsRequirementRepository repository,
        IMapper mapper)
    {
        private readonly ISsomaOperationsRequirementRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<SsomaOperationsRequirementByWorkerResponseDto>> ExecuteAsync(
            long businessId,
            long operationsId,
            long workerId,
            int page,
            int pageSize,
            string? search)
        {
            var entities = await _repository.GetListByWorkerAsync(businessId, operationsId, workerId, page, pageSize, search);
            return _mapper.Map<PagedResult<SsomaOperationsRequirementByWorkerResponseDto>>(entities);
        }
    }
}

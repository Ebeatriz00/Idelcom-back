using Application.DTOs.SsomaRequirement;
using AutoMapper;
using Core.Interfaces.Ssoma;
using Infrastructure.Exceptions;

namespace Application.UseCases.SsomaRequirement
{
    public class GetGeneralRequirementById(
        ISsomaRequirementRepository repository,
        IMapper mapper)
    {
        private readonly ISsomaRequirementRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<SsomaRequirementByIdDto?> ExecuteAsync(long requirementId, long businessId)
        {
            var entity = await _repository.GetByIdAsync(requirementId, businessId);
            
            if (entity == null)
                return null;

            if (entity.ScopeId != 1)
                throw new BusinessException("El requerimiento no corresponde al alcance general.");

            if (!entity.IsActive)
                throw new BusinessException("El requerimiento solicitado se encuentra inactivo.");

            return _mapper.Map<SsomaRequirementByIdDto>(entity);
        }
    }
}

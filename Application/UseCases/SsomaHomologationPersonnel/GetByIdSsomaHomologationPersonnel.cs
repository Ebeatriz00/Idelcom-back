using Application.DTOs.SsomaHomologationPersonnel;
using AutoMapper;
using Core.Interfaces.Ssoma;

namespace Application.UseCases.SsomaHomologationPersonnel
{
    public class GetByIdSsomaHomologationPersonnel(
        ISsomaHomologationPersonnelRepository repository,
        IMapper mapper)
    {
        private readonly ISsomaHomologationPersonnelRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<SsomaHomologationPersonnelByIdDto?> ExecuteAsync(long homologationPersonnelId, long businessId)
        {
            var entity = await _repository.GetByIdAsync(homologationPersonnelId, businessId);
            return _mapper.Map<SsomaHomologationPersonnelByIdDto?>(entity);
        }
    }
}

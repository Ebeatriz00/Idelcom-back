using Application.DTOs.Ssoma.Clinics;
using AutoMapper;
using Core.Interfaces.Ssoma.Clinics;

namespace Application.UseCases.Ssoma.Clinics
{
    public class GetByIdClinic(IClinicRepository repository, IMapper mapper)
    {
        private readonly IClinicRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<ClinicResponseDto?> ExecuteAsync(long clinicId, long businessId)
        {
            var result = await _repository.GetByIdAsync(clinicId, businessId);

            if (result == null)
                return null;

            return _mapper.Map<ClinicResponseDto>(result);
        }
    }
}

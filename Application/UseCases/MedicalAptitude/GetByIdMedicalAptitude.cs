using Application.DTOs.MedicalAptitude;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.MedicalAptitude
{
    public class GetByIdMedicalAptitude
    {
        private readonly IMedicalAptitudeRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdMedicalAptitude(IMedicalAptitudeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MedicalAptitudeResponseDto> ExecuteAsync(long medicalAptitudeId)
        {
            var entity = await _repository.GetByIdAsync(medicalAptitudeId);
            return _mapper.Map<MedicalAptitudeResponseDto>(entity);
        }
    }
}

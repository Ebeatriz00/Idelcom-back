using Application.DTOs.MedicalAptitude;
using AutoMapper;
using Core.Interfaces;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.MedicalAptitude
{
    public class CreateMedicalAptitude
    {
        private readonly IMedicalAptitudeRepository _repository;
        private readonly IMapper _mapper;

        public CreateMedicalAptitude(IMedicalAptitudeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(MedicalAptitudeCreateDto dto)
        {
            var entity = _mapper.Map<Core.Entities.MedicalAptitude>(dto);

            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Aptitud médica creada exitosamente."
            };
        }
    }
}

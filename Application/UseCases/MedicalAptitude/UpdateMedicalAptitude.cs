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
    public class UpdateMedicalAptitude
    {
        private readonly IMedicalAptitudeRepository _repository;
        private readonly IMapper _mapper;

        public UpdateMedicalAptitude(IMedicalAptitudeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(MedicalAptitudeUpdateDto dto)
        {
            var entity = _mapper.Map<Core.Entities.MedicalAptitude>(dto);

            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Aptitud médica actualizada correctamente."
                    : "No se pudo actualizar la aptitud médica."
            };
        }
    }
}

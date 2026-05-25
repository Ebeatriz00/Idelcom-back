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
    public class PatchMedicalAptitude
    {
        private readonly IMedicalAptitudeRepository _repository;
        private readonly IMapper _mapper;

        public PatchMedicalAptitude(IMedicalAptitudeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(MedicalAptitudeStatusToogleDto dto)
        {
            var updated = await _repository.PatchStatusAsync(
                dto.MedicalAptitudeId,
                dto.Status,
                dto.UsersBy,
                dto.BusinessId
            );

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado de la aptitud médica actualizado correctamente."
                    : "No se pudo actualizar el estado de la aptitud médica."
            };
        }
    }
}

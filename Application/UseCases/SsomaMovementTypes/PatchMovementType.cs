using Application.DTOs.SsomaMovementType;
using AutoMapper;
using Core.Interfaces;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaMovementTypes
{
    public class PatchSsomaMovementType
    {
        private readonly ISsomaMovementTypeRepository _repository;
        private readonly IMapper _mapper;

        public PatchSsomaMovementType(ISsomaMovementTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(SsomaMovementTypeStatusToogleDto dto)
        {
            var updated = await _repository.PatchStatusAsync(
                dto.SsomaMovementTypeId,
                dto.Status,
                dto.UsersBy,
                dto.BusinessId
            );

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del tipo de movimiento SSOMA actualizado correctamente."
                    : "No se pudo actualizar el estado del tipo de movimiento SSOMA."
            };
        }
    }
}

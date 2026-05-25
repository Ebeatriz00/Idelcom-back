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
    public class UpdateSsomaMovementType
    {
        private readonly ISsomaMovementTypeRepository _repository;
        private readonly IMapper _mapper;

        public UpdateSsomaMovementType(ISsomaMovementTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(SsomaMovementTypeUpdateDto dto)
        {
            var entity = _mapper.Map<Core.Entities.SsomaMovementType>(dto);

            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Tipo de movimiento SSOMA actualizado correctamente."
                    : "No se pudo actualizar el tipo de movimiento SSOMA."
            };
        }
    }
}

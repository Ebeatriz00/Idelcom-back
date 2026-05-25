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
    public class CreateSsomaMovementType
    {
        private readonly ISsomaMovementTypeRepository _repository;
        private readonly IMapper _mapper;

        public CreateSsomaMovementType(ISsomaMovementTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(SsomaMovementTypeCreateDto dto)
        {
            var entity = _mapper.Map<Core.Entities.SsomaMovementType>(dto);

            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Tipo de movimiento SSOMA creado exitosamente."
            };
        }
    }
}

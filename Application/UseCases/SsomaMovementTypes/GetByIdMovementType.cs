using Application.DTOs.SsomaMovementType;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaMovementTypes
{
    public class GetByIdSsomaMovementType
    {
        private readonly ISsomaMovementTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdSsomaMovementType(ISsomaMovementTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SsomaMovementTypeResponseDto> ExecuteAsync(long ssomaMovementTypeId)
        {
            var entity = await _repository.GetByIdAsync(ssomaMovementTypeId);
            return _mapper.Map<SsomaMovementTypeResponseDto>(entity);
        }
    }
}

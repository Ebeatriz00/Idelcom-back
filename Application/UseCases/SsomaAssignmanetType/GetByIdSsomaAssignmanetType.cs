using Application.DTOs.SsomaAssignmanetType;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaAssignmanetType
{
    public class GetByIdSsomaAssignmanetType
    {
        private readonly ISsomaAssignmanetTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdSsomaAssignmanetType(ISsomaAssignmanetTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SsomaAssignmanetTypeByIdDto> ExecuteAsync(long ssomaAssignTypeId)
        {
            var entities = await _repository.GetByIdAsync(ssomaAssignTypeId);
            return _mapper.Map<SsomaAssignmanetTypeByIdDto>(entities);
        }
    }
}

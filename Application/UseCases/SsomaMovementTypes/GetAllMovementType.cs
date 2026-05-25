using Application.DTOs.SsomaMovementType;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaMovementTypes
{
    public class GetAllSsomaMovementType
    {
        private readonly ISsomaMovementTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllSsomaMovementType(ISsomaMovementTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<SsomaMovementTypeResponseDto>> ExecuteAsync(
            long businessId,
            string? search,
            int page,
            int pageSize,
            long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);

            return _mapper.Map<PagedResult<SsomaMovementTypeResponseDto>>(entities);
        }
    }
}

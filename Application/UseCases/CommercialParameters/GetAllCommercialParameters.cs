using Application.DTOs.Area;
using Application.DTOs.CommercialParameters;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.CommercialParameters
{
    public class GetAllCommercialParameters
    {
        private readonly ICommercialParametersRepository _repository;
        private readonly IMapper _mapper;
        public GetAllCommercialParameters(ICommercialParametersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<ResponseCommercialParametersDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entity = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<ResponseCommercialParametersDto>>(entity);
        }
    }
}

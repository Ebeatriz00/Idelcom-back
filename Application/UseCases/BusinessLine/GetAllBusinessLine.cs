using Application.DTOs.BusinessLine;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.BusinessLine
{
    public class GetAllBusinessLine
    {
        private readonly IBusinessLineRepository _repository;
        private readonly IMapper _mapper;

        public GetAllBusinessLine(IBusinessLineRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<BusinessLineResponseDto>> ExecuteAsync(int businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<BusinessLineResponseDto>>(entities);
        }
    }
}

using Application.DTOs.LeadsStatus;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.LeadsStatus
{
    public class GetAllLeadsStatus
    {
        private readonly ILeadsStatusRepository _repository;
        private readonly IMapper _mapper;
        public GetAllLeadsStatus(ILeadsStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<LeadsStatusResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<LeadsStatusResponseDto>>(entities);
        }
    }
}

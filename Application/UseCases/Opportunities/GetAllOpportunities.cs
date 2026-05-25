using Application.DTOs.Opportunities;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Opportunities
{
    public class GetAllOpportunities
    {
        private readonly IOpportunitiesRepository _repository;
        private readonly IMapper _mapper;
        public GetAllOpportunities(IOpportunitiesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<OpportunitiesResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersId, long? stateId, long? workerId, DateTime? filterStartDate, DateTime? filterFinishDate, int? filterYear)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersId, stateId, workerId, filterStartDate, filterFinishDate, filterYear);
            return _mapper.Map<PagedResult<OpportunitiesResponseDto>>(entities);
        }
    }
}

using Application.DTOs.JobTitle;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.JobTitle
{
    public class GetAllJobTitle
    {
        private readonly IJobTitleRepository _repository;
        private readonly IMapper _mapper;

        public GetAllJobTitle(IJobTitleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<JobTitleResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<JobTitleResponseDto>>(entities);
        }
    }
}

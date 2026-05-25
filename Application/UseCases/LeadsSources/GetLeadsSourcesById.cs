using Application.DTOs.LeadsSources;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.LeadsSources
{
    public class GetLeadsSourcesById
    {
        private readonly ILeadsSourcesRepository _repository;
        private readonly IMapper _mapper;
        public GetLeadsSourcesById(ILeadsSourcesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<LeadsSourcesResponseDto> ExecuteAsync(long leadsSourcesId)
        {
            var entity = await _repository.GetByIdAsync(leadsSourcesId);
            return _mapper.Map<LeadsSourcesResponseDto>(entity);
        }
    }
}

using Application.DTOs.LeadsStatus;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.LeadsStatus
{
    public class GetByIdLeadsStatus
    {
        private readonly ILeadsStatusRepository _repository;
        private readonly IMapper _mapper;
        public GetByIdLeadsStatus(ILeadsStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<LeadsStatusResponseDto> ExecuteAsync(long leadsStatusId)
        {
            var entity = await _repository.GetByIdAsync(leadsStatusId);
            return _mapper.Map<LeadsStatusResponseDto>(entity);
        }
    }
}

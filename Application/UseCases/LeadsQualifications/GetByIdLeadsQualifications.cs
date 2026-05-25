using Application.DTOs.LeadsQualifications;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.LeadsQualifications
{
    public class GetLeadsQualificationsById
    {
        private readonly ILeadsQualificationsRepository _repository;
        private readonly IMapper _mapper;

        public GetLeadsQualificationsById(ILeadsQualificationsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<LeadsQualificationsResponseDto> ExecuteAsync(long leadsQualificationsId)
        {
            var entity = await _repository.GetByIdAsync(leadsQualificationsId);
            return _mapper.Map<LeadsQualificationsResponseDto>(entity);
        }
    }
}

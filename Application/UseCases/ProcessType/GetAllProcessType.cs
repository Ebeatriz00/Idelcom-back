using Application.DTOs.ProcessType;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ProcessType
{
    public class GetAllProcessType
    {
        private readonly IProcessTypeReporsitory _repository;
        private readonly IMapper _mapper;

        public GetAllProcessType(IProcessTypeReporsitory repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<ProcessTypeResponseDto>> ExecuteAsync(int businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<ProcessTypeResponseDto>>(entities);
        }
    }
}

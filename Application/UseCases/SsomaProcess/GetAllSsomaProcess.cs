using Application.DTOs.SsomaProcess;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Ssoma;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaProcess
{
    public class GetAllSsomaProcess(
        ISsomaProcessRepository repository, 
        IMapper mapper)
    {
        private readonly ISsomaProcessRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<SsomaProcessResponseDto>> ExecuteAsync(long businessId, int page, int pageSize, string? search, long? operationsId = null)
        {
            var entity = await _repository.GetAllAsync(businessId, page, pageSize, search, operationsId);
            return _mapper.Map<PagedResult<SsomaProcessResponseDto>>(entity);
        }
    }
}

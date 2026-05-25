using Application.DTOs.SsomaProcess;
using AutoMapper;
using Core.Interfaces.Ssoma;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaProcess
{
    public class GetByIdSsomaProcess(
        ISsomaProcessRepository repository,
        IMapper mapper)
    {
        private readonly ISsomaProcessRepository _repository = repository;
        private readonly IMapper _mapper = mapper; 
        public async Task<SsomaProcessByIdDto?> ExecuteAsync(long ssomaProcessId, long operationsId, long businessId)
        {
            var entity = await _repository.GetByIdAsync(ssomaProcessId, operationsId, businessId);
            
            if (entity == null)
            {
                return null;
            }

            return _mapper.Map<SsomaProcessByIdDto?>(entity);
        }
    }
}

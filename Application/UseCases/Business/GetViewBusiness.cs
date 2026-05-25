using Application.DTOs.Business;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Business
{
    public class GetViewBusiness
    {
        private readonly IBusinessRepository _repository;
        private readonly IMapper _mapper;
        public GetViewBusiness(IBusinessRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<BusinessViewDto> ExecuteAsync( long BusinessId)
        {
            var entities = await _repository.GetByViewAsync(BusinessId);
            return _mapper.Map<BusinessViewDto>(entities);
        }
    }
}

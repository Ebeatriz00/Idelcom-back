using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaMovementTypes
{
    public class GetSelectSsomaMovementType
    {
        private readonly ISsomaMovementTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectSsomaMovementType(ISsomaMovementTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetForSelectAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}

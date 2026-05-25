using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ConctactsCrm
{
    public class GetSelectContactsCrm
    {
        private readonly IContactsCrmRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectContactsCrm(IContactsCrmRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId, long clientsId, long? workerId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetForSelectAsync(businessId, clientsId, workerId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}

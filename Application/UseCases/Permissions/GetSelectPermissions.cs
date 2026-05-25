using AutoMapper;
using Core.Interfaces;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Permissions
{
    public class GetSelectPermissions
    {
        private readonly IPermissionsRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectPermissions(IPermissionsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetPermissionsForSelectAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }

    }
}

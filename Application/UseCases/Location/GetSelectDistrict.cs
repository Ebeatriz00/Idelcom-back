using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Location
{
    public class GetSelectDistrict
    {
        private readonly ILocationRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectDistrict(ILocationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedSelect<OptionItem>> ExecuteAsync(int departmentId, int provinceId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetDistrictForSelectAsync(departmentId, provinceId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}

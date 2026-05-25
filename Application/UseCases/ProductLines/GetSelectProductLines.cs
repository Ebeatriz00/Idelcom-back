using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ProductLines
{
    public class GetSelectProductLines
    {
        private readonly IProductLinesRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectProductLines(IProductLinesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId,long? categoriesId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetForSelectAsync(businessId, categoriesId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}

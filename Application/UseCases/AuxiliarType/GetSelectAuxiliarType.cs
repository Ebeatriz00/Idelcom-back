using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.AuxiliarType
{
    public class GetSelectAuxiliarType
    {
        private readonly IAuxiliarTypeRepository _auxiliarTypeRepository;
        private readonly IMapper _mapper;
        public GetSelectAuxiliarType(IAuxiliarTypeRepository auxiliarTypeRepository, IMapper mapper)
        {
            _auxiliarTypeRepository = auxiliarTypeRepository;
            _mapper = mapper;
        }
        public async Task<PagedSelect<OptionItem>> ExecuteAsync(string? search, int page, int pageSize)
        {
            var entities = await _auxiliarTypeRepository.GetAuxiliarTypeForSelectAsync(search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}

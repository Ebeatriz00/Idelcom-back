using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.TypeAnalysis
{
    public class GetSelectTypeAnalysis
    {
        private readonly ITypeAnalysisRepository _typeAnalysisRepository;
        private readonly IMapper _mapper;
        public GetSelectTypeAnalysis(ITypeAnalysisRepository typeAnalysisRepository, IMapper mapper)
        {
            _typeAnalysisRepository = typeAnalysisRepository;
            _mapper = mapper;
        }
        public async Task<PagedSelect<OptionItem>> ExecuteAsync(string? search, int page, int pageSize)
        {
            var entities = await _typeAnalysisRepository.GetTypeAnalysisForSelectAsync(search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}

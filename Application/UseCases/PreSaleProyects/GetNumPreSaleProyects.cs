using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.PreSaleProyects
{
    public class GetNumPreSaleProyects
    {
        private readonly IPreSaleProyectsRepository _repository;
        private readonly IMapper _mapper;

        public GetNumPreSaleProyects(IPreSaleProyectsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<string> ExecuteAsync(long businessId)
        {
            return await _repository.GetCodeAsync(businessId);
        }
    }
}

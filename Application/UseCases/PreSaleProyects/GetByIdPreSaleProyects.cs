using Application.DTOs.PreSaleProyects;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.PreSaleProyects
{
    public class GetPreSaleProyectsById
    {
        private readonly IPreSaleProyectsRepository _repository;
        private readonly IMapper _mapper;

        public GetPreSaleProyectsById(IPreSaleProyectsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PreSaleProyectsByIdDto> ExecuteAsync(string linkToken)
        {
            var entity = await _repository.GetByIdAsync(linkToken);
            return _mapper.Map<PreSaleProyectsByIdDto>(entity);
        }
    }
}

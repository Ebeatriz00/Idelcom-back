using Application.DTOs.SupplierGroups;
using AutoMapper;
using Core.Interfaces.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SupplierGroups
{
    public class GetByIdSupplierGroups
    {
        private readonly ISupplierGroupsRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdSupplierGroups(ISupplierGroupsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SupplierGroupsResponseDto> ExecuteAsync(long supplierGroupsId)
        {
            var entity = await _repository.GetByIdAsync(supplierGroupsId);
            return _mapper.Map<SupplierGroupsResponseDto>(entity);
        }
    }
}

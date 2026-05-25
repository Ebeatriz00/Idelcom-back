using Application.DTOs.Suppliers;
using AutoMapper;
using Core.Interfaces.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Suppliers
{
    public class GetSuppliersById
    {
        private readonly ISuppliersRepository _repository;
        private readonly IMapper _mapper;

        public GetSuppliersById(ISuppliersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SuppliersByIdDto> ExecuteAsync(long suppliersId)
        {
            var entity = await _repository.GetByIdAsync(suppliersId);
            return _mapper.Map<SuppliersByIdDto>(entity);
        }
    }
}

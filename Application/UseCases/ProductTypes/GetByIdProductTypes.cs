using Application.DTOs.ProductTypes;
using AutoMapper;
using Core.Interfaces.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ProductTypes
{
    public class GetProductTypesById
    {
        private readonly IProductTypesRepository _repository;
        private readonly IMapper _mapper;

        public GetProductTypesById(IProductTypesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductTypesByIdDto?> ExecuteAsync(long productTypesId)
        {
            var entity = await _repository.GetByIdAsync(productTypesId);

            if (entity == null)
            {
                return null;
            }
            return _mapper.Map<ProductTypesByIdDto?>(entity);
        }
    }
}

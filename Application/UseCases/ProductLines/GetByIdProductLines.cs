using Application.DTOs.ProductLines;
using AutoMapper;
using Core.Interfaces.logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ProductLines
{
    public class GetProductLinesById
    {
        private readonly IProductLinesRepository _repository;
        private readonly IMapper _mapper;

        public GetProductLinesById(IProductLinesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductLinesByIdDto?> ExecuteAsync(long productLinesId)
        {
            var entity = await _repository.GetByIdAsync(productLinesId);
            if (entity == null)
            {
                return null;
            }
            return _mapper.Map<ProductLinesByIdDto?>(entity);
        }
    }
}

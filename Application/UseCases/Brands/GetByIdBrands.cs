using Application.DTOs.Brands;
using AutoMapper;
using Core.Interfaces.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Brands
{
    public class GetBrandsById
    {
        private readonly IBrandsRepository _repository;
        private readonly IMapper _mapper;

        public GetBrandsById(IBrandsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BrandsResponseDto?> ExecuteAsync(long brandsId)
        {
            var entity = await _repository.GetByIdAsync(brandsId);
            if (entity == null)
            {
                return null;
            }
            return _mapper.Map<BrandsResponseDto>(entity);
        }
    }
}

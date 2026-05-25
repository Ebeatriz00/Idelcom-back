using Application.DTOs.Categories;
using AutoMapper;
using Core.Interfaces.logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Categories
{
    public class GetCategoriesById
    {
        private readonly ICategoriesRepository _repository;
        private readonly IMapper _mapper;

        public GetCategoriesById(ICategoriesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CategoriesResponseDto?> ExecuteAsync(long categoriesId)
        {
            var entity = await _repository.GetByIdAsync(categoriesId);

            if (entity == null)
            {
                return null;
            }
            return _mapper.Map<CategoriesResponseDto>(entity);
        }
    }
}

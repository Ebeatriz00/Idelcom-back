using Application.DTOs.FileTrackingProducts;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Logistic;
using Core.Interfaces.Services;
using Org.BouncyCastle.Tsp;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.FileTrackingProducts
{
    public class GetAllProductFiles
    {
        private readonly IFileTrackingProductsRepository _repository;
        private readonly IMapper _mapper;

        public GetAllProductFiles(
            IFileTrackingProductsRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<FileTrackingProductsResponseDto>> ExecuteAsync(long  productsId, long businessId, int page, int pageSize)
        {
           
            var entities = await _repository.GetAllAsync(businessId, productsId, page, pageSize);
            var result = _mapper.Map<PagedResult<FileTrackingProductsResponseDto>>(entities);

            return result;
        }
    }
}

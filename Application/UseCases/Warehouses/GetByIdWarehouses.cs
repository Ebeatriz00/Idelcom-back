using Application.DTOs.Warehouses;
using AutoMapper;
using Core.Interfaces.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Warehouses
{
    public class GetWarehousesById
    {
        private readonly IWarehousesRepository _repository;
        private readonly IMapper _mapper;

        public GetWarehousesById(IWarehousesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<WarehousesByIdDto?> ExecuteAsync(long warehousesId)
        {
            var entity = await _repository.GetByIdAsync(warehousesId);
            return _mapper.Map<WarehousesByIdDto?>(entity);
        }
    }
}

using Application.DTOs.Orders;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Orders
{
    public class GetAllOrders
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IMapper _mapper;

        public GetAllOrders(IOrdersRepository ordersRepository, IMapper mapper)
        {
            _ordersRepository = ordersRepository;
            _mapper = mapper;
        }
        public async Task<PagedResult<OrdersResponseDto>> ExecuteAsync(long businessId, string? search, long? responsibleStaff, int page, int pageSize)
        {
            var entities = await _ordersRepository.GetAllAsync(businessId, search, responsibleStaff, page, pageSize);
            return _mapper.Map<PagedResult<OrdersResponseDto>>(entities);
        }
    }
}

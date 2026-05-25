using Application.DTOs.DashboardPreSales;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.DashboardPreSales
{

    public interface IGetDashboardPreSalesState
    {
        Task<IEnumerable<DashboardPreSalesStateDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year);
    }

    public class GetDashboardPreSalesState : IGetDashboardPreSalesState
    {
        private readonly IDashboardPreSalesRepository _dashboardPreSalesState;
        private readonly IMapper _mapper;

        public GetDashboardPreSalesState(IDashboardPreSalesRepository dashboardPreSalesState, IMapper mapper)
        {
            _dashboardPreSalesState = dashboardPreSalesState;
            _mapper = mapper;
        }
               
        public async Task<IEnumerable<DashboardPreSalesStateDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year)
        {
            var metrics = await _dashboardPreSalesState.GetDashboardPreSalesState(usersId, quarter, usersBy, year);
            return _mapper.Map<IEnumerable<DashboardPreSalesStateDto>>(metrics);
        }
    }
}

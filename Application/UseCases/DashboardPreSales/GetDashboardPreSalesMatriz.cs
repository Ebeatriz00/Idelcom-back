using Application.DTOs.DashboardPreSales;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.DashboardPreSales
{
    public interface IGetDashboardPreSalesMatriz
    {
        Task<IEnumerable<DashboardPreSalesMatrizDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year);
    }
    public class GetDashboardPreSalesMatriz : IGetDashboardPreSalesMatriz
    {
        private readonly IDashboardPreSalesRepository _dashboardPreSalesRepository;
        private readonly IMapper _mapper;
        public GetDashboardPreSalesMatriz(IDashboardPreSalesRepository dashboardPreSalesRepository, IMapper mapper)
        {
            _dashboardPreSalesRepository = dashboardPreSalesRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<DashboardPreSalesMatrizDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year)
        {
            var metrics = await _dashboardPreSalesRepository.GetDashboardPreSalesMatriz(usersId, quarter, usersBy, year);
            return _mapper.Map<IEnumerable<DashboardPreSalesMatrizDto>>(metrics);
        }
    }
}

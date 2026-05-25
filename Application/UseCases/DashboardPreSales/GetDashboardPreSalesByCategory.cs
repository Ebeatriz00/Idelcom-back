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
    public interface IGetDashboardPreSalesByCategory
    {
        Task<IEnumerable<DashboardPreSalesByCategoryDto>> Execute(long? usersId, int? quarter, int? year);
    }
    public class GetDashboardPreSalesByCategory : IGetDashboardPreSalesByCategory
    {
        private readonly IDashboardPreSalesRepository _dashboardPreSalesRepository;
        private readonly IMapper _mapper;

        public GetDashboardPreSalesByCategory(IDashboardPreSalesRepository dashboardPreSalesRepository, IMapper mapper)
        {
            _dashboardPreSalesRepository = dashboardPreSalesRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<DashboardPreSalesByCategoryDto>> Execute(long? usersId, int? quarter, int? year)
        {
            var metrics = await _dashboardPreSalesRepository.GetDashboardPreSalesByCategory(usersId, quarter, year);
            return _mapper.Map<IEnumerable<DashboardPreSalesByCategoryDto>>(metrics);
        }
    }
}

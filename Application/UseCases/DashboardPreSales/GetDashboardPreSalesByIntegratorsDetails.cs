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
    public interface IGetDashboardPreSalesByIntegratorsDetails
    {
        Task<IEnumerable<DashboardPreSalesIntegratorsDetailsDto>> Execute(long? usersId, int? quarter, int? year, long? stateId);
    }
    public class GetDashboardPreSalesByIntegratorsDetails : IGetDashboardPreSalesByIntegratorsDetails
    {
        private readonly IDashboardPreSalesRepository _dashboardPreSalesRepository;
        private readonly IMapper _mapper;

        public GetDashboardPreSalesByIntegratorsDetails(IDashboardPreSalesRepository dashboardPreSalesRepository, IMapper mapper)
        {
            _dashboardPreSalesRepository = dashboardPreSalesRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<DashboardPreSalesIntegratorsDetailsDto>> Execute(long? usersId, int? quarter, int? year, long? stateId)
        {
            var metrics = await _dashboardPreSalesRepository.GetDashboardPreSalesIntegratorsDetails(usersId, quarter, year, stateId);
            return _mapper.Map<IEnumerable<DashboardPreSalesIntegratorsDetailsDto>>(metrics);
        }
    }
}

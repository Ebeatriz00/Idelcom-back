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
    public interface IGetDashboardPreSalesByEngineerDetails
    {
        Task<IEnumerable<DashboardPreSaleByEngineerDetailsDto>> Execute(long? usersId, int? quarter, int? year, long? stateId);
    }
    public class GetDashboardPreSalesByEngineerDetails : IGetDashboardPreSalesByEngineerDetails
    {
        private readonly IDashboardPreSalesRepository _dashboardPreSalesRepository;
        private readonly IMapper _mapper;

        public GetDashboardPreSalesByEngineerDetails(IDashboardPreSalesRepository dashboardPreSalesRepository, IMapper mapper)
        {
            _dashboardPreSalesRepository = dashboardPreSalesRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<DashboardPreSaleByEngineerDetailsDto>> Execute(long? usersId, int? quarter, int? year, long? stateId)
        {
            var metrics = await _dashboardPreSalesRepository.GetDashboardPreSalesByEngineerDetails(usersId, quarter, year, stateId);
            return _mapper.Map<IEnumerable<DashboardPreSaleByEngineerDetailsDto>>(metrics);
        }
    }
}

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

    public interface IGetDashboardPreSalesTotalQuotation
    {
        Task<IEnumerable<DashboardPreSalesQuotationDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year);
    }
    public class GetDashboardPreSalesTotalQuotation : IGetDashboardPreSalesTotalQuotation
    {
        private readonly IDashboardPreSalesRepository _dashboardPreSalesRepository;
        private readonly IMapper _mapper;

        public GetDashboardPreSalesTotalQuotation(IDashboardPreSalesRepository dashboardPreSalesRepository, IMapper mapper)
        {
            _dashboardPreSalesRepository = dashboardPreSalesRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DashboardPreSalesQuotationDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year)
        {
            var metrics = await _dashboardPreSalesRepository.GetTotalQuotation(usersId, quarter, usersBy, year);
            return _mapper.Map<IEnumerable<DashboardPreSalesQuotationDto>>(metrics);
        }
    }
}

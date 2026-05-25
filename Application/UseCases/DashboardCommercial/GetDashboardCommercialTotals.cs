using Application.DTOs.Dashboard;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Dashboard
{
    public interface IGetDashboardTotalsUseCase
    {
        Task<IEnumerable<DashboardCommercialTotalsDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year);
    }

    public class GetDashboardCommercialTotals : IGetDashboardTotalsUseCase
    {
        private readonly IDashboardCommercialRepository _dashboardRepository;
        private readonly IMapper _mapper;
        

        public GetDashboardCommercialTotals(IDashboardCommercialRepository dashboardRepository, IMapper mapper)
        {
            _dashboardRepository = dashboardRepository;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<DashboardCommercialTotalsDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year)
        {
            var metrics = await _dashboardRepository.GetDashboardTotals(usersId, quarter, usersBy, year);
            return _mapper.Map<IEnumerable<DashboardCommercialTotalsDto>>(metrics);
        }

    }
}

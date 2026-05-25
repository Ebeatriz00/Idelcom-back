using Application.DTOs.Dashboard;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Dashboard
{
    public interface IGetDashboardClosingUseCase
    {
        Task<IEnumerable<DashboardCommercialClosingDto>> Execute(long? usersId, int? year, int? quarter, long? usersBy);
    }
    public class GetDashboardClosingUseCase : IGetDashboardClosingUseCase
    {
        private readonly IDashboardCommercialRepository _dashboardRepository;
        private readonly IMapper _mapper;

        public GetDashboardClosingUseCase(IDashboardCommercialRepository dashboardRepository, IMapper mapper)
        {
            _dashboardRepository = dashboardRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DashboardCommercialClosingDto>> Execute(long? usersId, int? year, int? quarter, long? usersBy)
        {
            var metrics = await _dashboardRepository.GetDashboardClosingMetrics(usersId, year, quarter, usersBy);
            return _mapper.Map<IEnumerable<DashboardCommercialClosingDto>>(metrics);
        }
    }
}

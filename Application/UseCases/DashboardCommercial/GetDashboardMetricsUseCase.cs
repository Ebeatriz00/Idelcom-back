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
    public interface IGetDashboardMetricsUseCase
    {
        Task<IEnumerable<StateOpportunityMetricDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year);
    }

    public class GetDashboardMetricsUseCase : IGetDashboardMetricsUseCase
    {
        private readonly IDashboardCommercialRepository _dashboardRepository;
        private readonly IMapper _mapper;

        public GetDashboardMetricsUseCase(IDashboardCommercialRepository dashboardRepository, IMapper mapper)
        {
            _dashboardRepository = dashboardRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StateOpportunityMetricDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year) 
        {
            var metrics = await _dashboardRepository.GetMetricsByStateOpportunity(usersId, quarter, usersBy, year);
            return _mapper.Map<IEnumerable<StateOpportunityMetricDto>>(metrics);
        }
    }
}

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
    public interface IGetDashboardMetricsClientsUseCase
    {
        Task<IEnumerable<ClientMetricDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year);
    }

    public class GetDashboardMetricsClientsUseCase : IGetDashboardMetricsClientsUseCase
    {
        private readonly IDashboardCommercialRepository _dashboardRepository;
        private readonly IMapper _mapper;

        public GetDashboardMetricsClientsUseCase(IDashboardCommercialRepository dashboardRepository, IMapper mapper)
        {
            _dashboardRepository = dashboardRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClientMetricDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year)
        {
            var metrics = await _dashboardRepository.GetMetricsClients(usersId, quarter, usersBy, year);

            return _mapper.Map<IEnumerable<ClientMetricDto>>(metrics);
        }
    }
}

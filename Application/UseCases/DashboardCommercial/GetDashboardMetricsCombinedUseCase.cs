using Application.DTOs.Dashboard;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Dashboard
{
    public interface IGetDashboardMetricsCombinedUseCase
    {
        Task<IEnumerable<CombinedMetricDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year);
    }

    public class GetDashboardMetricsCombinedUseCase : IGetDashboardMetricsCombinedUseCase
    {
        private readonly IDashboardCommercialRepository _dashboardRepository;

        public GetDashboardMetricsCombinedUseCase(IDashboardCommercialRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<IEnumerable<CombinedMetricDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year)
        {
            var metricsEntity = await _dashboardRepository.GetMetricsCombined(usersId, quarter, usersBy, year);
            var metricsDto = metricsEntity.Select(m => new CombinedMetricDto
            {
                QuarterNum = m.QuarterNum,
                StateName = m.StateName,
                StateColor = m.StateColor,
                Quantity = m.Quantity
            });

            return metricsDto;
        }
    }
}

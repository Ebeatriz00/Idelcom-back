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
    public interface IGetDashboardMetricsQuarterUseCase
    {
        Task<IEnumerable<QuarterMetricDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year);
    }

    public class GetDashboardMetricsQuarterUseCase : IGetDashboardMetricsQuarterUseCase
    {
        private readonly IDashboardCommercialRepository _dashboardRepository;
        private readonly IMapper _mapper; 

        public GetDashboardMetricsQuarterUseCase(IDashboardCommercialRepository dashboardRepository, IMapper mapper)
        {
            _dashboardRepository = dashboardRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<QuarterMetricDto>> Execute(long? usersId, int? quarter, long? usersBy, int? year)
        {
            var metrics = await _dashboardRepository.GetMetricsByQuarter(usersId, quarter, usersBy, year);

            var dtos = new List<QuarterMetricDto>();
            foreach (var m in metrics)
            {
                dtos.Add(new QuarterMetricDto { QuarterNum = m.QuarterNum, Quantity = m.Quantity });
            }
            return dtos;
        }
    }
}

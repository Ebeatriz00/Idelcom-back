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
    public interface IGetDashboardProbabilityUseCase
    {
        Task<IEnumerable<DashboardCommercialProbabilityDto>> Execute(long? usersId, int? quarter, long? usersBy, decimal? probability, int? year);
    }
    public class GetDashboardProbabilityAmountUseCase : IGetDashboardProbabilityUseCase
    {
        private readonly IDashboardCommercialRepository _dashboardRepository;
        private readonly IMapper _mapper;

        public GetDashboardProbabilityAmountUseCase(IDashboardCommercialRepository dashboardRepository, IMapper mapper)
        {
            _dashboardRepository = dashboardRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DashboardCommercialProbabilityDto>> Execute(long? usersId, int? quarter, long? usersBy, decimal? probability, int? year)
        {
            var metrics = await _dashboardRepository.GetDashboardProbabilityAmount(usersId, quarter, usersBy, probability, year);
            return _mapper.Map<IEnumerable<DashboardCommercialProbabilityDto>>(metrics);
        }
    }
}

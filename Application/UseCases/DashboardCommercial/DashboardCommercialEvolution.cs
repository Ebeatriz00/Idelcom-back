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
    public interface IGetDashboardEvolutionUseCase
    {
        Task<IEnumerable<DashboardCommercialEvolutionDto>> Execute(long? usersId, int? year, int? quarter, long? usersBy);
    }
    public class GetDashboardCommercialEvolutionUseCase : IGetDashboardEvolutionUseCase
    {
        private readonly IDashboardCommercialRepository _dashboardRepository;
        private readonly IMapper _mapper;

        public GetDashboardCommercialEvolutionUseCase(IDashboardCommercialRepository dashboardRepository, IMapper mapper)
        {
            _dashboardRepository = dashboardRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DashboardCommercialEvolutionDto>> Execute(long? usersId, int? year, int? quarter, long? usersBy)
        {
            var metrics = await _dashboardRepository.GetDashboardCommercialEvolution(usersId, year, quarter, usersBy);
            return _mapper.Map<IEnumerable<DashboardCommercialEvolutionDto>>(metrics);
        }
    }
}

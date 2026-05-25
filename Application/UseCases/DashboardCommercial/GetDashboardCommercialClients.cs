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
    public interface IGetDashboardClientOpportunityUseCase
    {
        Task<IEnumerable<DashboardCommercialClientDto>> Execute(long? usersId, int? year, int? quarter, long? usersBy, long? clientId = null);
    }

    public class GetDashboardClientOpportunityUseCase : IGetDashboardClientOpportunityUseCase
    {
        private readonly IDashboardCommercialRepository _dashboardRepository;
        private readonly IMapper _mapper;

        public GetDashboardClientOpportunityUseCase(IDashboardCommercialRepository dashboardRepository, IMapper mapper)
        {
            _dashboardRepository = dashboardRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DashboardCommercialClientDto>> Execute(long? usersId, int? year, int? quarter, long? usersBy, long? clientId = null)
        {
            var metrics = await _dashboardRepository.GetDashboardClientOpportunity(usersId, year, quarter, usersBy, clientId);
            return _mapper.Map<IEnumerable<DashboardCommercialClientDto>>(metrics);
        }
    }
}

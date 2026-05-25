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
    public interface IGetDashboardPreSalesCollaborator
    {
        Task<IEnumerable<DashboardPreSalesCollaboratorDto>> Execute(long? usersId, int? quarter, int? year, long? stateId);
    }
    public class GetDashboardPreSalesCollaborator : IGetDashboardPreSalesCollaborator
    {
        private readonly IDashboardPreSalesRepository _dashboardPreSalesRepository;
        private readonly IMapper _mapper;
        public GetDashboardPreSalesCollaborator(IDashboardPreSalesRepository dashboardPreSalesRepository, IMapper mapper)
        {
            _dashboardPreSalesRepository = dashboardPreSalesRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DashboardPreSalesCollaboratorDto>> Execute(long? usersId, int? quarter, int? year, long? stateId)
        {
            var metrics = await _dashboardPreSalesRepository.GetDashboardPreSalesCollaborator(usersId, quarter, year, stateId);
            return _mapper.Map<IEnumerable<DashboardPreSalesCollaboratorDto>>(metrics);
        }
    }
}

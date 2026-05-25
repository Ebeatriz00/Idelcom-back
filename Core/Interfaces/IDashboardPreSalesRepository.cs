using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IDashboardPreSalesRepository
    {
        Task<IEnumerable<DashboardPreSalesQuotation>> GetTotalQuotation(long? usersId, int? quarter, long? usersBy, int? year);
        Task<IEnumerable<DashboardPreSalesState>> GetDashboardPreSalesState(long? usersId, int? quarter, long? usersBy, int? year);
        Task<IEnumerable<DashboardPreSalesCombined>> GetDashboardPreSalesCombined(long? usersId, int? quarter, long? usersBy, int? year);
        Task<IEnumerable<DashboardPreSalesByEngineer>> GetDashboardPreSalesByEngineer(long? usersId, int? quarter, int? year, long? stateId);
        Task<IEnumerable<DashboardPreSalesMatriz>> GetDashboardPreSalesMatriz(long? usersId, int? quarter, long? usersBy, int? year );
        Task<IEnumerable<DashboardPreSalesCollaborator>> GetDashboardPreSalesCollaborator(long? usersId, int? quarter, int? year, long? stateId);
        Task<IEnumerable<DashboardPreSalesIntegrators>> GetDashboardPreSalesIntegrators(long? usersId, int? quarter, int? year, long? stateId);
        Task<IEnumerable<DashboardPreSaleByEngineerDetails>> GetDashboardPreSalesByEngineerDetails(long? usersId, int? quarter, int? year, long? stateId);
        Task<IEnumerable<DashboardPreSalesIntegratorsDetails>> GetDashboardPreSalesIntegratorsDetails(long? usersId, int? quarter, int? year, long? stateId);
        Task<IEnumerable<DashboardPreSalesCollaboratorDetails>> GetDashboardPreSalesCollaboratorDetails(long? usersId, int? quarter, int? year, long? stateId);
        Task<IEnumerable<DashboardPreSalesByCategory>> GetDashboardPreSalesByCategory(long? usersId, int? quarter, int? year);
    }
}

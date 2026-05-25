using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IDashboardCommercialRepository
    {
        Task<IEnumerable<StateOpportunityMetric>> GetMetricsByStateOpportunity(long? usersId, int? quarter, long? usersBy, int? year);
        Task<IEnumerable<ClientMetric>> GetMetricsClients(long? usersId, int? quarter, long? usersBy, int? year);
        Task<IEnumerable<QuarterMetric>> GetMetricsByQuarter(long? usersId, int? quarter, long? usersBy, int? year);
        Task<IEnumerable<CombinedMetric>> GetMetricsCombined(long? usersId, int? quarter, long? usersBy, int? year);
        Task<IEnumerable<DashboardCommercialProbability>> GetDashboardProbabilityAmount(long? usersId, int? quarter, long? usersBy, decimal? probability, int? year);
        Task<IEnumerable<DashboardCommercialEvolution>> GetDashboardCommercialEvolution(long? usersId, int? year, int? quarter, long? usersBy);
        Task<IEnumerable<DashboardCommercialClosing>> GetDashboardClosingMetrics(long? usersId, int? year, int? quarter, long? usersBy);
        Task<IEnumerable<DashboardCommercialClient>> GetDashboardClientOpportunity(long? usersId, int? year, int? quarter, long? usersBy, long? clientId = null);
        Task<IEnumerable<DashboardCommercialTotals>> GetDashboardTotals(long? usersId, int? quarter, long? usersBy, int? year);
    }
}

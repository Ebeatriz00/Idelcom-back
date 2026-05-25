using Application.UseCases.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardCommercialController : ControllerBase
    {
        private readonly IGetDashboardMetricsUseCase _getDashboardMetricsUseCase;
        private readonly IGetDashboardMetricsClientsUseCase _getDashboardMetricsClientsUseCase;
        private readonly IGetDashboardMetricsQuarterUseCase _getDashboardMetricsQuarterUseCase;
        private readonly IGetDashboardMetricsCombinedUseCase _getMetricsCombinedUseCase;
        private readonly IGetDashboardProbabilityUseCase _getDashboardProbabilityUseCase;
        private readonly IGetDashboardEvolutionUseCase _getDashboardEvolutionUseCase;
        private readonly IGetDashboardClosingUseCase _getDashboardClosingUseCase;
        private readonly IGetDashboardClientOpportunityUseCase _getDashboardClientOpportunityUseCase;
        private readonly IGetDashboardTotalsUseCase _getDashboardTotalsUseCase;


        public DashboardCommercialController(IGetDashboardMetricsUseCase getDashboardMetricsUseCase, 
            IGetDashboardMetricsClientsUseCase getDashboardMetricsClientsUseCase, 
            IGetDashboardMetricsQuarterUseCase getDashboardMetricsQuarterUseCase, 
            IGetDashboardMetricsCombinedUseCase getMetricsCombinedUseCase, 
            IGetDashboardProbabilityUseCase getDashboardProbabilityUseCase, 
            IGetDashboardEvolutionUseCase getDashboardEvolutionUseCase, 
            IGetDashboardClosingUseCase getDashboardClosingUseCase, 
            IGetDashboardClientOpportunityUseCase getDashboardClientOpportunityUseCase, 
            IGetDashboardTotalsUseCase getDashboardTotalsUseCase)
        {
            _getDashboardMetricsUseCase = getDashboardMetricsUseCase;
            _getDashboardMetricsClientsUseCase = getDashboardMetricsClientsUseCase;
            _getDashboardMetricsQuarterUseCase = getDashboardMetricsQuarterUseCase;
            _getMetricsCombinedUseCase = getMetricsCombinedUseCase;
            _getDashboardProbabilityUseCase = getDashboardProbabilityUseCase;
            _getDashboardEvolutionUseCase = getDashboardEvolutionUseCase;
            _getDashboardClosingUseCase = getDashboardClosingUseCase;
            _getDashboardClientOpportunityUseCase = getDashboardClientOpportunityUseCase;
            _getDashboardTotalsUseCase = getDashboardTotalsUseCase;
        }

        [HttpGet("metricState")]
        public async Task<IActionResult> GetMetricsByState([FromQuery] long? usersId, [FromQuery] int? quarter, [FromQuery] long? usersBy, [FromQuery] int? year)
        {
            var result = await _getDashboardMetricsUseCase.Execute(usersId, quarter, usersBy, year);
            return Ok(result);
        }

        [HttpGet("metrics-clients")]
        public async Task<IActionResult> GetMetricsClients([FromQuery] long? usersId, [FromQuery] int? quarter, [FromQuery] long? usersBy, [FromQuery] int? year)
        {
            var result = await _getDashboardMetricsClientsUseCase.Execute(usersId, quarter, usersBy, year);
            return Ok(result);
        }

        [HttpGet("metrics-quarter")]
        public async Task<IActionResult> GetMetricsQuarter([FromQuery] long? usersId, [FromQuery] int? quarter, [FromQuery] long? usersBy, [FromQuery] int? year)
        {
            var result = await _getDashboardMetricsQuarterUseCase.Execute(usersId, quarter, usersBy, year);
            return Ok(result);
        }

        [HttpGet("metrics-combined")]
        public async Task<IActionResult> GetMetricsCombined([FromQuery] long? usersId, [FromQuery] int? quarter, [FromQuery] long? usersBy, [FromQuery] int? year)
        {
            var result = await _getMetricsCombinedUseCase.Execute(usersId, quarter, usersBy, year);
            return Ok(result);
        }
        [HttpGet("metrics-probability")]
        public async Task<IActionResult> GetProbabilityAmount(
            [FromQuery] long? usersId,
            [FromQuery] int? quarter,
            [FromQuery] long? usersBy,
            [FromQuery] decimal? probability,
            [FromQuery] int? year) 
        {
            var result = await _getDashboardProbabilityUseCase.Execute(usersId, quarter, usersBy, probability, year);
            return Ok(result);
        }

        [HttpGet("metrics-evolution")]
        public async Task<IActionResult> GetCommercialEvolution(
            [FromQuery] long? usersId,
            [FromQuery] int? year,  
            [FromQuery] int? quarter,
            [FromQuery] long? usersBy)
        {
            var result = await _getDashboardEvolutionUseCase.Execute(usersId, year, quarter, usersBy);
            return Ok(result);
        }

        [HttpGet("metrics-closing")]
        public async Task<IActionResult> GetClosingMetrics(
            [FromQuery] long? usersId,
            [FromQuery] int? year,     
            [FromQuery] int? quarter,
            [FromQuery] long? usersBy)
        {
            var result = await _getDashboardClosingUseCase.Execute(usersId, year, quarter, usersBy);
            return Ok(result);
        }


        [HttpGet("metrics-client-opportunity")]
        public async Task<IActionResult> GetClientOpportunityMetrics(
            [FromQuery] long? usersId,
            [FromQuery] int? year,
            [FromQuery] int? quarter,
            [FromQuery] long? usersBy,
            [FromQuery] long? clientId) 
        {
            var result = await _getDashboardClientOpportunityUseCase.Execute(usersId, year, quarter, usersBy, clientId);
            return Ok(result);
        }

        [HttpGet("metricsQuotation")]
        public async Task<IActionResult> GetTotals(
            [FromQuery] long? usersId,
            [FromQuery] int? quarter,
            [FromQuery] long? usersBy,
            [FromQuery] int? year
            )
        {
            var result = await _getDashboardTotalsUseCase.Execute(usersId, quarter, usersBy, year);
            return Ok(result);
        }

    }
}


using Application.UseCases.Dashboard;
using Application.UseCases.DashboardPreSales;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardPreSalesController : ControllerBase
    {
        private readonly IGetDashboardPreSalesTotalQuotation _getDashboardTotalQuotation;
        private readonly IGetDashboardPreSalesState _getDashboardPreSalesState;
        private readonly IGetDashboardPreSalesCombined _getDashboardPreSalesCombined;
        private readonly IGetDashboardPreSalesByEngineer _getDashboardPreSalesByEngineer;
        private readonly IGetDashboardPreSalesMatriz _getDashboardPreSalesMatriz;
        private readonly IGetDashboardPreSalesCollaborator _getdashboardPreSalesCollaborator;
        private readonly IGetDashboardPreSalesIntegrators _getDashboardPreSalesIntegrators;
        private readonly IGetDashboardPreSalesByEngineerDetails _getDashboardPreSalesByEngineerDetails;
        private readonly IGetDashboardPreSalesByIntegratorsDetails _getDashboardPreSalesByIntegratorsDetails;
        private readonly IGetDashboardPreSalesByCollaboratorDetails _getDashboardPreSalesByCollaboratorDetails;
        private readonly IGetDashboardPreSalesByCategory _getDashboardPreSalesByCategory;

        public DashboardPreSalesController(
            IGetDashboardPreSalesTotalQuotation getDashboardTotalQuotation,
            IGetDashboardPreSalesState getDashboardPreSalesState,
            IGetDashboardPreSalesCombined getDashboardPreSalesCombined,
            IGetDashboardPreSalesByEngineer getDashboardPreSalesByEngineer,
            IGetDashboardPreSalesMatriz getDashboardPreSalesMatriz,
            IGetDashboardPreSalesCollaborator getDashboardPreSalesCollaborator,
            IGetDashboardPreSalesIntegrators getDashboardPreSalesIntegrators,
            IGetDashboardPreSalesByEngineerDetails getDashboardPreSalesByEngineerDetails,
            IGetDashboardPreSalesByIntegratorsDetails getDashboardPreSalesByIntegratorsDetails,
            IGetDashboardPreSalesByCollaboratorDetails getDashboardPreSalesByCollaboratorDetails,
            IGetDashboardPreSalesByCategory getDashboardPreSalesByCategory
)
        {
            _getDashboardTotalQuotation = getDashboardTotalQuotation;
            _getDashboardPreSalesState = getDashboardPreSalesState;
            _getDashboardPreSalesCombined = getDashboardPreSalesCombined;
            _getDashboardPreSalesByEngineer = getDashboardPreSalesByEngineer;
            _getDashboardPreSalesMatriz = getDashboardPreSalesMatriz;
            _getdashboardPreSalesCollaborator = getDashboardPreSalesCollaborator;
            _getDashboardPreSalesIntegrators = getDashboardPreSalesIntegrators;
            _getDashboardPreSalesByEngineerDetails = getDashboardPreSalesByEngineerDetails;
            _getDashboardPreSalesByIntegratorsDetails = getDashboardPreSalesByIntegratorsDetails;
            _getDashboardPreSalesByCollaboratorDetails = getDashboardPreSalesByCollaboratorDetails;
            _getDashboardPreSalesByCategory = getDashboardPreSalesByCategory;

        }



        [HttpGet("QuotaionTotals")]
        public async Task<IActionResult> GetQuotationTotals(
            [FromQuery] long? usersId,
            [FromQuery] int? quarter,
            [FromQuery] long? usersBy,
            [FromQuery] int? year
            )
        {
            var result = await _getDashboardTotalQuotation.Execute(usersId, quarter, usersBy, year);
            return Ok(result);
        }

        [HttpGet("ByStates")]
        public async Task<IActionResult> GetDashboardStates(
            [FromQuery] long? usersId,
            [FromQuery] int? quarter,
            [FromQuery] long? usersBy,
            [FromQuery] int? year
            )
        {
            var result = await _getDashboardPreSalesState.Execute(usersId, quarter, usersBy, year);
            return Ok(result);
        }

        [HttpGet("Combined")]
        public async Task<IActionResult> GetDashboardCombined(
            [FromQuery] long? usersId,
            [FromQuery] int? quarter,
            [FromQuery] long? usersBy,
            [FromQuery] int? year
            )
        {
            var result = await _getDashboardPreSalesCombined.Execute(usersId, quarter, usersBy, year);
            return Ok(result);
        }

        [HttpGet("ByEngineer")]
        public async Task<IActionResult> GetDashboardByEngineer(
            [FromQuery] long? usersId,
            [FromQuery] int? quarter,
            [FromQuery] int? year,
            [FromQuery] long? stateId
            )
        {
            var result = await _getDashboardPreSalesByEngineer.Execute(usersId, quarter, year, stateId);
            return Ok(result);
        }

        [HttpGet("Matriz")]
        public async Task<IActionResult> GetDashboardMatriz(
            [FromQuery] long? usersId,
            [FromQuery] int? quarter,
            [FromQuery] long? usersBy,
            [FromQuery] int? year
            )
        {
            var result = await _getDashboardPreSalesMatriz.Execute(usersId, quarter, usersBy, year);
            return Ok(result);
        }

        [HttpGet("ByCollaborator")]
        public async Task<IActionResult> GetDashboardByCollaborator(
            [FromQuery] long? usersId,
            [FromQuery] int? quarter,
            [FromQuery] int? year,
            [FromQuery] long? stateId
            )
        {
            var result = await _getdashboardPreSalesCollaborator.Execute(usersId, quarter, year, stateId);
            return Ok(result);
        }

        [HttpGet("ByIntegrators")]
        public async Task<IActionResult> GetDashboardByIntegrators(
            [FromQuery] long? usersId,
            [FromQuery] int? quarter,
            [FromQuery] int? year,
            [FromQuery] long? stateId
            )
        {
            var result = await _getDashboardPreSalesIntegrators.Execute(usersId, quarter, year, stateId);
            return Ok(result);

        }

        [HttpGet("ByEngineerDetails")]
        public async Task<IActionResult> GetDashboardByEngineerDetails(
            [FromQuery] long? usersId,
            [FromQuery] int? quarter,
            [FromQuery] int? year,
            [FromQuery] long? stateId
            )
        {
            var result = await _getDashboardPreSalesByEngineerDetails.Execute(usersId, quarter, year, stateId);
            return Ok(result);
        }

        [HttpGet("ByIntegratorsDetails")]
        public async Task<IActionResult> GetDashboardByIntegratorsDetails(
            [FromQuery] long? usersId,
            [FromQuery] int? quarter,
            [FromQuery] int? year,
            [FromQuery] long? stateId
            )
        {
            var result = await _getDashboardPreSalesByIntegratorsDetails.Execute(usersId, quarter, year, stateId);
            return Ok(result);
        }

        [HttpGet("ByCollaboratorDetails")]
        public async Task<IActionResult> GetDashboardByCollaboratorDetails(
            [FromQuery] long? usersId,
            [FromQuery] int? quarter,
            [FromQuery] int? year,
            [FromQuery] long? stateId
            )
        {
            var result = await _getDashboardPreSalesByCollaboratorDetails.Execute(usersId, quarter, year, stateId);
            return Ok(result);
        }

        [HttpGet("ByCategory")]
        public async Task<IActionResult> GetDashboardByCategory(
            [FromQuery] long? usersId,
            [FromQuery] int? quarter,
            [FromQuery] int? year
            )
        {
            var result = await _getDashboardPreSalesByCategory.Execute(usersId, quarter, year);
            return Ok(result);
        }
    }
}


using Application.DTOs.QuotationLogisticsSuggestion;
using Application.UseCases.QuotationLogisticsSuggestion;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationLogisticsSuggestionController(
        GenerateQuotationLogisticsSuggestionsUseCase generateSuggestions,
        ListQuotationLogisticsSuggestionsUseCase listSuggestions,
        UpdateQuotationLogisticsSuggestionUseCase updateSuggestion,
        AddManualQuotationLogisticsSuggestionUseCase addManualSuggestion,
        AssignWorkOrderQuotationLogisticsSuggestionUseCase assignWorkOrderSuggestion,
        DisableQuotationLogisticsSuggestionUseCase disableSuggestion) : BaseController
    {
        private readonly GenerateQuotationLogisticsSuggestionsUseCase _generateSuggestions = generateSuggestions;
        private readonly ListQuotationLogisticsSuggestionsUseCase _listSuggestions = listSuggestions;
        private readonly UpdateQuotationLogisticsSuggestionUseCase _updateSuggestion = updateSuggestion;
        private readonly AddManualQuotationLogisticsSuggestionUseCase _addManualSuggestion = addManualSuggestion;
        private readonly AssignWorkOrderQuotationLogisticsSuggestionUseCase _assignWorkOrderSuggestion = assignWorkOrderSuggestion;
        private readonly DisableQuotationLogisticsSuggestionUseCase _disableSuggestion = disableSuggestion;

        [HttpPost]
        [Route("Generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateQuotationLogisticsSuggestionDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _generateSuggestions.ExecuteAsync(request, businessId, userId);
            return Ok(result);
        }

        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> List([FromQuery] ListQuotationLogisticsSuggestionFilterDto filter)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _listSuggestions.ExecuteAsync(filter, businessId);
            return Ok(result);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateQuotationLogisticsSuggestionDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updateSuggestion.ExecuteAsync(request, businessId, userId);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddManual")]
        public async Task<IActionResult> AddManual([FromBody] AddManualQuotationLogisticsSuggestionDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _addManualSuggestion.ExecuteAsync(request, businessId, userId);
            return Ok(result);
        }

        [HttpPatch]
        [Route("AssignWorkOrder")]
        public async Task<IActionResult> AssignWorkOrder([FromBody] AssignWorkOrderQuotationLogisticsSuggestionDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _assignWorkOrderSuggestion.ExecuteAsync(request, businessId, userId);
            return Ok(result);
        }

        [HttpPatch]
        [Route("Disable/{id:long}")]
        public async Task<IActionResult> Disable(long id)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _disableSuggestion.ExecuteAsync(businessId, id, userId);
            return Ok(result);
        }
    }
}

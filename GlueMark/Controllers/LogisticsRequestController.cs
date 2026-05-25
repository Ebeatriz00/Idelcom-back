using Application.DTOs.LogisticsRequest;
using Application.UseCases.LogisticsRequest;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogisticsRequestController(
        CreateLogisticsRequestFromSelectedSuggestionsUseCase createFromSelectedSuggestions) : BaseController
    {
        private readonly CreateLogisticsRequestFromSelectedSuggestionsUseCase _createFromSelectedSuggestions = createFromSelectedSuggestions;

        [HttpPost]
        [Route("CreateFromSelectedSuggestions")]
        public async Task<IActionResult> CreateFromSelectedSuggestions([FromBody] CreateLogisticsRequestFromSelectedSuggestionsDto request)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createFromSelectedSuggestions.ExecuteAsync(request, businessId, userId);
            return Ok(result);
        }
    }
}

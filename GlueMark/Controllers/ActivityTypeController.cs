using Application.UseCases.ActivityState;
using Application.UseCases.ActivityType;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityTypeController : ControllerBase
    {
        private readonly CreateActivityType _createActivityType;
        private readonly GetAllActivityType _getAllActivityType;
        private readonly GetByIdActivityType _getByIdActivityType;
        private readonly UpdateActivityType _updateActivityType;
        private readonly PatchActivityType _patchActivityType;
        private readonly GetSelectActivityType _getSelectActivityType;
        private readonly ILinkTokenService _linkToken;

        public ActivityTypeController(CreateActivityType createActivityType, GetAllActivityType getAllActivityType, GetByIdActivityType getByIdActivityType, UpdateActivityType updateActivityType, PatchActivityType patchActivityType, GetSelectActivityType getSelectActivityType, ILinkTokenService linkToken)
        {
            _createActivityType = createActivityType;
            _getAllActivityType = getAllActivityType;
            _getByIdActivityType = getByIdActivityType;
            _updateActivityType = updateActivityType;
            _patchActivityType = patchActivityType;
            _getSelectActivityType = getSelectActivityType;
            _linkToken = linkToken;
        }
        [HttpGet]
        [Route("ActivityTypeSelect")]
        public async Task<IActionResult> ActivityTypeSelect(
            [FromQuery] long businessId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectActivityType.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }
    }
}

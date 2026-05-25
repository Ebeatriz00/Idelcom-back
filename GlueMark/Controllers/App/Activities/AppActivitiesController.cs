using Application.UseCases.Operations.OperationsWorkOrderActivity;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.App.Activities
{
    [Route("api/app/activities")]
    public class AppActivitiesController(GetAppActivitiesByResponsible useCase) : AppBaseController
    {
        private readonly GetAppActivitiesByResponsible _useCase = useCase;

        [HttpGet("get-activities")]
        public async Task<IActionResult> GetActivities()
        {
            var userId = GetCurrentAppUserId();
            var businessId = GetCurrentAppBusinessId();

            var result = await _useCase.ExecuteAsync(userId, businessId);
            return Ok(result);
        }
    }
}

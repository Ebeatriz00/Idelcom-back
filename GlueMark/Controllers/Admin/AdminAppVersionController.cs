using Application.DTOs.MobileAppVersion;
using Application.UseCases.MobileAppVersion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Admin
{
    [Route("api/admin/app-version")]
    [ApiController]
    [Authorize(Policy = "RequireAppVersionAdmin")]
    public class AdminAppVersionController(UpsertMobileAppVersionConfig upsertMobileAppVersionConfig) : ControllerBase
    {
        private readonly UpsertMobileAppVersionConfig _upsertMobileAppVersionConfig = upsertMobileAppVersionConfig;

        [HttpPut]
        public async Task<IActionResult> Put(
            [FromBody] MobileAppVersionUpsertDto request,
            CancellationToken cancellationToken = default)
        {
            var result = await _upsertMobileAppVersionConfig.ExecuteAsync(request, cancellationToken);
            return Ok(result);
        }
    }
}

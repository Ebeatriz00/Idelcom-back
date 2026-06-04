using Application.DTOs.MobileAppVersion;
using Application.UseCases.MobileAppVersion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.App
{
    [Route("api/app/version")]
    [ApiController]
    [AllowAnonymous]
    public class AppVersionController(GetMobileAppVersionConfig getMobileAppVersionConfig) : ControllerBase
    {
        private readonly GetMobileAppVersionConfig _getMobileAppVersionConfig = getMobileAppVersionConfig;

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string platform = "android",
            [FromQuery] string environment = "prod",
            CancellationToken cancellationToken = default)
        {
            var result = await _getMobileAppVersionConfig.ExecuteAsync(
                new MobileAppVersionQueryDto
                {
                    Platform = platform,
                    Environment = environment
                },
                cancellationToken);

            return Ok(result);
        }
    }
}

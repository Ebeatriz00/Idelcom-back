using Application.DTOs.AppAuth;
using Application.UseCases.AppAuth;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.App.Auth
{
    [Route("api/app/auth")]
    public class AppAuthController(AppLoginUseCase loginUseCase) : AppBaseController
    {
        private readonly AppLoginUseCase _loginUseCase = loginUseCase;

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AppLoginRequestDto request, CancellationToken ct)
        {
            var ip = GetClientIp();
            var response = await _loginUseCase.ExecuteAsync(request, ip, ct);
            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromServices] AppLogoutUseCase logoutUseCase, CancellationToken ct)
        {
            var jti = GetCurrentAppJti();
            var response = await logoutUseCase.ExecuteAsync(jti, ct);
            return Ok(response);
        }
    }
}

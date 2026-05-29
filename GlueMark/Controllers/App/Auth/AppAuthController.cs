using Application.DTOs.AppAuth;
using Application.Exceptions;
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
            try
            {
                var ip = GetClientIp();
                var response = await _loginUseCase.ExecuteAsync(request, ip, ct);
                return Ok(response);
            }
            catch (AuthLockedOutException ex)
            {
                var retrySeconds = ex.RetryAfter is TimeSpan ra
                    ? (int)Math.Ceiling(ra.TotalSeconds)
                    : (int?)null;

                if (retrySeconds.HasValue)
                    Response.Headers["Retry-After"] = retrySeconds.Value.ToString();

                return StatusCode(StatusCodes.Status429TooManyRequests, new
                {
                    status = 0,
                    code = "AUTH_LOCKED_OUT",
                    errors = ex.Errors,
                    retryAfterSeconds = retrySeconds
                });
            }
            catch (AuthInvalidCredentialsException ex)
            {
                return Unauthorized(new
                {
                    status = 0,
                    code = "AUTH_INVALID_CREDENTIALS",
                    errors = new[] { new { code = "2001", message = ex.Message } },
                    attemptsRemaining = ex.AttemptsRemaining
                });
            }
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

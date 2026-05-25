using Application.DTOs.Auth;
using Application.Exceptions;
using Application.UseCases.Auth; 
using Core.Options;
using Core.Interfaces.Abstractions;
using Core.Interfaces.Services;
using GlueMark.Extensions;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json.Linq;
using SharedKernel;
using System.IdentityModel.Tokens.Jwt;
using ITokenService = Core.Interfaces.Services.ITokenService;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace GlueMark.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public sealed class AuthController : ControllerBase
    {
        private readonly GetAuth _getAuth;
        private readonly ITokenService _tokens;
        private readonly IRefreshTokenService _refreshTokens;
        private readonly ITokenBlacklist _blacklist;
        private readonly JwtOptions _jwtOptions;

        public AuthController(
            GetAuth getAuth,
            ITokenService tokens,
            IRefreshTokenService refreshTokens,
            ITokenBlacklist blacklist,
            IOptions<JwtOptions> jwtOptions)
        {
            _getAuth = getAuth;
            _tokens = tokens;
            _refreshTokens = refreshTokens;
            _blacklist = blacklist;
            _jwtOptions = jwtOptions.Value;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] AuthRequestDto dto, CancellationToken ct)
        {
            try
            {
                var ip = HttpContext.GetClientIp();
                var result = await _getAuth.ExecuteAsync(dto, ip, ct);

                // Configurar cookies HttpOnly
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes),
                    Path = "/"
                };

                // Cookie para el access token
                Response.Cookies.Append("accessToken", result.ApiToken, cookieOptions);

                // Cookie para el refresh token (más larga expiración)
                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/"
                };
                Response.Cookies.Append("refreshToken", result.RefreshToken, refreshCookieOptions);

                Response.Headers.CacheControl = "no-store";
                Response.Headers.Pragma = "no-cache";

                // Devuelves la respuesta sin los tokens en el body
                return Ok(new
                {
                    usersId = result.UsersId,
                    usersName = result.UsersName,
                    businessId = result.BusinessId,
                    businessName = result.BusinessName,
                    profilesName = result.ProfilesName,
                    profilesId = result.ProfilesId,
                    usersPhotho = result.UserPhoto,
                    workerId = result.WorkerId,
                    areasId = result.AreasId,
                    usersVisibiliyId = result.UsersVisibiliyId,
                });
            }
            catch (AuthLockedOutException ex)
            {
                if (ex.RetryAfter is TimeSpan ra)
                    Response.Headers["Retry-After"] = ((int)Math.Ceiling(ra.TotalSeconds)).ToString();

                return StatusCode(StatusCodes.Status429TooManyRequests, new
                {
                    status = 0,
                    code = "AUTH_LOCKED_OUT",
                    errors = ex.Errors
                });
            }
            catch (AuthInvalidCredentialsException ex)
            {
                return Unauthorized(new
                {
                    status = 0,
                    code = "AUTH_INVALID_CREDENTIALS",
                    errors = new[] { new { code = "2001", message = ex.Message } }
                });
            }
            catch (Application.Exceptions.ValidationException ex)
            {
                return UnprocessableEntity(new
                {
                    status = 0,
                    code = "VALIDATION_ERROR",
                    errors = ex.Errors
                });
            }
            catch (DatabaseException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    status = 0,
                    code = "DATABASE_ERROR",
                    errors = new[] { new { code = "5000", message = "Error de base de datos." } }
                });
            }
        }
        [AllowAnonymous]
        [HttpGet("session")]
        public IActionResult GetSession()
        {
            // 1) lee la cookie
            if (!Request.Cookies.TryGetValue("accessToken", out var accessToken)
                || string.IsNullOrEmpty(accessToken))
            {
                return Ok(new
                {
                    authenticated = false,
                    expiresAt = (DateTime?)null,
                    remainingSeconds = 0,
                    canRefresh = Request.Cookies.ContainsKey("refreshToken")
                });
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(accessToken);

                // exp en segundos UNIX
                var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                if (!long.TryParse(expClaim, out var expSec))
                {
                    return Ok(new
                    {
                        authenticated = false,
                        expiresAt = (DateTime?)null,
                        remainingSeconds = 0,
                        canRefresh = Request.Cookies.ContainsKey("refreshToken")
                    });
                }

                var exp = DateTimeOffset.FromUnixTimeSeconds(expSec).UtcDateTime;
                var remaining = (int)Math.Max(0, (exp - DateTime.UtcNow).TotalSeconds);

                return Ok(new
                {
                    authenticated = remaining > 0,
                    expiresAt = exp,
                    remainingSeconds = remaining,
                    canRefresh = Request.Cookies.ContainsKey("refreshToken")
                });
            }
            catch
            {
                return Ok(new
                {
                    authenticated = false,
                    expiresAt = (DateTime?)null,
                    remainingSeconds = 0,
                    canRefresh = Request.Cookies.ContainsKey("refreshToken")
                });
            }
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Me()
        {
            var userId = User.FindFirst("uid")?.Value ?? User.Identity?.Name ?? "unknown";
            var business = User.FindFirst("bid")?.Value ?? "-";
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "-";

            return Ok(new { userId, businessId = business, role });
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized(new { status = 0, code = "REFRESH_TOKEN_MISSING" });

                string IssueAccess(long userId, long businessId, string profileName, string sid)
                {
                    return _tokens.Create(userId, businessId, profileName, sid);
                }

                var (access, refresh) = await _refreshTokens.RotateAsync(
                    refreshToken,
                    IssueAccess,
                    ip: HttpContext.GetClientIp(),
                    device: null,
                    newRefreshLifetime: TimeSpan.FromDays(7),
                    ct);

                var accessExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes);

                var accessCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,   
                    Expires = accessExpiresAt,
                    Path = "/"
                };

                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/"
                };

                Response.Cookies.Append("accessToken", access, accessCookieOptions);
                Response.Cookies.Append("refreshToken", refresh, refreshCookieOptions);

                Response.Headers.CacheControl = "no-store";
                Response.Headers.Pragma = "no-cache";

                return Ok(new
                {
                    status = 1,
                    message = "Refrescado.",
                    expiresAt = accessExpiresAt
                });
            }
            catch (Infrastructure.Exceptions.RefreshTokenExpiredException ex)
            {
                Response.Cookies.Delete("accessToken");
                Response.Cookies.Delete("refreshToken");
                return Unauthorized(new { status = 0, code = "REFRESH_TOKEN_EXPIRED", errors = ex.Errors });
            }
            catch (Infrastructure.Exceptions.RefreshTokenInvalidException ex)
            {
                Response.Cookies.Delete("accessToken");
                Response.Cookies.Delete("refreshToken");
                return Unauthorized(new { status = 0, code = "REFRESH_TOKEN_INVALID", errors = ex.Errors });
            }
        }
        
        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            if (Request.Cookies.TryGetValue("accessToken", out var accessToken) &&
                !string.IsNullOrWhiteSpace(accessToken))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(accessToken);

                var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                var exp = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

                if (!string.IsNullOrWhiteSpace(jti) && long.TryParse(exp, out var expUnix))
                {
                    var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
                    var ttl = expiresAt - DateTime.UtcNow;

                    if (ttl > TimeSpan.Zero)
                    {
                        await _blacklist.BlacklistAsync(jti, ttl, ct);

                        var sid = jwt.Claims.FirstOrDefault(c => c.Type == "sid")?.Value;
                        if (!string.IsNullOrWhiteSpace(sid))
                        {
                            var sessionService = HttpContext.RequestServices.GetRequiredService<IUserSessionService>();
                            await sessionService.RevokeSessionAsync(sid, ttl, ct);
                        }
                    }
                }
            }

            if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken) &&
                !string.IsNullOrWhiteSpace(refreshToken))
            {
                await _refreshTokens.RevokeAsync(refreshToken, "Logout manual", ct);
            }

            Response.Cookies.Delete("accessToken", new CookieOptions { Path = "/" });
            Response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/" });

            return Ok(new { ok = true });
        }

        [HttpGet("bootstrap")]
        public async Task<IActionResult> Bootstrap(
        [FromServices] GetAuthBootstrapUseCase useCase,
        [FromQuery] long usersId,
        [FromQuery] long businessId,
        CancellationToken ct)
        {
            var dto = await useCase.Handle(new GetAuthBootstrapRequest(usersId, businessId), ct);
            return Ok(dto);
        }

        [HttpPost("invalidate")]
        public async Task<IActionResult> Invalidate(
        [FromServices] InvalidateAuthCacheUseCase useCase,
        [FromBody] InvalidateAuthCacheCommand cmd,
        CancellationToken ct)
        {
            var ok = await useCase.Handle(cmd, ct);
            return Ok(new { ok });
        }
    }
}

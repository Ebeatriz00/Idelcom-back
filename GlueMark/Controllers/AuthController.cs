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
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SharedKernel;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
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
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            GetAuth getAuth,
            ITokenService tokens,
            IRefreshTokenService refreshTokens,
            ITokenBlacklist blacklist,
            IOptions<JwtOptions> jwtOptions,
            ILogger<AuthController> logger)
        {
            _getAuth = getAuth;
            _tokens = tokens;
            _refreshTokens = refreshTokens;
            _blacklist = blacklist;
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth-login")]
        [HttpPost("login")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] AuthRequestDto dto, CancellationToken ct)
        {
            try
            {
                var ip = HttpContext.GetClientIp();
                var result = await _getAuth.ExecuteAsync(dto, ip, ct);

                // Cookie del access token con 2 minutos extra sobre el JWT para que
                // el browser siga enviándola mientras el frontend detecta la expiración y llama refresh.
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes + 2),
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
        public async Task<IActionResult> GetSession(CancellationToken ct)
        {
            var canRefresh = Request.Cookies.ContainsKey("refreshToken");

            if (!Request.Cookies.TryGetValue("accessToken", out var accessToken)
                || string.IsNullOrEmpty(accessToken))
            {
                return Ok(new { authenticated = false, expiresAt = (DateTime?)null, remainingSeconds = 0, canRefresh });
            }

            try
            {
                // Validación criptográfica de la firma, issuer y audience (sin verificar lifetime
                // para poder reportar el estado aunque el token haya expirado).
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = _jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtOptions.Audience,
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                };

                var handler = new JsonWebTokenHandler();
                var result = await handler.ValidateTokenAsync(accessToken, validationParams);

                if (!result.IsValid)
                    return Ok(new { authenticated = false, expiresAt = (DateTime?)null, remainingSeconds = 0, canRefresh });

                var jwt = (JsonWebToken)result.SecurityToken;

                var exp = jwt.ValidTo.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(jwt.ValidTo, DateTimeKind.Utc)
                    : jwt.ValidTo.ToUniversalTime();

                var remaining = (int)Math.Max(0, (exp - DateTime.UtcNow).TotalSeconds);

                if (remaining <= 0)
                    return Ok(new { authenticated = false, expiresAt = exp, remainingSeconds = 0, canRefresh });

                // Validación server-side: blacklist y sesión revocada.
                var jti = jwt.Id;
                var sid = jwt.Claims.FirstOrDefault(c => c.Type == "sid")?.Value;

                if (!string.IsNullOrEmpty(jti) && await _blacklist.IsBlacklistedAsync(jti, ct))
                    return Ok(new { authenticated = false, expiresAt = exp, remainingSeconds = 0, canRefresh, revoked = true });

                if (!string.IsNullOrEmpty(sid))
                {
                    var sessionService = HttpContext.RequestServices.GetRequiredService<IUserSessionService>();
                    if (await sessionService.IsRevokedAsync(sid, ct))
                        return Ok(new { authenticated = false, expiresAt = exp, remainingSeconds = 0, canRefresh, revoked = true });
                }

                return Ok(new { authenticated = true, expiresAt = exp, remainingSeconds = remaining, canRefresh });
            }
            catch
            {
                return Ok(new { authenticated = false, expiresAt = (DateTime?)null, remainingSeconds = 0, canRefresh });
            }
        }

        [Authorize]
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
        [EnableRateLimiting("auth-refresh")]
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

                var ip = HttpContext.GetClientIp();
                var (access, refresh) = await _refreshTokens.RotateAsync(
                    refreshToken,
                    IssueAccess,
                    ip: ip,
                    device: null,
                    newRefreshLifetime: TimeSpan.FromDays(7),
                    ct);

                _logger.LogInformation("Refresh exitoso — IP: {Ip}", ip);
                var accessExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes);

                var accessCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = accessExpiresAt.AddMinutes(2),
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
                _logger.LogWarning("Refresh token expirado — IP: {Ip}", HttpContext.GetClientIp());
                ClearAuthCookies();
                return Unauthorized(new { status = 0, code = "REFRESH_TOKEN_EXPIRED", errors = ex.Errors });
            }
            catch (Infrastructure.Exceptions.RefreshTokenInvalidException ex)
            {
                _logger.LogWarning("Refresh token inválido (posible reuso) — IP: {Ip}", HttpContext.GetClientIp());
                ClearAuthCookies();
                return Unauthorized(new { status = 0, code = "REFRESH_TOKEN_INVALID", errors = ex.Errors });
            }
        }
        
        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            _logger.LogInformation("Logout — IP: {Ip}", HttpContext.GetClientIp());

            if (Request.Cookies.TryGetValue("accessToken", out var accessToken) &&
                !string.IsNullOrWhiteSpace(accessToken))
            {
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = _jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtOptions.Audience,
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                };

                var handler = new JsonWebTokenHandler();
                var result = await handler.ValidateTokenAsync(accessToken, validationParams);

                if (result.IsValid)
                {
                    var jwt = (JsonWebToken)result.SecurityToken;
                    var ttl = jwt.ValidTo.ToUniversalTime() - DateTime.UtcNow;

                    if (ttl > TimeSpan.Zero)
                    {
                        await _blacklist.BlacklistAsync(jwt.Id, ttl, ct);

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

            ClearAuthCookies();

            return Ok(new { ok = true });
        }

        private void ClearAuthCookies()
        {
            // Path="/" debe coincidir exactamente con el Path usado al setear las cookies.
            // Sin esto el browser ignora el Delete y las cookies quedan atrapadas.
            var deleteOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTime.UnixEpoch
            };
            Response.Cookies.Delete("accessToken", deleteOptions);
            Response.Cookies.Delete("refreshToken", deleteOptions);
        }

        [Authorize]
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

        [Authorize]
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

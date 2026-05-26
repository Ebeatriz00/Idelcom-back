using Core.Interfaces.Abstractions;
using Core.Interfaces.Security;
using Core.Interfaces.Services;
using Core.Options;
using DependencyInjection.Dependency.ServiceExtensions;
using GlueMark.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

// Configuración de caché en memoria distribuida para manejo de estado compartido.
builder.Services.AddDistributedMemoryCache();

// Registro de opciones de configuración fuertemente tipadas desde el archivo de configuración.
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtSetting"));
builder.Services.Configure<LoginAttemptOptions>(builder.Configuration.GetSection("LoginAttempt"));

// Registro de controladores de la Web API con configuración de JSON personalizada.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new SharedKernel.Helpers.DateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new SharedKernel.Helpers.DateTimeNullableConverter());
    });

// Configuración de caché en memoria local para optimización de datos frecuentes.
builder.Services.AddMemoryCache();

// Registro centralizado de los módulos de la aplicación (Clean Architecture).
builder.Services.AddApplicationModules(builder.Configuration);

// Configuración de AutoMapper para el mapeo de objetos entre capas.
builder.Services.AddAutoMapper(
    cfg =>
    {
        // TODO: Si en algún momento se decide usar la versión comercial de AutoMapper, aquí se puede configurar la licencia.
        // cfg.LicenseKey = "";
    },
    AppDomain.CurrentDomain.GetAssemblies()
);

// Configuración de Swagger para la documentación interactiva de la API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CONFIGURACIÓN DE SEGURIDAD (JWT) ---
// Obtención y preparación de la clave simétrica para la firma de tokens.
var key = builder.Configuration.GetValue<string>("JwtSetting:Key");
var keyBytes = Encoding.UTF8.GetBytes(key!);
var signingKey = new SymmetricSecurityKey(keyBytes);

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(config =>
{
    // TODO: En entornos de producción, esta propiedad DEBE ser 'true' para obligar el uso de HTTPS.
    // Se recomienda usar: config.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;

    config.Events = new JwtBearerEvents
    {
        // Intercepta la petición para extraer el token de diferentes fuentes (Header, Cookies o QueryString).
        OnMessageReceived = context =>
        {
            var path = context.HttpContext.Request.Path;

            // Lógica específica para SignalR: permite pasar el token por QueryString en hubs de notificaciones.
            if (path.StartsWithSegments("/api/hubs/notifications"))
            {
                var qsToken = context.Request.Query["access_token"].FirstOrDefault();
                if (!string.IsNullOrEmpty(qsToken))
                {
                    context.Token = qsToken;
                    return Task.CompletedTask;
                }

                // Fallback a Cookie para SignalR.
                if (context.Request.Cookies.TryGetValue("accessToken", out var hubCookieToken) &&
                    !string.IsNullOrWhiteSpace(hubCookieToken))
                {
                    context.Token = hubCookieToken;
                }

                return Task.CompletedTask;
            }

            // Extracción estándar desde el Header Authorization (Bearer).
            var apiAuthHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(apiAuthHeader) &&
                apiAuthHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Token = apiAuthHeader["Bearer ".Length..].Trim();
                return Task.CompletedTask;
            }

            // Extracción desde Cookie para clientes que no usan Headers.
            if (context.Request.Cookies.TryGetValue("accessToken", out var apiCookieToken) &&
                !string.IsNullOrWhiteSpace(apiCookieToken))
            {
                context.Token = apiCookieToken;
            }

            return Task.CompletedTask;
        },

        // Evento crítico: se ejecuta tras la validación criptográfica del token.
        // Aplica lógica de negocio de revocación y contexto de inquilino (Multitenancy).
        OnTokenValidated = async context =>
        {
            var blacklist = context.HttpContext.RequestServices.GetRequiredService<ITokenBlacklist>();
            var sessionService = context.HttpContext.RequestServices.GetRequiredService<IUserSessionService>();

            var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti))
            {
                context.Fail("Token inválido: falta el identificador único (JTI).");
                return;
            }

            // Identificamos la fuente del token para aplicar la validación de revocación correspondiente.
            var source = context.Principal?.FindFirst("source")?.Value;
            if (source == "mobile")
            {
                // Validación para App Móvil: consulta en la tabla de sesiones de dispositivos.
                var appSessionRepo = context.HttpContext.RequestServices.GetRequiredService<IAppDeviceSessionRepository>();
                var isAppSessionRevoked = await appSessionRepo.IsRevokedAsync(jti);
                if (isAppSessionRevoked)
                {
                    context.Fail("Acceso denegado: La sesión del dispositivo móvil ha sido revocada.");
                    return;
                }
            }
            else
            {
                // Validación para ERP (Web): consulta servicios de sesión y listas negras generales.
                var sid = context.Principal?.FindFirst("sid")?.Value;

                if (string.IsNullOrEmpty(sid))
                {
                    context.Fail("Token inválido: falta el identificador de sesión (SID).");
                    return;
                }

                if (await sessionService.IsRevokedAsync(sid, context.HttpContext.RequestAborted))
                {
                    context.Fail("Acceso denegado: La sesión web ha sido revocada.");
                    return;
                }

                if (await blacklist.IsBlacklistedAsync(jti, context.HttpContext.RequestAborted))
                {
                    context.Fail("Acceso denegado: El token se encuentra en la lista negra.");
                    return;
                }
            }

            // Validación de claims de identidad y pertenencia a negocio.
            var sub = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var bid = context.Principal?.FindFirst("bid")?.Value;

            if (string.IsNullOrEmpty(sub) || string.IsNullOrEmpty(bid))
            {
                context.Fail("Token inválido: faltan claims requeridos (Sub/Bid).");
                return;
            }

            // Almacenamos el Business ID en el contexto para acceso rápido en controladores.
            context.HttpContext.Items["bid"] = bid;
        },

        // Manejo de fallos de autorización (cuando el usuario no está autenticado).
        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Intento de acceso no autorizado en: {path}", context.Request.Path);

            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var result = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                status = 0,
                message = "Usuario No Autorizado o sesión expirada."
            });

            return context.Response.WriteAsync(result);
        },

        // Registro de errores internos durante la validación del token.
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(context.Exception, "Error interno de autenticación: {message}", context.Exception.Message);
            return Task.CompletedTask;
        }
    };

    // Parámetros de validación técnica del JWT.
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero, // Estricto con la expiración.
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        NameClaimType = JwtRegisteredClaimNames.Sub,
        RoleClaimType = ClaimTypes.Role,
        ValidIssuer = builder.Configuration["JwtSetting:Issuer"],
        ValidAudience = builder.Configuration["JwtSetting:Audience"],
        RequireExpirationTime = true
    };
});

// --- CONFIGURACIÓN DE AUTORIZACIÓN (POLÍTICAS) ---
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("TenantBound", p => p.RequireClaim("bid"))
    .AddPolicy("RequireAppAccess", p => p.RequireClaim("source", "mobile")); // Exige que el token provenga de la App Móvil.

// Configuración de políticas de CORS para permitir orígenes de confianza.
var allowedOriginsSection = builder.Configuration.GetSection("AllowedOrigins");
var allowedOrigins = allowedOriginsSection
    .GetChildren()
    .Select(x => x.Value)
    .Where(x => !string.IsNullOrWhiteSpace(x))
    .Select(x => x!.Trim().TrimEnd('/'))
    .ToArray();

if (allowedOrigins.Length == 0 && !string.IsNullOrWhiteSpace(allowedOriginsSection.Value))
{
    allowedOrigins = allowedOriginsSection.Value
        .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(x => x.TrimEnd('/'))
        .ToArray();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetPreflightMaxAge(TimeSpan.FromSeconds(86400));
    });
});

// Soporte para SignalR (comunicación bidireccional).
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
});

// Configuración adicional de caché para el pipeline con límites de tamaño.
builder.Services.AddMemoryCache(opt =>
{
    opt.SizeLimit = 100_000;
    opt.CompactionPercentage = 0.2;
});

var app = builder.Build();

// Habilitación de Swagger en entornos de desarrollo o producción controlada.
var enableSwaggerInProd = builder.Configuration.GetValue<bool>("EnableSwaggerInProduction");

if (app.Environment.IsDevelopment() ||
    (app.Environment.IsProduction() && enableSwaggerInProd) ||
    (app.Environment.IsStaging() && enableSwaggerInProd))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "Idelcom API V1");
    });
}

// Middleware global para manejo de excepciones.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Habilitación de WebSockets para SignalR.
app.UseWebSockets();

app.UseRouting();

// Aplicación de CORS antes de Auth.
app.UseCors("AllowSpecificOrigin");

// Middlewares de seguridad: Autenticación y luego Autorización.
app.UseAuthentication();
app.UseAuthorization();

// Mapeo de controladores.
app.MapControllers();

// Endpoint del Hub de notificaciones.
app.MapHub<Infrastructure.Hubs.NotificationsHub>("/api/hubs/notifications")
  .RequireCors("AllowSpecificOrigin");

app.Run();

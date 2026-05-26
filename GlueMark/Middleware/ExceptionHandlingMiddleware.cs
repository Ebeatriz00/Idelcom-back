using Application.Exceptions;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace GlueMark.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Application.Exceptions.BaseException ex)
            {
                _logger.LogWarning(ex,
                    "Excepción de dominio [{Code}] en {Method} {Path}",
                    ex.ErrorCode, context.Request.Method, context.Request.Path);

                await HandleBaseExceptionAsync(context, ex, "application");
            }
            catch (Infrastructure.Exceptions.BaseException ex)
            {
                _logger.LogError(ex,
                    "Excepción de infraestructura [{Code}] en {Method} {Path}",
                    ex.ErrorCode, context.Request.Method, context.Request.Path);

                await HandleBaseExceptionAsync(context, ex, "infrastructure");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Excepción no controlada en {Method} {Path} — {ExceptionType}: {Message}",
                    context.Request.Method, context.Request.Path,
                    ex.GetType().Name, ex.Message);

                await HandleUnexpectedExceptionAsync(context, ex);
            }
        }

        private static Task HandleBaseExceptionAsync(HttpContext context, dynamic ex, string source)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex.StatusCode;

            var response = new Dictionary<string, object>
            {
                ["status"] = 0,
                ["code"] = ex.ErrorCode,
                ["errors"] = ex.Errors
            };

            if (!string.IsNullOrWhiteSpace(ex.Details))
                response["details"] = ex.Details;

            return context.Response.WriteAsJsonAsync(response);
        }

        private static Task HandleUnexpectedExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(new
            {
                status = 0,
                code = "ERR_INTERNAL",
                message = "Ha ocurrido un error inesperado.",
                details = ex.Message
            });

            return context.Response.WriteAsync(result);
        }
    }
}

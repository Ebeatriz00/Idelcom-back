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

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Application.Exceptions.BaseException ex)
            {
                await HandleBaseExceptionAsync(context, ex, "application");
            }
            catch (Infrastructure.Exceptions.BaseException ex)
            {
                await HandleBaseExceptionAsync(context, ex, "infrastructure");
            }
            catch (Exception ex)
            {
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
                response["details"] = ex.Details; // Solo se agrega si viene con contenido

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

using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using Application.Exceptions;
using FluentValidation;

namespace API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, title, errors) = MapException(exception);

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, "Unhandled exception occurred.");
            else
                _logger.LogWarning(exception, "Request failed with {StatusCode}: {Message}", statusCode, exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var payload = new
            {
                status = (int)statusCode,
                title,
                errors
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }

        private static (HttpStatusCode StatusCode, string Title, object? Errors) MapException(Exception exception)
        {
            return exception switch
            {
                ValidationException validationEx => (
                    HttpStatusCode.BadRequest,
                    "One or more validation errors occurred.",
                    validationEx.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })),

                DocumentNotFoundException notFoundEx => (
                    HttpStatusCode.NotFound, notFoundEx.Message, null),

                ForbiddenAccessException forbiddenEx => (
                    HttpStatusCode.Forbidden, forbiddenEx.Message, null),

                InvalidCredentialsException invalidCredsEx => (
                    HttpStatusCode.Unauthorized, invalidCredsEx.Message, null),

                InvalidTokenException invalidTokenEx => (
                    HttpStatusCode.Unauthorized, invalidTokenEx.Message, null),

                EmailAlreadyExistsException emailEx => (
                    HttpStatusCode.Conflict, emailEx.Message, null),

                AuthenticationException authEx => (
                    HttpStatusCode.Unauthorized, authEx.Message, null),

                ArgumentException argEx => (
                    HttpStatusCode.BadRequest, argEx.Message, null),

                InvalidOperationException invalidOpEx => (
                    HttpStatusCode.BadRequest, invalidOpEx.Message, null),

                UserNotFoundException userNotFoundEx => (
                    HttpStatusCode.NotFound, userNotFoundEx.Message, null),

                _ => (
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred. Please try again later.",
                    null)
            };
        }
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
using ChallengeApi.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace ChallengeApi.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, "Excepción no controlada: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = ex switch
            {
                UserAlreadyExistsException => (HttpStatusCode.Conflict, ex.Message),
                InvalidCredentialsException => (HttpStatusCode.Unauthorized, ex.Message),
                UserNotFoundException => (HttpStatusCode.NotFound, ex.Message),
                DomainException => (HttpStatusCode.BadRequest, ex.Message),
                HttpRequestException => (HttpStatusCode.BadGateway,
                    "Error al comunicarse con el servicio externo de horóscopo."),
                _ => (HttpStatusCode.InternalServerError,
                    "Ocurrió un error inesperado. Por favor intente nuevamente.")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                statusCode = (int)statusCode,
                message
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}

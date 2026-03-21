using System.Net;
using System.Text.Json;

namespace Api.Middleware
{
    /// <summary>
    /// Middleware для глобальної обробки помилок
    /// Перехоплює всі необроблені винятки і повертає JSON-відповідь
    /// </summary>
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
            catch(Exception ex)
            {
                _logger.LogError(ex, "Непередбачена помилка: {Message}", ex.Message);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    status = 500,
                    message = "Внутрішня помилка серверу",
                    detail = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                        ? ex.Message
                        : null

                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);

            }
        }
    }
}

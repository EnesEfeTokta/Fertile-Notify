using System.Net;
using System.Text.Json;
using FertileNotify.Domain.Exceptions;

namespace FertileNotify.API.Middlewares
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            string message = "A server error occurred. Please try again.";

            switch (ex)
            {
                case NotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = ex.Message;
                    break;

                case BusinessRuleException:
                    status = HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;

                case ArgumentException:
                    status = HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;

                default:
                    _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                    break;
            }

            var response = new { error = message };
            var jsonResponse = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}

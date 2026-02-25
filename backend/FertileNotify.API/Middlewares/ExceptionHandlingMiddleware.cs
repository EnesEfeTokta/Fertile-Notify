using System.Net;
using System.Text.Json;
using FertileNotify.API.Models.Responses;
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
            try { await _next(context); }
            catch (Exception ex) { await HandleExceptionAsync(context, ex); }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "Exception caught in middleware.");

            int status = (int)HttpStatusCode.InternalServerError;
            string errorCode = "INTERNAL_SERVER_ERROR";
            string message = "An unexpected error occurred.";
            object? details = null;

            if (ex is DomainException domainException)
            {
                status = domainException.StatusCode;
                errorCode = domainException.ErrorCode;
                message = domainException.Message;
                details = domainException.Details;
            }
            else
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
            }

            var errorResponse = new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Data = new
                {
                    Code = errorCode,
                    Details = details,
                    TraceId = context.TraceIdentifier
                },
                Errors = new List<string> { message }
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = status;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
        }
    }
}

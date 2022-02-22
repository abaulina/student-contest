using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace StudentContest.Api.ExceptionMiddleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
                _logger = logger;
                _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                    await _next(context);
            }
            catch (Exception ex)
            {
                    _logger.LogError($"{ex}: {ex.Message} \r\n {ex.StackTrace}");
                    await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            
            response.StatusCode = exception switch
            {
                ArgumentException _ => (int)HttpStatusCode.BadRequest,
                DbUpdateException _ => (int)HttpStatusCode.InternalServerError,
                InvalidCredentialException => (int)HttpStatusCode.Unauthorized,
                SaltParseException => (int)HttpStatusCode.Unauthorized,
                SecurityTokenException => (int)HttpStatusCode.Unauthorized,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.BadRequest
            };

            var result = JsonSerializer.Serialize(new { error = response.StatusCode, exception.Message });
            return response.WriteAsync(result);
        }
    }
}

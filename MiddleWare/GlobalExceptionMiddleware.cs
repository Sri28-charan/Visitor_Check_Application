using System.Net;
using System.Text.Json;

namespace Visitor_Check_Application.MiddleWare
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
                _logger.LogError(ex, "Unhandled exception occured");

             
                context.Response.ContentType = "application/json";

                context.Response.StatusCode = ex switch
                {
                    ArgumentException=>StatusCodes.Status400BadRequest,
					KeyNotFoundException => StatusCodes.Status404NotFound,
                    UnauthorizedAccessException=>StatusCodes.Status401Unauthorized,
                    _=>StatusCodes.Status500InternalServerError,

				};

                var response = new
                {
                    statusCode = context.Response.StatusCode,
                    message = ex.Message,
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}

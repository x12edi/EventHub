using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EventHub.Web.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Capture request details
            var request = context.Request;
            var logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | {request.Method} | {request.Path} | User: {(context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "Anonymous")}";
            Console.WriteLine(logMessage);
            // Log to file
            try
            {
                var logPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logs", "requests.log");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                await File.AppendAllTextAsync(logPath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Log to console if file write fails
                Console.WriteLine($"Logging error: {ex.Message}");
            }

            // Call the next middleware
            await _next(context);
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
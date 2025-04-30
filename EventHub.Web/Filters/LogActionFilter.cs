using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace EventHub.Web.Filters
{
    public class LogActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["StartTime"] = Stopwatch.GetTimestamp();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var startTime = (long)context.HttpContext.Items["StartTime"];
            var elapsed = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
            Console.WriteLine($"Action {context.ActionDescriptor.DisplayName} took {elapsed}ms");
        }
    }
}
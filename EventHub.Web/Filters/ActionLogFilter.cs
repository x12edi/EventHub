using Microsoft.AspNetCore.Mvc.Filters;

namespace EventHub.Web.Filters
{
    public class ActionLogFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller.GetType().Name;
            var action = context.ActionDescriptor.DisplayName;
            var user = context.HttpContext.User.Identity.Name ?? "Anonymous";
            Console.WriteLine($"[LOG] User {user} is executing {controller}.{action} at {DateTime.UtcNow}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var controller = context.Controller.GetType().Name;
            var action = context.ActionDescriptor.DisplayName;
            var user = context.HttpContext.User.Identity.Name ?? "Anonymous";
            var result = context.Result?.GetType().Name ?? "NoResult";
            Console.WriteLine($"[LOG] User {user} completed {controller}.{action} with result {result} at {DateTime.UtcNow}");
        }
    }
}
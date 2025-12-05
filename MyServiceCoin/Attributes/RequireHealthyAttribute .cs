using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyServiceCoin.Attributes
{
    public class RequireHealthyAttribute : ActionFilterAttribute
    {
        private readonly string _message;

        public RequireHealthyAttribute(string message = "Service is currently unavailable.")
        {
            _message = message;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var healthCheckService =
                context.HttpContext.RequestServices.GetRequiredService<HealthCheckService>();

            var health = await healthCheckService.CheckHealthAsync();

            if (health.Status != HealthStatus.Healthy)
            {
                // ✅ گرفتن نام کنترلر جاری به صورت داینامیک
                var controllerName =
                    context.RouteData.Values["controller"]?.ToString() ?? "UnknownController";

                context.Result = new ObjectResult(new
                {
                    status = "Unhealthy",
                    message = $"{controllerName} → {_message}"
                })
                {
                    StatusCode = StatusCodes.Status503ServiceUnavailable
                };

                return; // ❌ مانع اجرای اکشن
            }

            await next(); // ✅ اگر Healthy بود اجرا می‌شود
        }
    }
}

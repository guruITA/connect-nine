using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Connect.Presentation.Health.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // MapGet instead of MapHealthChecks, so every other method gets 405 and the answer stays our own (#311).
        endpoints.MapGet("/api/health", async (HttpContext context, HealthCheckService healthChecks) =>
            {
                HealthReport report = await healthChecks.CheckHealthAsync(context.RequestAborted);
                await HealthAnswerWriter.WriteAsync(context, report);
            })
            .AllowAnonymous();

        return endpoints;
    }
}

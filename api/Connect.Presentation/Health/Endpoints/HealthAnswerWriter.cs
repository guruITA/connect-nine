using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Connect.Presentation.Health.Endpoints;

// Turns a health report into the short answer that deploy jobs and monitors read (#311).
// The address is open to everyone, so the answer only names the failing checks: never an
// exception, a description, a duration, a version or a connection string.
public static class HealthAnswerWriter
{
    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        context.Response.Headers.CacheControl = "no-store";

        if (report.Status != HealthStatus.Unhealthy)
        {
            return Results.Json(new { status = "healthy" }).ExecuteAsync(context);
        }

        string[] failing = report.Entries
            .Where(entry => entry.Value.Status == HealthStatus.Unhealthy)
            .Select(entry => entry.Key)
            .ToArray();

        return Results.Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Connect is not healthy",
                extensions: new Dictionary<string, object?> { ["failing"] = failing })
            .ExecuteAsync(context);
    }
}

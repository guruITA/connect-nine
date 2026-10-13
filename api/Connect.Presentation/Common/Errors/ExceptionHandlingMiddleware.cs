using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Connect.Presentation.Common.Errors;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing an HTTP request.");

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();

            var (statusCode, title) = ContainsDatabaseException(exception)
                ? (StatusCodes.Status503ServiceUnavailable, "Connect is not reachable right now")
                : (StatusCodes.Status500InternalServerError, "An unexpected error occurred.");

            await TypedResults.Problem(statusCode: statusCode, title: title)
                .ExecuteAsync(context);
        }
    }

    private static bool ContainsDatabaseException(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is DbException)
            {
                return true;
            }
        }

        return false;
    }
}

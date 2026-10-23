using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Connect.Presentation.Screen;

// The server also serves the built screen from wwwroot, so the test site needs one address and no CORS (#267).
public static class ScreenPipeline
{
    // Before the endpoints and any authentication, so a file of the screen never asks for a login.
    public static IApplicationBuilder UseScreen(this IApplicationBuilder app)
    {
        // No MapFallback or MapFallbackToFile: routing picks a fallback over the 405 of a known address with
        // another method (POST /api/health, #311). This runs only when no endpoint answered and the answer is 404.
        app.UseStatusCodePages(WriteNotFoundAsync);

        return app.UseDefaultFiles().UseStaticFiles();
    }

    private static async Task WriteNotFoundAsync(StatusCodeContext statusCodeContext)
    {
        HttpContext context = statusCodeContext.HttpContext;
        if (context.Response.StatusCode != StatusCodes.Status404NotFound)
        {
            return;
        }

        if (context.Request.Path.StartsWithSegments("/api"))
        {
            await TypedResults.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "This address does not exist")
                .ExecuteAsync(context);
            return;
        }

        // Every other address is a page of the screen, which picks it from the address itself (BrowserRouter).
        // A missing file such as /logo.svg keeps its 404.
        bool isRead = HttpMethods.IsGet(context.Request.Method) || HttpMethods.IsHead(context.Request.Method);
        if (!isRead || Path.HasExtension(context.Request.Path.Value))
        {
            return;
        }

        IFileInfo screen = context.RequestServices
            .GetRequiredService<IWebHostEnvironment>()
            .WebRootFileProvider
            .GetFileInfo("index.html");
        if (!screen.Exists)
        {
            return;
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/html; charset=utf-8";
        if (HttpMethods.IsGet(context.Request.Method))
        {
            await context.Response.SendFileAsync(screen, context.RequestAborted);
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Connect.Presentation.StaticFiles;

public static class ScreenCacheRules
{
    public static StaticFileOptions Options { get; } = new()
    {
        OnPrepareResponse = context =>
        {
            string path = context.Context.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/assets/", StringComparison.OrdinalIgnoreCase))
            {
                context.Context.Response.Headers.CacheControl = "public, max-age=31536000, immutable";
            }
            else if (string.Equals(path, "/index.html", StringComparison.OrdinalIgnoreCase))
            {
                context.Context.Response.Headers.CacheControl = "no-cache";
            }
        }
    };
}

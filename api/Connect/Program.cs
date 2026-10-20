using Connect.Application;
using Connect.Infrastructure;
using Connect.Presentation;
using Connect.Presentation.Accounts.Endpoints;
using Connect.Presentation.Common.Errors;
using Connect.Presentation.Health.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// One JSON object per line, with the scopes of the request (RequestId, TraceId) and a UTC time (#312).
builder.Logging
    .ClearProviders()
    .AddJsonConsole(options =>
    {
        options.IncludeScopes = true;
        options.UseUtcTimestamp = true;
        options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    });

builder.Services
    .AddProblemDetails()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// First, so the request line also holds the status of an error answer (#312).
app.UseHttpLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthEndpoints();
app.MapAccountEndpoints();

app.Run();

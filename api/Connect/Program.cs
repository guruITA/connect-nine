using Connect.Application;
using Connect.Infrastructure;
using Connect.Presentation;
using Connect.Presentation.Health.Endpoints;
using Connect.Presentation.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddProblemDetails()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthEndpoints();
app.UseStaticFiles(ScreenCacheRules.Options);
app.MapFallbackToFile("index.html", ScreenCacheRules.Options);

app.Run();

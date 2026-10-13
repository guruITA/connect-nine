using Connect.Application;
using Connect.Infrastructure;
using Connect.Presentation;
using Connect.Presentation.Accounts.Endpoints;
using Connect.Presentation.Common.Errors;
using Connect.Presentation.Health.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddProblemDetails()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthEndpoints();
app.MapAccountEndpoints();

app.Run();

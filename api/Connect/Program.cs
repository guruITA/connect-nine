using Connect.Application;
using Connect.Infrastructure;
using Connect.Presentation;
using Connect.Presentation.Health.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddProblemDetails()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

var app = builder.Build();

app.UseExceptionHandler();
app.MapHealthEndpoints();

app.Run();

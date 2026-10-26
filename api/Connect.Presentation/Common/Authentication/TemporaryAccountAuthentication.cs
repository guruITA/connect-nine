using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Connect.Presentation.Common.Authentication;

// Temporary until log in (#15) adds the session cookie. A request says which account it is with
// the header X-Temporary-Account-Id, and only in Development with TemporaryAccount:Enabled set.
// Everywhere else nobody is logged in, so an address behind the guard answers 401.
public static class TemporaryAccountAuthentication
{
    public const string Scheme = "TemporaryAccount";
    public const string Header = "X-Temporary-Account-Id";

    public static IServiceCollection AddTemporaryAccountAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAuthentication(Scheme)
            .AddScheme<TemporaryAccountOptions, TemporaryAccountHandler>(
                Scheme,
                options => options.Enabled = configuration.GetValue<bool>("TemporaryAccount:Enabled"));

        return services;
    }
}

public sealed class TemporaryAccountOptions : AuthenticationSchemeOptions
{
    public bool Enabled { get; set; }
}

public sealed class TemporaryAccountHandler(
    IOptionsMonitor<TemporaryAccountOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IHostEnvironment environment)
    : AuthenticationHandler<TemporaryAccountOptions>(options, logger, encoder)
{
    private readonly IHostEnvironment _environment = environment;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Options.Enabled
            || !_environment.IsDevelopment()
            || !Guid.TryParse(Request.Headers[TemporaryAccountAuthentication.Header], out var accountId))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, accountId.ToString())],
            TemporaryAccountAuthentication.Scheme);

        return Task.FromResult(AuthenticateResult.Success(
            new AuthenticationTicket(new ClaimsPrincipal(identity), TemporaryAccountAuthentication.Scheme)));
    }
}

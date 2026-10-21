using Connect.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Connect.Infrastructure.Persistence;

// Adds one made up test member, so a later check can log in as a known account (#276, #277).
// Only ever adds: an existing test member is left exactly as it is, even when the password
// setting changed since, and the password itself is never stored anywhere but the hash.
public sealed class TestDataSeeder(
    UserManager<Account> userManager,
    IConfiguration configuration,
    ILogger<TestDataSeeder> logger)
{
    private const string TestMemberEmail = "test-member@example.org";

    private readonly UserManager<Account> _userManager = userManager;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<TestDataSeeder> _logger = logger;

    public async Task SeedAsync()
    {
        string? password = _configuration["Seeding:TestMemberPassword"];
        if (string.IsNullOrEmpty(password))
        {
            _logger.LogWarning("Test data: no password for the test member, skipped");
            return;
        }

        Account? existing = await _userManager.FindByEmailAsync(TestMemberEmail);
        if (existing is not null)
        {
            return;
        }

        Account account = new()
        {
            Id = Guid.NewGuid(),
            UserName = TestMemberEmail,
            Email = TestMemberEmail,
            EmailConfirmed = true,
        };

        IdentityResult result = await _userManager.CreateAsync(account, password);
        if (!result.Succeeded)
        {
            string codes = string.Join(", ", result.Errors.Select(error => error.Code));
            _logger.LogWarning("Test data: could not add the test member, {Codes}", codes);
        }
    }
}

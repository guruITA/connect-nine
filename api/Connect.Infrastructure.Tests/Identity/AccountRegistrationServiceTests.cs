using Connect.Application.Common.Results;
using Connect.Domain.Users;
using Connect.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using NSubstitute;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests.Identity;

// Application types must be available through the project's global usings.
// Assumes Result<T> has IsSuccess, Value and Error, CreateAccountResult has
// Id and Email, and Error implements value equality.
[TestFixture]
public sealed class AccountRegistrationServiceTests
{
    private const string Email = "person@example.com";
    private const string Password = "ValidPassword123!";
    private const string DuplicateMessage = "This email address already has an account";

    private UserManager<Account> _userManager;
    private ILogger<AccountRegistrationService> _logger;
    private AccountRegistrationService sut;

    [SetUp]
    public void SetUp()
    {
        _userManager = Substitute.For<UserManager<Account>>(
            Substitute.For<IUserStore<Account>>(),
            Options.Create(new IdentityOptions()),
            Substitute.For<IPasswordHasher<Account>>(),
            Array.Empty<IUserValidator<Account>>(),
            Array.Empty<IPasswordValidator<Account>>(),
            Substitute.For<ILookupNormalizer>(),
            new IdentityErrorDescriber(),
            Substitute.For<IServiceProvider>(),
            Substitute.For<ILogger<UserManager<Account>>>());
        _logger = Substitute.For<ILogger<AccountRegistrationService>>();
        sut = new AccountRegistrationService(_userManager, _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _userManager.Dispose();
    }

    [TestCase("person@example.com")]
    [TestCase("  person@example.com  ")]
    [TestCase("\tperson@example.com\r\n")]
    public async Task RegisterAsync_WhenIdentitySucceeds_ReturnsCreatedAccount(string inputEmail)
    {
        Account? createdAccount = null;
        _userManager.CreateAsync(Arg.Any<Account>(), Password).Returns(call =>
        {
            createdAccount = call.Arg<Account>();
            return Task.FromResult(IdentityResult.Success);
        });

        var result = await sut.RegisterAsync(inputEmail, Password, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(createdAccount, Is.Not.Null);
            Assert.That(createdAccount?.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(createdAccount?.Email, Is.EqualTo(Email));
            Assert.That(createdAccount?.UserName, Is.EqualTo(Email));
            Assert.That(result.Value?.Id, Is.EqualTo(createdAccount?.Id));
            Assert.That(result.Value?.Email, Is.EqualTo(Email));
        }

        await _userManager!.Received(1).CreateAsync(Arg.Any<Account>(), Password);
        AssertNoWarnings();
    }

    [Test]
    public async Task RegisterAsync_WhenCalledTwice_CreatesDistinctAccountIds()
    {
        ReturnsIdentityResult(IdentityResult.Success);

        var first = await sut.RegisterAsync(Email, Password, CancellationToken.None);
        var second = await sut.RegisterAsync("other@example.com", Password, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first.IsSuccess, Is.True);
            Assert.That(second.IsSuccess, Is.True);
        }

        Assert.That(first.Value?.Id, Is.Not.EqualTo(second.Value?.Id));
    }

    [Test]
    public async Task RegisterAsync_WhenCancellationAlreadyRequested_ThrowsWithoutCallingIdentity()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        OperationCanceledException? exception = Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await sut!.RegisterAsync(Email, Password, cancellation.Token));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.CancellationToken, Is.EqualTo(cancellation.Token));
        await _userManager.DidNotReceive().CreateAsync(Arg.Any<Account>(), Arg.Any<string>());
        AssertNoWarnings();
    }

    [TestCase("DuplicateEmail")]
    [TestCase("DuplicateUserName")]
    public async Task RegisterAsync_WhenIdentityReportsDuplicate_ReturnsConflict(string code)
    {
        ReturnsIdentityResult(IdentityResult.Failed(new IdentityError { Code = code }));

        var result = await sut!.RegisterAsync(Email, Password, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(Error.Conflict(DuplicateMessage)));
            AssertNoWarnings();
        }
    }

    [Test]
    public async Task RegisterAsync_WhenDuplicateAndOtherErrorsOccur_PrioritizesConflict()
    {
        ReturnsIdentityResult(IdentityResult.Failed(
            new IdentityError { Code = "PasswordTooShort" },
            new IdentityError { Code = "DuplicateEmail" }));

        var result = await sut.RegisterAsync(Email, Password, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(Error.Conflict(DuplicateMessage)));
            AssertNoWarnings();
        }
    }

    [Test]
    public async Task RegisterAsync_WhenIdentityRejectsRegistration_ReturnsFailureAndLogsCodes()
    {
        ReturnsIdentityResult(IdentityResult.Failed(
            new IdentityError { Code = "PasswordTooShort" },
            new IdentityError { Code = "PasswordRequiresDigit" }));

        var result = await sut.RegisterAsync(Email, Password, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(Error.Failure));
        }

        // LogWarning is an extension method; inspect the underlying Log call.
        var warnings = GetWarningArguments();
        Assert.That(warnings, Has.Length.EqualTo(1));
        var state = (IEnumerable<KeyValuePair<string, object?>>)warnings[0][2]!;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                state.Single(item => item.Key == "IdentityErrorCodes").Value,
                Is.EqualTo("PasswordTooShort, PasswordRequiresDigit"));
            Assert.That(
                state.Single(item => item.Key == "{OriginalFormat}").Value,
                Is.EqualTo("Account registration was rejected by Identity. Error codes: {IdentityErrorCodes}"));
            Assert.That(warnings[0][3], Is.Null);
        }
    }

    private static IEnumerable<TestCaseData> AccountUniqueViolations()
    {
        foreach (var constraint in new[]
        {
            AccountIdentityDatabaseNames.EmailUniqueIndex,
            AccountIdentityDatabaseNames.UserNameUniqueIndex
        })
        {
            yield return new TestCaseData(constraint, false);
            yield return new TestCaseData(constraint, true);
        }
    }

    [TestCaseSource(nameof(AccountUniqueViolations))]
    public async Task RegisterAsync_WhenAccountUniqueConstraintIsViolated_ReturnsConflict(
        string constraintName,
        bool wrapPostgresException)
    {
        Exception innerException = CreatePostgresException(PostgresErrorCodes.UniqueViolation, constraintName);
        if (wrapPostgresException)
        {
            innerException = new InvalidOperationException("Intermediate wrapper", innerException);
        }

        ReturnsException(new DbUpdateException("Insert failed", innerException));

        var result = await sut.RegisterAsync(Email, Password, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(Error.Conflict(DuplicateMessage)));
            AssertNoWarnings();
        }
    }

    [Test]
    public void RegisterAsync_WhenDifferentUniqueConstraintIsViolated_PropagatesException()
    {
        AssertDatabaseExceptionPropagates(new DbUpdateException(
            "Insert failed",
            CreatePostgresException(PostgresErrorCodes.UniqueViolation, "unrelated_unique_index")));
    }

    [Test]
    public void RegisterAsync_WhenAccountConstraintHasDifferentSqlState_PropagatesException()
    {
        AssertDatabaseExceptionPropagates(new DbUpdateException(
            "Insert failed",
            CreatePostgresException(PostgresErrorCodes.NotNullViolation, AccountIdentityDatabaseNames.EmailUniqueIndex)));
    }

    [Test]
    public void RegisterAsync_WhenDatabaseExceptionHasNoInnerException_PropagatesException()
    {
        AssertDatabaseExceptionPropagates(new DbUpdateException("Insert failed"));
    }

    [Test]
    public void RegisterAsync_WhenDatabaseExceptionHasNonPostgresInnerException_PropagatesException()
    {
        AssertDatabaseExceptionPropagates(new DbUpdateException(
            "Insert failed", new InvalidOperationException("Database unavailable")));
    }

    [Test]
    public void RegisterAsync_WhenUnexpectedExceptionOccurs_PropagatesException()
    {
        var expected = new InvalidOperationException("Unexpected failure");
        ReturnsException(expected);

        InvalidOperationException? actual = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await sut!.RegisterAsync(Email, Password, CancellationToken.None));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual, Is.SameAs(expected));
            AssertNoWarnings();
        }
    }

    private void ReturnsIdentityResult(IdentityResult result)
    {
        _userManager!.CreateAsync(Arg.Any<Account>(), Password).Returns(Task.FromResult(result));
    }

    private void ReturnsException(Exception exception)
    {
        _userManager!.CreateAsync(Arg.Any<Account>(), Password)
            .Returns(_ => Task.FromException<IdentityResult>(exception));
    }

    private void AssertDatabaseExceptionPropagates(DbUpdateException expected)
    {
        ReturnsException(expected);

        DbUpdateException? actual = Assert.ThrowsAsync<DbUpdateException>(async () =>
            await sut!.RegisterAsync(Email, Password, CancellationToken.None));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual, Is.SameAs(expected));
            AssertNoWarnings();
        }
    }

    private static PostgresException CreatePostgresException(string sqlState, string constraintName)
    {
        return new PostgresException(
            messageText: "Database constraint violation",
            severity: "ERROR",
            invariantSeverity: "ERROR",
            sqlState: sqlState,
            constraintName: constraintName);
    }

    private object?[][] GetWarningArguments()
    {
        return _logger!.ReceivedCalls()
            .Where(call => call.GetMethodInfo().Name == nameof(ILogger.Log))
            .Select(call => call.GetArguments())
            .Where(arguments => Equals(arguments[0], LogLevel.Warning))
            .ToArray();
    }

    private void AssertNoWarnings()
    {
        Assert.That(GetWarningArguments(), Is.Empty);
    }
}

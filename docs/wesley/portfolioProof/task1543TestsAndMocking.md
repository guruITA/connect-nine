# Proof: task 3.12 The code conventions for tests and NSubstitute (#1543)

Merge request !77, branch `docs/1543-tests-and-mocking`. Issue: 03 Create an account (#14). Files: `docs/team/codeConventions.md`, part Tests and mocking, `docs/team/onboarding.md`, `docs/architecture/configuration.md`, `docs/testing/testingStrategy.md`, `api/Connect.Presentation.Tests/Accounts/Validators/CreateAccountRequestValidatorTests.cs`, in `api/Connect.Infrastructure.Tests` the project file, `DependencyInjectionTests.cs` and `Persistence/ConnectDbContextDatabaseTests.cs`, and the project files, `CreateAccountHandlerTests.cs` and `AccountEndpointTests.cs` of `api/Connect.Application.Tests` and `api/Connect.Tests`.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk.** A database test runs every migration of its branch on its database and leaves its rows behind. On a laptop `DefaultConnection` is the database the server runs on, so a test that reads it moves that database onto the migrations of whatever branch was tested last. My own profile, tag, visitor and test data tests did that through `ConnectionStrings__DefaultConnection`. Rules 1 and 2 of the new part give the tests a database of their own.
2. **Measured, not assumed.** I ran !50 at 0e38e40f against PostgreSQL 16 with five settings, each in a home folder of its own so no secret of mine was read:
   - only `DefaultConnection`, as the pipeline had it: 70 of 76, the six tests of `AccountDatabaseTests` red with "ConnectionStrings:TestsConnection is not configured"
   - both names: 76 of 76, and the infrastructure tests six runs in a row green, on new and on used databases
   - only `TestsConnection` as a user secret: three red, the three tests that read `DefaultConnection`
   - only `TestsConnection` as an environment variable: the same three red
   - nothing set: nine red
   A database name that did not exist yet was created by the first run, so rule 2 needs no extra step.
3. **My decision of 20 October 2026.** Tests read only `TestsConnection`, because `TestConfiguration` gets it from the user secrets in the code. On 14 October 2026 I chose `DefaultConnection` as the one name for the server (task 1.7, #8). That stays: the tests now get a name of their own next to it.
4. **The example is a real test.** The example of rule 4 is `CreateAccountHandlerTests` as it is in this merge request, word for word. It passes, and it turned red when I let the handler pass `CancellationToken.None` instead of the token it got, for one run.
5. **A wrong substitute stops the build.** With a substitute, a mistake can give a green test that proves nothing. Without the analyzer I measured it: `.Received(1)` on a member that is not virtual passed although nothing called it, and the real code ran once. With NSubstitute.Analyzers.CSharp 1.0.17 in `Connect.Infrastructure.Tests`, a substitute of a class member that is not virtual gave `error NS1000: Member Count can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.` and the build failed, because every project has TreatWarningsAsErrors. NSubstitute warns about the same thing itself: "NSubstitute can only work with virtual members of the class that are overridable in the test assembly, so any non-virtual code in the class will actually execute!" (NSubstitute, n.d.).
6. **Why the database is never replaced.** Microsoft's guide on testing with EF Core says "properly mocking DbSet query functionality is not possible, since queries are expressed via LINQ operators, which are static extension method calls over IQueryable." (Microsoft, 2021). A test on a substitute of the database can pass where PostgreSQL fails, so rule 7 sends those tests to backend rule 16: SQLite or a real PostgreSQL.
7. **The in-memory provider hid a difference.** `ConnectDbContextTests` ran on the in-memory provider of EF Core, which the same guide calls "highly discouraged" as a stand-in database: "in-memory has all the disadvantages of SQLite, along with a few more - and offers no advantages in return" (Microsoft, 2021). I ran a copy of it on PostgreSQL 16: 12 of 32 red, where the in-memory provider gave 32 of 32. Every red one compared a time read back from the database with the time before the save. PostgreSQL keeps six digits of a second and .NET seven, so `14:27:08.4570872` came back as `14:27:08.457087`. The code was right and the tests were not, and only the real database showed it. Rule 3 now says how to compare a time read back, and rule 7 names the in-memory provider.
8. **The guides say the same.** Part 4 and part 6 of `docs/team/onboarding.md`, `docs/architecture/configuration.md` and `docs/testing/testingStrategy.md` now name `ConnectionStrings:TestsConnection`, the database `connect_test` and NSubstitute. The merge of !50 had put the `dotnet user-secrets init` block back right after the sentence that says not to run it, and that block is gone. I followed part 4 and part 6 word for word with an empty secrets folder: 94 of 94 tests green, and the only database the tests created was the test database. The database of the server was never touched.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rules come from.** My review of !50 (note 1990213): the pipeline was red because backend-test set only `DefaultConnection`, while Nick's `TestConfiguration` reads `TestsConnection`. Gurpreet's note offered to read `DefaultConnection` instead. In my reply I asked to keep `TestsConnection` and add it to backend-test. Nick had added that line four minutes before my reply, and merged !50 with it.
2. **The rules follow the agreed code.** The team agreed that Nick's .NET code sets the way. `AccountRegistrationServiceTests`, merged with !50 while I wrote the rules, uses NSubstitute 6.2.0, also for the class `UserManager<Account>`. Rule 5 follows that: a class only when the members the test needs are virtual. The analyzer accepts every test of his as it is.
3. **What moves, and who does it.** Gurpreet and I move our own tests that still read `DefaultConnection`. Mine are on my task #1541 (notes 1990173 and 1990214): `ProfileDatabaseTests`, `MemberProfileEndpointTests`, `TagDatabaseTests`, `MemberProfileTagEndpointTests`, `VisitorDatabaseTests`, `TestDataSeederTests` and `TestDataSeedingTests`. Gurpreet's is `HealthDatabaseTests`.
4. **One way for every stand-in.** My decision of 20 October 2026: every stand-in in a test is a substitute of NSubstitute, so a test reads the same everywhere. The two stub classes written by hand in the tests of !50 are rewritten in this merge request:
   - `CreateAccountHandlerTests`: `StubRegistrationService` is now a substitute of `IAccountRegistrationService`. The test still checks the result, the email, the password and the cancellation token.
   - `AccountEndpointTests`: `StubCreateAccountHandler` is now a substitute of the handler, and the flag `handlerCalled` is now `DidNotReceive()`. The test of a wrong form turned red when I let the endpoint call the handler before it answered 400, for one run.
   - NSubstitute and the analyzer are in `Connect.Application.Tests` and `Connect.Tests` too, the same versions as in `Connect.Infrastructure.Tests`.
   - My own open merge requests follow the rule too. `CapturingLoggerProvider` in `MemberProfileEndpointTests` (!68, !70 to !73, !75 and !76) and `WarningCapturingLoggerProvider` in `TestDataSeederTests` (!74) are substitutes of `ILoggerProvider` and `ILogger` now. Run side by side on !68, the substitute caught the same 39 log messages as the class it replaces, and the seeder test turned red when I let the seeder log a warning on its second run.
5. **Tests that did not match the rules are adjusted.** Also my decision of 20 October 2026, in this merge request:
   - `ConnectDbContextTests` is now `ConnectDbContextDatabaseTests`, the name of code conventions rule 12 for a database test. It runs on the test database of rule 1 with `TestConfiguration`, the way `AccountDatabaseTests` does, and compares a time read back within a microsecond. The package of the in-memory provider is out of the project file, because no test uses it any more.
   - `AddInfrastructure_ReturnsServices` read `DefaultConnection` since !50. It only registers services and never opens a connection, so it has its own setting in the test again, as it had before !50.
   - With only `TestsConnection` set, 92 of 94 tests pass. The two that do not are Gurpreet's `HealthDatabaseTests`, which he moves himself (#1541). With both names set, as backend-test does, 94 of 94. The adjusted tests passed five runs in a row on new databases.
6. **The validators, after Nick's review.** Nick asked in his review of this merge request on 21 October 2026 (note 1992657) to test the validators with `FluentValidation.TestHelper`, because the tests read better. Rule 8 of the new part says so, backend rule 13 points to it, and `docs/testing/testingStrategy.md` no longer says that no test helper is added. FluentValidation's own guide recommends "treating validators as ‘black boxes’ - provide input to them and then assert whether the validation results are correct or incorrect" (FluentValidation, n.d.). His `CreateAccountRequestValidatorTests` now uses `TestValidate`, `ShouldHaveValidationErrorFor`, `WithErrorMessage` with `Only()`, `WithoutErrorMessage` and `ShouldNotHaveAnyValidationErrors`, and `Only()` keeps what `Errors.Single(...)` checked before: one message for the field. The five tests are green, the two email tests turned red when I changed the email message of the validator for one run, and the password test when I changed the password message. My own validator tests follow the rule too: `CreateMemberProfileRequestValidatorTests` (21 tests, from !68) and `AddTagRequestValidatorTests` (5 tests, from !72), one commit per merge request (!68 2ef31bac, !70 dcd284f5, !71 517b9c84, !72 417ebd83, !73 97c08f53, !75 cc3b9b48, !76 a98b348b). Five tests turned red when I changed the age band message and one when I changed the tag length message. `SuggestedTagsQueryValidator` is not a FluentValidation validator, so its tests stay as they are.

## TODO

- [ ] Nick: confirm in the review that the new part says what `TestConfiguration` and `AccountRegistrationServiceTests` do, and that `ConnectDbContextDatabaseTests`, `AddInfrastructure_ReturnsServices`, `CreateAccountHandlerTests` and `AccountEndpointTests` still test what they tested.
- [ ] Nick: confirm that rule 8 and the rewritten `CreateAccountRequestValidatorTests` are what he asked for in note 1992657.
- [ ] A teammate who did not write the rules: set `TestsConnection` with rule 2 alone, run the tests, and write in the review what was unclear.
- [ ] Wesley and Gurpreet: move the tests that still read `DefaultConnection` to `TestsConnection` (#1541).

## References

FluentValidation. (n.d.). *Test extensions*. FluentValidation documentation. Retrieved October 21, 2026, from https://docs.fluentvalidation.net/en/latest/testing.html

Microsoft. (2021, November 7). *Choosing a testing strategy*. Microsoft Learn. Retrieved October 20, 2026, from https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy

NSubstitute. (n.d.). *Creating a substitute*. Retrieved October 20, 2026, from https://nsubstitute.github.io/help/creating-a-substitute/

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

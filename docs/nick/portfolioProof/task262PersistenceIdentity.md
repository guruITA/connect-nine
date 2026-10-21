# Proof: persistence and account-registration boundaries (#262)

MR [!50](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/merge_requests/50), task [#262](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/262). Profile: Full Stack Software Developer, level 3. Portflow candidates: OR1, OR4, OR5, AA2, AA3 and AA4.

## Result

Application depends on contracts while Infrastructure owns EF Core, Identity registration, database naming, migrations and provider-specific behavior. Account creation uses `IAccountRegistrationService`; `IAppDbContext` is available as a persistence boundary for use cases that need queries and saving.

## OR1 — boundary design and alternatives

The design separates the business use case from technical registration and database details. Relevant evidence is [`IAccountRegistrationService.cs`](../../../api/Connect.Application/Accounts/CreateAccount/IAccountRegistrationService.cs), [`IAppDbContext.cs`](../../../api/Connect.Application/Common/Abstractions/IAppDbContext.cs), [`AccountRegistrationService.cs`](../../../api/Connect.Infrastructure/Identity/AccountRegistrationService.cs), [`ConnectDbContext.cs`](../../../api/Connect.Infrastructure/Persistence/ConnectDbContext.cs), [`IEntity.cs`](../../../api/Connect.Domain/Common/IEntity.cs), [`Account.cs`](../../../api/Connect.Domain/Users/Account.cs) and [`DependencyInjectionTests.cs`](../../../api/Connect.Infrastructure.Tests/DependencyInjectionTests.cs).

The rejected alternative was direct EF Core/Identity access from Application. That would expose infrastructure concerns and make the use case harder to test or reuse. Decision [007 Identity is an exception to the `IAppDbContext` rule](../../decisions/007-identityAndIAppDbContext.md) records the more precise recommendation: ordinary persistence uses `IAppDbContext`, while Identity-owned lifecycle operations use a focused application-owned capability. Account creation therefore uses `IAccountRegistrationService`, whose Infrastructure implementation owns `UserManager<Account>` and translates Identity/database failures into application results. A second rejected claim is that every feature already needs `IAppDbContext`: account creation currently uses the more specific registration service, and the broader context can grow when a consuming use case needs it.

Decision 007 was written after the implementation was approved and merged, so it is evidence of the team’s clarified architectural reasoning and agreement, not proof that the exception was fully planned before coding. The implementation and tests remain the primary evidence for what was actually built.

## OR4 — database validation

The tests show several separate guarantees. [`AccountRegistrationServiceTests.cs`](../../../api/Connect.Infrastructure.Tests/Identity/AccountRegistrationServiceTests.cs) covers Identity success, trimming, cancellation, duplicate Identity errors, conflict precedence, logged failure codes, recognized PostgreSQL unique violations and propagation of unrelated database exceptions. [`AccountDatabaseTests.cs`](../../../api/Connect.Infrastructure.Tests/Persistence/AccountDatabaseTests.cs) runs against PostgreSQL and covers case-insensitive uniqueness, normalized lookup, password-hash constraints and a concurrent insert where one row remains. [`ConnectDbContextTests.cs`](../../../api/Connect.Infrastructure.Tests/Persistence/ConnectDbContextTests.cs) covers the `IEntity` constraint, tracking, auditing and cancellation using the InMemory provider. The migration [`20261020083006_EnforceNormalizedAccountUniqueness.cs`](../../../api/Connect.Infrastructure/Persistence/Migrations/20261020083006_EnforceNormalizedAccountUniqueness.cs) makes normalized uniqueness a database rule as well as a service-level check. The supplied run reports 61 Infrastructure tests passed.

The final evidence must distinguish tests that can run without PostgreSQL from tests that require the real provider. A passing lightweight test is not evidence that PostgreSQL constraints, migrations or race behavior are correct.

## OR5 and AA3 — privacy and ethical consequences

The service needs an email address to register an account, but internal persistence and password handling must not become a public API contract. The database stores a password hash through Identity; the endpoint tests separately assert that the submitted password is absent from the response. Duplicate-email responses also create an account-membership signal. The current implementation should be described honestly as a usability/privacy trade-off; indistinguishable acknowledgement through email verification is a future option, not an implemented fact.

## AA2 and AA4 — critical comparison and recommendation

The recommendation is to keep persistence behind contracts and let Infrastructure translate the known database conflict into an application error. Unknown database failures remain exceptions, because pretending every database exception is a business conflict would hide operational failures. This is also why the proof must not describe all database errors as temporary unreachability.

## Limits

Do not claim full HTTP-to-Identity registration coverage, password-hash verification or concurrency-race proof unless the final tests explicitly demonstrate them. Do not claim that `IAppDbContext` is required by every future feature.

## Evidence checklist

- [x] Attach project-reference/dependency-registration evidence: [`Connect.Application.csproj`](../../../api/Connect.Application/Connect.Application.csproj), [`Connect.Infrastructure.csproj`](../../../api/Connect.Infrastructure/Connect.Infrastructure.csproj) and [`DependencyInjectionTests.cs`](../../../api/Connect.Infrastructure.Tests/DependencyInjectionTests.cs).
- [x] Link the migration and PostgreSQL test setup: [`EnforceNormalizedAccountUniqueness`](../../../api/Connect.Infrastructure/Persistence/Migrations/20261020083006_EnforceNormalizedAccountUniqueness.cs), [`TestConfiguration.cs`](../../../api/Connect.Infrastructure.Tests/TestConfiguration.cs) and [`docker-compose.yml`](../../../docker-compose.yml).
- [x] Link the architectural clarification: [decision 007](../../decisions/007-identityAndIAppDbContext.md).
- [x] Add the actual test output and provider details; see the [shared test run](task262ValidationAndTesting.md#test-run), the [backend-tests pipeline job](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/jobs/1345780) and the linked PostgreSQL configuration above.
- [ ] Add any review evidence that caused a boundary or database rule to change.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

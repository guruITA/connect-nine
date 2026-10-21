# 007 Identity is an exception to the IAppDbContext rule

**Status.** Agreed on 20 October 2026 by the team members listed below. The decision was written down after the implementation was approved and merged.
**Issue.** How ASP.NET Core Identity persists accounts when application handlers normally use `IAppDbContext`.
**Related decision.** [002 The structure of the server](002-layeredArchitecture.md) defines the general layered-architecture and `IAppDbContext` rule; this decision records its Identity exception.
**Related merge request.** [!58](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/merge_requests/58) changes the surrounding documents through `docs/12-dbcontext-name-follows-code`; it does not contain this decision record.
**Who decides.** The whole team. Drafted by Nick Hoebe.

## Context

Decision 002 says that an Application handler reaches persistence through the
application-owned `IAppDbContext` contract. This keeps handlers independent
from Infrastructure and gives normal feature code one consistent data-access boundary.

Account creation is different. `Account` inherits from `IdentityUser<Guid>`, and
`ConnectDbContext` inherits from `IdentityDbContext<Account, IdentityRole<Guid>, Guid>`.
ASP.NET Core Identity owns the account-management workflow: it creates the user, hashes
and validates the password, applies Identity options, and persists related Identity data.

The current implementation therefore uses `IAccountRegistrationService` in Application.
Infrastructure implements it with `UserManager<Account>`. The service translates
`IdentityResult` and database uniqueness failures into application-owned results before
the handler sees them.

This is also a deliberate compromise in dependency direction. The Application project
owns the use-case contract but does not directly reference ASP.NET Core Identity or EF
Core. The concrete `UserManager<Account>`, `IdentityDbContext` and provider-specific
database behavior remain in Infrastructure. Application still has the small
`IAppDbContext` contract for ordinary persistence, without taking a dependency on the
concrete EF Core context or Identity framework.

## Options

### 1. Force Identity operations through IAppDbContext

The account handler would construct or modify `Account` through `IAppDbContext` and save
it directly.

| Advantages | Disadvantages |
|---|---|
| Follows the general persistence rule literally. | Bypasses `UserManager` and its password, validation and normalization workflow. |
| Uses one apparent data-access contract. | Application would need to reproduce or understand Identity-specific behavior. |
| The handler would look similar to other write handlers. | Identity-related tables and invariants could be persisted incorrectly. |

### 2. Use an application-owned Identity capability

The account use case depends on `IAccountRegistrationService`. Infrastructure uses
`UserManager<Account>` and hides Identity-specific types and behavior behind that
contract.

| Advantages | Disadvantages |
|---|---|
| Keeps the Identity workflow in the framework integration boundary. | Requires a focused interface and an Infrastructure implementation. |
| Application receives an application-owned `Result` instead of `IdentityResult`. | Identity-specific failures must be translated deliberately. |
| The service can be substituted in application tests. | A new Identity use case may need another focused capability. |

### 3. Expose UserManager directly to Application

Application handlers would inject `UserManager<Account>` and call Identity directly.

| Advantages | Disadvantages |
|---|---|
| Requires the least wrapper code. | Application becomes coupled to ASP.NET Core Identity. |
| Gives handlers the complete Identity API. | Framework-specific results and behavior leak into the use case. |
| Familiar to ASP.NET Core developers. | Testing and a future identity implementation change become harder. |

## Decision

Choose option 2. **ASP.NET Core Identity is a deliberate, narrow exception to the
normal `IAppDbContext` persistence rule.** Account-management use cases must use a
focused application-owned Identity capability, currently `IAccountRegistrationService`,
instead of writing Identity entities through `IAppDbContext`.

This exception applies only to operations that are owned by ASP.NET Core Identity, such
as account registration, password handling, sign-in and role management. It does not
permit ordinary application features to inject `UserManager`, `RoleManager`,
`ConnectDbContext` or `DbContext` directly.

The boundary has these responsibilities:

- Application defines the smallest interface needed by the use case.
- Infrastructure implements that interface using ASP.NET Core Identity. The concrete
  `UserManager<Account>` stays in Infrastructure rather than moving into Application.
- Identity-specific types such as `IdentityResult` and `IdentityError` do not cross into
  Application.
- Infrastructure translates Identity and database failures into application-owned
  results and logs diagnostic details without exposing them to the client.
- Application handlers still own the use-case flow and depend only on the application
  contract.
- `IAppDbContext` remains the persistence contract for non-Identity entities and
  application-owned data, including `Profile`.
- This keeps the production Application project minimally dependent on persistence
  technology: its project references Domain and dependency-injection abstractions, while
  EF Core and ASP.NET Core Identity are Infrastructure concerns.

The exception is not a general repository exception and does not justify adding a
second data-access abstraction for every entity. New Identity capabilities require a
concrete use case first; then the team adds a focused interface rather than copying the
whole `UserManager<TUser>` API.

## Consequences

- The architecture rule is clear: normal persistence uses `IAppDbContext`; Identity
  lifecycle operations use a focused application-owned Identity interface.
- Account creation preserves Identity's password hashing, validation, normalization and
  related persistence behavior.
- Application remains insulated from `IdentityResult`, `IdentityError` and the concrete
  `UserManager<Account>` implementation.
- Moving `UserManager<Account>` and EF Core persistence into Infrastructure keeps the
  Application project small and framework-independent, while the application-owned
  contracts preserve testability.
- Infrastructure contains the translation between framework failures, PostgreSQL
  uniqueness failures and application results.
- There is a small amount of additional interface and mapping code.
- A reviewer must check that the exception stays limited to Identity-owned operations
  and that new interfaces expose only a real use case.
- Direct `ConnectDbContext` access remains appropriate in Infrastructure tests and
  infrastructure-only services, but not in Application handlers.

## Agreed by

| Name | Date agreed | Remark |
|---|---|---|
| Nick Hoebe | 20 October 2026 | Agreed and documented after the implementation was merged |
| Gurpreet Singh | 20 October 2026 | Agreed |
| Wesley Oerlemans | 20 October 2026 | Agreed |

## References

- `docs/decisions/002-layeredArchitecture.md`, the layered architecture and the general `IAppDbContext` rule.
- `docs/future-improvements/003-abstractAspnetIdentity.md`, the review of focused Identity abstractions.
- `docs/team/codeConventions.md`, backend structure rule 7.
- `api/Connect.Application/Common/Abstractions/IAppDbContext.cs`.
- `api/Connect.Application/Accounts/CreateAccount/IAccountRegistrationService.cs`.
- `api/Connect.Infrastructure/Identity/AccountRegistrationService.cs`.
- `api/Connect.Infrastructure/Persistence/ConnectDbContext.cs`.

## AI use declaration

AI was used as a writing support tool while working on this document. AI was used to
inspect the existing implementation and related decisions, and to improve the
structure, wording, readability and clarity of the documentation. The decision was
checked against the repository before inclusion.

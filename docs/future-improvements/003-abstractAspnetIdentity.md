# 003 Review the ASP.NET Core Identity abstraction

**Status.** Proposed on 13 October 2026. This is a possible improvement for a future team to consider, not work that must be completed this semester.

**Related improvements.** 001, A clearer separation between Application and Infrastructure.

**Related issues.** Issue 03, Create an account.

**Who decides.** The whole team, at a stand-up or a meeting. This document describes a possible improvement and does not replace the existing architecture decision.

## Context

Connect uses ASP.NET Core Identity to manage user accounts. Its `UserManager<Account>` provides operations such as creating users, finding users and validating passwords.

Account creation now depends on the application-owned `IAccountRegistrationService` interface. Infrastructure implements that interface through `AccountRegistrationService`, which delegates account creation to `UserManager<Account>` and translates Identity and database uniqueness failures into application-owned results.

This is a useful boundary for the current use case. A future team can review whether additional identity use cases should use similarly focused application-owned interfaces.

This would not require introducing a repository or query service. The intention is to abstract user-management operations, not to create another layer for database queries.

## Options

### 1. Keep using UserManager in Application

Application handlers inject `UserManager<Account>` and call its methods directly.

| Advantages | Disadvantages |
|---|---|
| Simple implementation with little additional code. | Application depends directly on ASP.NET Core Identity. |
| Provides access to the full functionality of UserManager. | Application handlers may be harder to test without framework-specific setup. |
| Familiar to developers working with ASP.NET Core Identity. | Changes to the identity implementation may affect application handlers. |

### 2. Keep focused application-owned abstractions

Application defines interfaces containing only the operations required by each use case. Infrastructure implements them using `UserManager<Account>`. `IAccountRegistrationService` and `AccountRegistrationService` are the current example.

| Advantages | Disadvantages |
|---|---|
| Application handlers depend on an application-owned contract. | Requires an interface and an implementation. |
| The identity dependency can be substituted in unit tests. | Identity errors may need to be translated into application-level results. |
| The underlying implementation can change with less impact on handlers. | The abstraction requires maintenance as requirements evolve. |

### 3. Introduce broader or smaller interfaces only when needed

Keep account creation separate from other identity capabilities, such as password verification, unless a concrete use case requires combining them. Avoid adding interfaces without a meaningful boundary.

| Advantages | Disadvantages |
|---|---|
| Each interface can expose a narrow set of operations. | Multiple interfaces may create unnecessary complexity. |
| Use cases can depend on only the capabilities they require. | Additional registrations and implementations may be needed. |

## Proposed recommendation

**Keep the existing focused abstraction and extend it only when a concrete use case justifies it.** The move of account registration behind `IAccountRegistrationService` already removes direct `UserManager` usage from the account-creation handler.

The following guidelines could be applied if the team decides to pursue this improvement.

### 1. Define the interface in Application

`IAccountRegistrationService` lives with the account-creation use case in Application. Future interfaces should follow the same pattern: describe only the capability needed by an application use case, rather than reproduce the entire `UserManager<TUser>` API.

Potential operations include:

- Creating an account.
- Finding a user by an identifier when a use case requires it.
- Finding a user by email when a use case requires it.
- Validating a user's password when a use case requires it.

The interface should only include operations that the application actually uses.

### 2. Implement the interface in Infrastructure

Infrastructure provides `AccountRegistrationService`, which delegates account creation to `UserManager<Account>`.

The service translates Identity failures and database unique-constraint failures into application-owned `Result<CreateAccountResult>` values.

Application handlers depend on `IAccountRegistrationService`, while the concrete implementation remains an Infrastructure concern.

### 3. Keep Identity-specific results behind the boundary

ASP.NET Core Identity uses types such as `IdentityResult` and `IdentityError`.

If the goal is to keep Application independent of Identity, the interface should avoid exposing these types. Infrastructure can translate success and failure into an application-owned result type containing the information that use cases need.

The abstraction should preserve useful error information without leaking the framework's result types.

### 4. Consider the Account dependency

**An interface around `UserManager` does not automatically remove every dependency on ASP.NET Core Identity.**

If `Account` inherits from `IdentityUser<Guid>`, an Application interface that accepts or returns `Account` still depends on an Identity-specific type. The current registration interface avoids this by accepting primitive input and returning an application result containing the account ID and email.

There are several possible approaches:

- **Accept the dependency.** Keep using `Account` if the coupling is limited and the benefits of introducing a separate model do not justify the additional work.
- **Use an application-facing model.** Expose only the user information required by a use case through an application-owned model, such as a user summary containing an ID and email address.
- **Separate the domain model from the Identity model.** Consider this only if the application has meaningful domain requirements for users that should be independent of authentication and identity management.

These approaches are alternatives, not mandatory steps. A separate user model introduces mapping and synchronization considerations, so it should not be introduced merely to remove a framework reference.

### 5. Keep the abstraction focused

Do not expose every method available on `UserManager<TUser>`. Avoid turning application-owned identity interfaces into duplicates of the original framework API.

If different use cases eventually need substantially different capabilities, the team can consider smaller interfaces at that point.

### 6. Register implementations through dependency injection

Infrastructure registers `AccountRegistrationService` for `IAccountRegistrationService`. Future identity abstractions should use the same dependency-injection boundary. The startup project should compose the application using the relevant dependency registration methods.

## Consequences

The current account-registration abstraction provides these benefits:

- The account-creation handler avoids a direct dependency on `UserManager<Account>`.
- Identity operations can be substituted in unit tests.
- The underlying identity implementation can potentially be changed with less impact on Application.
- Additional interfaces, result mappings and registration code may be needed.
- Application may still depend on Identity if it continues to use `Account` or other Identity-specific types.
- Introducing separate user models may add complexity that is not justified by the current requirements.

A future team should identify remaining `UserManager` usage and whether that coupling creates a meaningful problem. Extend the existing focused abstraction only when it addresses a concrete need without creating an unnecessary identity layer.

## References

Microsoft. (n.d.). *Introduction to Identity on ASP.NET Core*. Microsoft Learn. https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity

Microsoft. (n.d.). *Common web application architectures*. Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

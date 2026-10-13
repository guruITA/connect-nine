# 003 Abstract ASP.NET Core Identity from Application

**Status.** Proposed on 13 October 2026. This is a possible improvement for a future team to consider, not work that must be completed this semester.

**Related improvements.** 001, A clearer separation between Application and Infrastructure.

**Related issues.** Issue 03, Create an account.

**Who decides.** The whole team, at a stand-up or a meeting. This document describes a possible improvement and does not replace the existing architecture decision.

## Context

Connect uses ASP.NET Core Identity to manage user accounts. Its `UserManager<TUser>` provides operations such as creating users, finding users and validating passwords.

If Application handlers depend directly on `UserManager<AppUser>`, they become coupled to ASP.NET Core Identity. This may be acceptable for a small application, but it can make application use cases harder to separate from the identity implementation.

A possible improvement is to define an application-owned interface that exposes only the identity operations required by the application. Infrastructure can implement that interface by delegating to ASP.NET Core Identity.

This would not require introducing a repository or query service. The intention is to abstract user-management operations, not to create another layer for database queries.

## Options

### 1. Use UserManager directly

Application handlers inject `UserManager<AppUser>` and call its methods directly.

| Advantages | Disadvantages |
|---|---|
| Simple implementation with little additional code. | Application depends directly on ASP.NET Core Identity. |
| Provides access to the full functionality of UserManager. | Application handlers may be harder to test without framework-specific setup. |
| Familiar to developers working with ASP.NET Core Identity. | Changes to the identity implementation may affect application handlers. |

### 2. Introduce an application-owned identity abstraction

Application defines an interface containing only the operations required by its use cases. Infrastructure implements it using `UserManager<AppUser>`.

| Advantages | Disadvantages |
|---|---|
| Application handlers depend on an application-owned contract. | Requires an interface and an implementation. |
| The identity dependency can be substituted in unit tests. | Identity errors may need to be translated into application-level results. |
| The underlying implementation can change with less impact on handlers. | The abstraction requires maintenance as requirements evolve. |

### 3. Introduce smaller interfaces for individual capabilities

Define separate interfaces for distinct requirements, such as account creation and password verification.

| Advantages | Disadvantages |
|---|---|
| Each interface can expose a narrow set of operations. | Multiple interfaces may create unnecessary complexity. |
| Use cases can depend on only the capabilities they require. | Additional registrations and implementations may be needed. |

## Proposed recommendation

**Option 2: introduce an application-owned identity abstraction if the existing framework coupling justifies it.**

The following guidelines could be applied if the team decides to pursue this improvement.

### 1. Define the interface in Application

For example, an interface named `IUserIdentityService` could live in `Application/Abstractions/Identity`.

It should describe the identity capabilities needed by application use cases, rather than reproduce the entire `UserManager<TUser>` API.

Potential operations include:

- Finding a user by an identifier.
- Finding a user by email when a use case requires it.
- Creating an account.
- Validating a user's password when needed.

The interface should only include operations that the application actually uses.

### 2. Implement the interface in Infrastructure

Infrastructure would provide an implementation that delegates to `UserManager<AppUser>`.

For example, `IdentityUserService` could implement `IUserIdentityService` and translate the results of Identity operations into application-owned result types.

Application handlers would depend on `IUserIdentityService`, while the concrete implementation would remain an Infrastructure concern.

### 3. Keep Identity-specific results behind the boundary

ASP.NET Core Identity uses types such as `IdentityResult` and `IdentityError`.

If the goal is to keep Application independent of Identity, the interface should avoid exposing these types. Infrastructure can translate success and failure into an application-owned result type containing the information that use cases need.

The abstraction should preserve useful error information without leaking the framework's result types.

### 4. Consider the AppUser dependency

**An interface around `UserManager` does not automatically remove every dependency on ASP.NET Core Identity.**

If `AppUser` inherits from `IdentityUser`, an Application interface that accepts or returns `AppUser` still depends on an Identity-specific type. The interface hides the service dependency, but not the user model dependency.

There are several possible approaches:

- **Accept the dependency.** Keep using `AppUser` if the coupling is limited and the benefits of introducing a separate model do not justify the additional work.
- **Use an application-facing model.** Expose only the user information required by a use case through an application-owned model, such as a user summary containing an ID and email address.
- **Separate the domain model from the Identity model.** Consider this only if the application has meaningful domain requirements for users that should be independent of authentication and identity management.

These approaches are alternatives, not mandatory steps. A separate user model introduces mapping and synchronization considerations, so it should not be introduced merely to remove a framework reference.

### 5. Keep the abstraction focused

Do not expose every method available on `UserManager<TUser>`. Avoid turning `IUserIdentityService` into a duplicate of the original framework API.

If different use cases eventually need substantially different capabilities, the team can consider smaller interfaces at that point.

### 6. Register the implementation through dependency injection

Infrastructure should register the implementation of `IUserIdentityService`. The startup project should compose the application using the relevant dependency registration methods.

## Consequences

If this improvement is adopted:

- Application handlers can avoid a direct dependency on `UserManager<AppUser>`.
- Identity operations can be substituted in unit tests.
- The underlying identity implementation can potentially be changed with less impact on Application.
- Additional interfaces, result mappings and registration code may be needed.
- Application may still depend on Identity if it continues to use `AppUser` or other Identity-specific types.
- Introducing separate user models may add complexity that is not justified by the current requirements.

A future team should first identify where `UserManager` is used and whether that coupling creates a meaningful problem. If an abstraction is introduced, it should address those concrete needs without creating an unnecessary identity layer.

## References

Microsoft. (n.d.). *Introduction to Identity on ASP.NET Core*. Microsoft Learn. https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity

Microsoft. (n.d.). *Common web application architectures*. Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

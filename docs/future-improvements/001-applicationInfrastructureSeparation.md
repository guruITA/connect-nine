# 001 A clearer separation between Application and Infrastructure

**Status.** Proposed on 13 October 2026. This is a possible improvement for a future team to consider, not work that must be completed this semester.

**Related issues.** Issue 11, The team agrees on the layered architecture. Issue 03, Create an account.

**Who decides.** The whole team, at a stand-up or a meeting. This document describes a possible improvement and does not replace the existing architecture decision.

## Context

Connect uses four main projects: Presentation, Application, Domain and Infrastructure. The team chose this layered architecture to give each project a clear responsibility and use project references to enforce the intended dependency direction.

The existing architecture already applies dependency inversion in some areas. For example, Application defines `IAppDbContext`, while Infrastructure provides the EF Core implementation through `ConnectDbContext`. Account creation follows the same direction: Application defines `IAccountRegistrationService`, while Infrastructure implements it through ASP.NET Core Identity in `AccountRegistrationService`.

However, introducing an interface does not automatically remove coupling. An abstraction can still expose implementation-specific types, and Application can remain dependent on a particular framework even when that framework is accessed through an interface.

As the application grows, the team may benefit from reviewing these boundaries to establish whether they are sufficiently clear and whether the current abstractions provide the intended separation.

Microsoft describes Clean Architecture as an approach in which the application core defines abstractions that infrastructure implements (Microsoft, n.d.-a).

## Options

### 1. Keep the current architecture

Continue using the existing four projects and current abstractions.

| Advantages | Disadvantages |
|---|---|
| No additional refactoring is required. | Existing framework dependencies may become harder to identify as the application grows. |
| The current architecture is already established. | The responsibility of an abstraction may not always be clear. |
| The approach can remain simple and pragmatic. | Some dependencies may make isolated testing or future implementation changes harder. |

### 2. Review and strengthen architectural boundaries

Keep the four projects, but review whether Application abstractions describe application needs or expose details of their underlying implementations.

Possible areas for review include:

- The EF Core functionality exposed by `IAppDbContext`.
- Direct dependencies on ASP.NET Core Identity.
- The location of external-service integrations and their abstractions.
- Dependency registration and project references.

| Advantages | Disadvantages |
|---|---|
| Dependencies can be isolated where doing so provides value. | Existing code may need to be refactored. |
| Application use cases can become easier to test independently. | Additional interfaces and result mappings may be needed. |
| Developers gain clearer guidance about where new code belongs. | Poorly designed abstractions can add complexity without improving separation. |

### 3. Introduce repositories for all database access

Create repository interfaces in Application and implementations in Infrastructure for each entity or aggregate.

| Advantages | Disadvantages |
|---|---|
| Persistence operations can be exposed through focused contracts. | Repositories can duplicate functionality already provided by EF Core. |
| Database-specific behavior can be hidden behind repository implementations. | Many interfaces and implementations may be required. |
| Repository contracts can support particular domain or persistence requirements. | Complex queries and projections can become more cumbersome if every query must pass through generic repositories. |

EF Core's `DbContext` already provides functionality associated with the Repository and Unit of Work patterns. Custom repositories can still be useful when they provide a meaningful abstraction (Microsoft, n.d.-b).

## Proposed recommendation

**Option 2: review and strengthen the existing boundaries without introducing repositories everywhere by default.**

The following guidelines could be applied if the team decides to pursue this improvement.

### 1. Keep use cases in Application

Commands, queries, their handlers and application-level coordination belong in Application. Handlers should implement use cases rather than contain technical infrastructure implementations.

### 2. Keep technical implementations in Infrastructure

`ConnectDbContext`, EF Core configurations, migrations, database-specific behavior and concrete external-service integrations belong in Infrastructure.

Application may define abstractions for the capabilities it requires, with Infrastructure providing their implementations.

### 3. Review existing abstractions

Review `IAppDbContext` and other Application-facing interfaces to determine whether they expose implementation details unnecessarily.

For example, `DbSet<T>` and `IQueryable<T>` are EF Core or LINQ-related abstractions that can expose details of query construction and execution. They may be appropriate for some applications, but they should be an intentional choice.

See improvement 002 for the database abstraction and improvement 003 for the ASP.NET Core Identity boundary.

### 4. Avoid unnecessary abstractions

Do not create repositories, interfaces or services merely to follow a pattern. Introduce them when they clarify a boundary, isolate a dependency or make testing meaningfully easier.

### 5. Keep dependency registration clear

Application and Infrastructure can each provide a registration method, such as `AddApplication()` and `AddInfrastructure(configuration)`. The startup project should compose the application through dependency injection.

See improvement 004 for a more detailed proposal.

## Intended dependency direction

The following project references should continue to enforce the intended dependency direction:

- Presentation references Application.
- Application references Domain.
- Infrastructure references Application to implement its abstractions.
- Domain has no project dependencies.
- The startup project references the required layers and composes the application.

Infrastructure may depend on external frameworks and technical implementations. Application should only take on additional dependencies when their benefits justify the coupling.

## Consequences

If this improvement is adopted:

- Developers should have clearer guidance about where code belongs.
- Unnecessary coupling to infrastructure implementations may be reduced.
- Application use cases may become easier to test in isolation.
- Some existing code may need to be refactored.
- Additional abstractions may introduce maintenance costs.
- Some framework dependencies may remain intentionally because removing them would add complexity without meaningful benefits.

A future team should review the actual implementation and tests before deciding which changes are worthwhile. The objective is to improve the architecture where it matters, not to eliminate every framework reference at any cost.

## References

Microsoft. (n.d.-a). *Common web application architectures*. Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

Microsoft. (n.d.-b). *Implementing the infrastructure persistence layer with Entity Framework Core*. Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implementation-entity-framework-core

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

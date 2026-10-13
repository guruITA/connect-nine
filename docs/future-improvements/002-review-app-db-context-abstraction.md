# 002 Review the IAppDbContext abstraction

**Status.** Proposed on 13 October 2026. This is a possible improvement for a future team to consider, not work that must be completed this semester.

**Related improvements.** 001, A clearer separation between Application and Infrastructure.

**Who decides.** The whole team, at a stand-up or a meeting. This document describes a possible improvement and does not replace the existing architecture decision.

## Context

Connect defines `IAppDbContext` in Application and implements it through `ConnectDbContext` in Infrastructure. This allows application handlers to depend on an abstraction instead of the concrete EF Core database context.

However, the level of separation depends on what `IAppDbContext` exposes.

If the interface exposes `DbSet<T>`, `IQueryable<T>` or generic persistence operations, Application may still depend on EF Core's query and persistence model. Although these types provide flexibility, they can make it harder to change the persistence implementation without affecting application code.

This does not automatically make the current design wrong. EF Core is designed to support application development with `DbContext`, and exposing query capabilities can be a pragmatic choice, particularly for CQRS query handlers.

The improvement is to review whether the existing abstraction matches the application's needs or whether more focused persistence contracts would provide a useful separation.

## Options

### 1. Keep the current IAppDbContext

Continue exposing the existing database abstraction to application handlers.

| Advantages | Disadvantages |
|---|---|
| Simple and flexible. | Application may remain coupled to EF Core-related types. |
| Query handlers can construct queries directly. | Testing queries independently of EF Core may be more difficult. |
| Avoids additional interfaces and implementations. | Database-specific query behavior can become part of application logic. |

### 2. Introduce use-case-specific persistence operations

Replace or supplement generic database access with interfaces that expose the operations required by particular use cases.

For example, an account-creation handler may require an operation to check whether an email address is already registered. A query handler may need a projection containing only a user's ID and display name.

These requirements could be expressed through focused contracts rather than exposing a general-purpose database context.

| Advantages | Disadvantages |
|---|---|
| Application depends on operations it actually needs. | More interfaces and implementation methods may be required. |
| Persistence details can be hidden behind focused contracts. | Query requirements may evolve and require contract changes. |
| Unit tests can substitute the required operations. | A large number of small interfaces can become difficult to maintain. |

### 3. Introduce repositories for selected persistence requirements

Use repositories where they provide a meaningful abstraction, such as aggregate persistence or complex domain-specific queries, while retaining direct query capabilities where they are appropriate.

| Advantages | Disadvantages |
|---|---|
| Can provide clear boundaries for particular persistence operations. | Requires careful decisions about which operations belong in repositories. |
| Can protect domain invariants around aggregate persistence. | Generic repositories may duplicate EF Core functionality. |
| Allows different persistence approaches for different needs. | A mixed approach requires clear conventions. |

## Proposed recommendation

**Review the existing IAppDbContext before changing it.** Do not automatically replace it with repositories or remove `IQueryable<T>` everywhere.

A future team should:

1. Identify the members currently exposed by `IAppDbContext`.
2. Find which handlers and other application components use each member.
3. Determine whether those uses introduce actual coupling or testing problems.
4. Keep flexible query access where it is useful and understood.
5. Introduce more focused persistence operations where they meaningfully improve separation.

### Important considerations

**Query handlers.** A query handler that builds an EF Core query through `IQueryable<T>` may depend on query translation, supported expressions and provider behavior. Such a query cannot always be tested accurately with an in-memory collection or a mocked `IQueryable<T>`. Integration tests against the actual database provider may be more appropriate.

**Command handlers.** Commands that only need a few persistence operations may benefit from more focused contracts, especially when those contracts express a meaningful application requirement.

**Unit of Work.** EF Core's `DbContext` already implements Unit of Work-like behavior. A separate `IUnitOfWork` should only be introduced if it provides a useful application-facing contract, such as a clear save boundary. It should not simply duplicate `SaveChangesAsync()` without a reason.

**Repository design.** If repositories are introduced, they should have a clear purpose. A generic repository with `Get`, `Add`, `Update` and `Delete` methods may offer little value over `DbContext`. Focused repository operations can be more useful when they encapsulate meaningful persistence requirements.

## Consequences

If this improvement is adopted:

- Application persistence dependencies may become more explicit.
- Some handlers may become easier to test independently of the database.
- Query implementations may need to move into Infrastructure.
- Additional interfaces and implementation code may be required.
- Some direct EF Core query access may remain by design.

The team should select the smallest change that solves an identified problem. A more abstract design is not automatically a better design.

## References

Microsoft. (n.d.). *Implementing the infrastructure persistence layer with Entity Framework Core*. Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implementation-entity-framework-core

Microsoft. (n.d.). *Common web application architectures*. Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

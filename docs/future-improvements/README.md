# Future improvements

This folder contains proposals for possible improvements to Connect's architecture, implementation and development practices.

These documents describe ideas that the current or a future team can evaluate. They are not requirements, approved architectural decisions or work that must be completed during the current semester.

## Proposals

| Document | Description |
|---|---|
| [001 - Application and Infrastructure separation](001-application-infrastructure-separation.md) | Reviews the existing layered architecture and proposes ways to strengthen the separation between Application and Infrastructure. |
| [002 - Review the IAppDbContext abstraction](002-review-app-db-context-abstraction.md) | Examines whether the current database abstraction exposes more EF Core functionality than Application needs. |
| [003 - Review the ASP.NET Core Identity abstraction](003-abstract-aspnet-identity.md) | Documents the existing account-registration abstraction and explores when additional identity boundaries are justified. |

## How to use these documents

Each proposal describes the context, possible approaches, a recommendation and the potential consequences of adopting it.

A future team should:

1. Review the relevant parts of the current implementation.
2. Determine whether the described problem exists and whether it has a meaningful impact.
3. Discuss the options and trade-offs with the team.
4. Decide whether to adopt, modify or reject the proposal.
5. Update the relevant documentation if a decision is made.

The proposals are related, but they do not all need to be implemented together. A team can adopt an individual improvement when it provides value without implementing the others first.

## Guiding principle

The purpose of these proposals is to improve maintainability, testability and clarity where doing so provides practical value.

Additional interfaces, services and layers also introduce complexity. They should be added when they solve a meaningful problem, not merely to follow a particular architectural pattern.

The existing architecture and implementation remain the source of truth until the team agrees on a change.

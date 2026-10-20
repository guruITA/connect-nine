# Nick's Design Patterns Evidence

> **Scope:** Patterns visible in, or directly proposed for, the current backend in `api/`  
> **Purpose:** Explain which patterns are being used, why they fit Connect, and which
> patterns are still proposals.

## My contribution

I documented the patterns used by the backend example that I designed and explained in
[`architecture.md`](architecture.md). I focused on patterns that can be supported by the
current `api/` code instead of presenting every planned architecture idea as if it were
already implemented.

The current user-creation flow is the example used throughout this document. It shows how
dependency injection, interfaces, a command/use-case handler, and layered separation work
together. CQRS and Strategy are included as future directions, but their status is
explicitly marked because the current API does not yet contain a complete query side or a
matching feature.

## Pattern status

| Pattern or structure | Status in the current API |
| --- | --- |
| Layered architecture | Implemented through separate backend projects |
| Dependency injection | Implemented |
| Interface-based abstraction | Implemented |
| Command/use-case handler | Implemented for creating users |
| CQRS | Partly represented by the create command; not yet a complete command/query separation |
| Strategy | Proposed for future matching rules; not implemented |

## 1. Layered architecture

Layered architecture is the main architectural structure of the backend. The API is split
into projects with different responsibilities:

- Presentation handles HTTP requests and validation.
- Application coordinates use cases.
- Domain contains business concepts such as `User`.
- Infrastructure handles PostgreSQL, EF Core, migrations, and concrete technical services.

### Problem

Without a separation between these responsibilities, endpoints can become responsible for
validation, business decisions, password hashing, and database access at the same time.
That makes the code harder to understand and increases the chance that a technical change
affects unrelated behavior.

### Where it is used

The structure is visible in the project files under `api/`:

- `Connect.Presentation`
- `Connect.Application`
- `Connect.Domain`
- `Connect.Infrastructure`

The host project `Connect` composes these projects in `Program.cs`.

### Why it fits Connect

The team is building one product with one database, so separate services would add
operational complexity that the current project does not need. Separate projects still
give the team clear boundaries and make it easier for developers to find code.

The structure is deliberately limited. It provides maintainability and a possible path to
scale the application later without introducing microservices or additional infrastructure
before the product requires it.

### Alternative

A single project with folders would be simpler at the beginning, but the compiler would not
protect the boundaries between the layers. Microservices would create separate deployment,
communication, and data-ownership problems for a small student team.

## 2. Dependency injection

Dependency injection supplies classes with the services they need instead of making them
construct those services directly.

### Problem

`UserService` needs an `ICreateUserHandler`, and `CreateUserCommandHandler` needs both a
database context and a password hasher. If those classes constructed concrete
implementations themselves, they would be tightly coupled to Infrastructure and harder to
replace or test.

### Where it is used

The host registers the application parts through extension methods:

- `AddApplication()` registers `UserService`.
- `AddInfrastructure()` registers `ConnectDbContext`, `IPasswordHasher`, and
  `ICreateUserHandler`.
- `AddPresentation()` registers the request validators.

ASP.NET Core then supplies these dependencies to the endpoint and services at runtime.

### Why it fits Connect

Dependency injection is already part of ASP.NET Core, so the team does not need to
maintain an additional container. It keeps the composition of the application in one
recognisable place and allows the application flow to depend on contracts.

### Cost and alternative

The registrations add another place that must remain correct: a missing registration is
usually found when the application starts or handles a request. The simpler alternative
would be to instantiate dependencies with `new`, but that would couple the code to concrete
technical implementations and make replacement or testing more difficult.

## 3. Interface-based abstraction

Interfaces are used as boundaries between the application flow and concrete
implementations.

### Problem

Application code should not need to know which password hashing library is used or which
class performs the create-user persistence operation. Those technical choices may change,
and tests may need replaceable implementations.

### Where it is used

The create-user flow uses:

- `ICreateUserHandler`, implemented by `CreateUserCommandHandler`
- `IPasswordHasher`, implemented by `PasswordHasher`

Infrastructure registers these mappings in its dependency injection setup.

### Why it fits Connect

The contracts describe what the application needs without exposing how the work is done.
For example, the application asks for a password hash through `IPasswordHasher` rather
than depending directly on `Microsoft.AspNetCore.Identity.PasswordHasher`.

### Cost and alternative

An interface introduces another file and another name to understand. For a very small
operation, calling a concrete class directly could be simpler. In this backend the
interfaces are useful because they keep the application flow separate from Infrastructure
and provide a clear replacement point.

## 4. Command and use-case handler

The user-creation operation is represented as a command and handled by a dedicated class.
This is a use-case or command-handler structure. It is also the first part of the
feature-oriented organisation described in the architecture document.

### Problem

Putting all user-creation logic inside the HTTP endpoint would mix transport concerns with
normalisation, duplicate detection, password hashing, entity creation, and persistence.

### Where it is used

- `CreateUserCommand` carries the data for the operation.
- `ICreateUserHandler` defines the operation contract.
- `CreateUserCommandHandler` performs the operation.
- `UserService` delegates to the handler.

### Why it fits Connect

The operation has one clear place where its behavior can be read and changed. A future
feature can follow the same structure without making `UserEndpoints` responsible for all
business operations.

### Cost and alternative

This structure uses more classes than putting the logic directly in the endpoint. The
alternative is a small endpoint with inline logic, which may be acceptable for a trivial
API but becomes harder to maintain as the number of operations grows.

## 5. CQRS: partly implemented direction

Command Query Responsibility Segregation separates operations that change data from
operations that read data.

### Current status

The current API contains a create command, but it does not yet contain a complete query
side. Therefore, this document does not claim that CQRS is fully implemented.

`CreateUserCommand` and `CreateUserCommandHandler` demonstrate the command side. A future
read feature could introduce a separate query and query handler when the product requires
one.

### Why it may fit Connect

Reads and writes may eventually need different data shapes. For example, a public profile
read should not return private account fields. Separate query models could make that
boundary explicit.

### Cost and decision rule

CQRS creates more types and files. It should only be extended when a feature has a real
reason to separate its read and write behavior. Until then, the current command handler
is the evidence-backed structure and the query side remains a design direction.

## 6. Strategy: proposed for matching

Strategy is a proposed pattern for future matching rules. It is not implemented in the
current API.

### Problem

The matching requirements may change as the product learns which recommendations are
useful. Different matching rules could include shared interests, profile type, location,
or opportunity relevance.

### Proposed structure

A future matching service could depend on an `IMatchStrategy` interface. Individual
matching algorithms would implement that interface and could be selected without putting
all matching rules in one large service.

### Why it may fit Connect

The pattern would make a genuine variation in matching rules explicit. A new algorithm
could be added as a separate implementation and tested independently.

### Decision rule

Strategy should only be introduced when there are at least two meaningful matching
algorithms or a clear requirement to switch algorithms. With one fixed matching rule, a
normal service method is simpler and should be preferred.

## Evidence in the repository

- [Application entry point](../../../api/Connect/Program.cs)
- [Application dependency injection](../../../api/Connect.Application/DependencyInjection.cs)
- [Infrastructure dependency injection](../../../api/Connect.Infrastructure/DependencyInjection.cs)
- [User endpoint](../../../api/Connect.Presentation/Users/UserEndpoints.cs)
- [User service](../../../api/Connect.Application/Users/Services/UserService.cs)
- [Create-user command](../../../api/Connect.Application/Users/DTO/CreateUserCommand.cs)
- [Create-user handler contract](../../../api/Connect.Application/Users/Contracts/ICreateUserHandler.cs)
- [Create-user handler](../../../api/Connect.Infrastructure/Persistence/Users/CreateUserCommandHandler.cs)
- [Password hasher contract](../../../api/Connect.Application/Users/Contracts/IPasswordHasher.cs)
- [Password hasher implementation](../../../api/Connect.Infrastructure/Persistence/Users/PasswordHasher.cs)
- [Domain user entity](../../../api/Connect.Domain/Users/User.cs)

## Personal contribution

I selected the patterns that are supported by the current backend example, explained the
problem each one addresses, and documented the trade-offs against simpler alternatives.
The architecture and pattern choices were discussed with the team, while I led the
structure and documentation of this evidence.

The create-user flow is the main proof because it shows several patterns working together:
the layered structure separates responsibilities, dependency injection connects the
projects, interfaces hide concrete implementations, and the command handler gives the
use case a clear home.

## Portfolio mapping

**BoKSA topic:** Architecture and Design Patterns

**Evidence provided:**

- Patterns identified in the current backend code
- The problem and reason for each implemented pattern
- Alternatives and costs
- A clear distinction between implemented and proposed patterns
- Direct links to the source files that demonstrate the patterns

## AI use declaration

AI was used as a writing support tool while working on this document. I used AI to help
structure the explanation, improve wording and readability, and identify relevant
implementation evidence from the `api/` folder. The pattern selection, technical
interpretation, reasoning, and final decisions remain my responsibility. I reviewed the
document against the source code and decided which text to include.
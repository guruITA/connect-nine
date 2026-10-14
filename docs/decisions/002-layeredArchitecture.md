# 002 The structure of the server: four layer projects and CQRS by feature

**Status.** Decided by the whole team on 1 October 2026 (rule 8 adjusted to the code by Wesley Oerlemans on 8 October 2026), per `docs/team/openItems.md` and `docs/architecture/architecture.md`. Written down here on 8 October 2026. The whole team decided it together, and the part Agreed by records that.
**Issue.** 11 The team agrees on the layered architecture (#12), tasks 11.1 (#253) and 11.2 (#254).
**Who decided.** The whole team. Drafted by Wesley Oerlemans.

## Context

After January 2027 the students leave, and someone else must understand and change Connect. Four of the five students are new to .NET. So the server needs a structure a beginner can follow without breaking it by accident, and that a later developer can read on one page. The database is already decided: PostgreSQL 16 with EF Core (26 September 2026, `docs/architecture/techStack.md`). It is not reopened here.

### One real request through the four layers

Making an account, `POST /api/accounts`, the first slice (#14):

1. **Presentation** reads the form, checks it with FluentValidation and calls one handler. It knows nothing about the database.
2. **Application** holds the handler `CreateAccountHandler` with its `CreateAccountCommand`. It decides: is this email address already taken, then hash the password and save. It reaches the data only through the contract `IAppDbContext`.
3. **Domain** holds the account itself and its rules, for example what a valid email address is. It uses nothing else.
4. **Infrastructure** holds `ConnectDbContext`, the EF Core class that implements `IAppDbContext` and talks to PostgreSQL.

## Options

### Structure: four separate projects or one project with four folders

| | Four separate projects | One project with four folders |
|---|---|---|
| Files for one new feature | The same files, spread over the projects | The same files, in folders of one project |
| What a beginner must learn | Project references and five project files | Only folders |
| How a rule is unit tested without the database | A rule in Domain is tested in a project with no reference to Infrastructure or a database driver. In Connect that project is `Connect.Application.Tests/Domain`, because there is no separate `Connect.Domain.Tests` (task 3.1, #259) | There is only one project, so a test for a rule in the Domain folder sits in the same test project as Infrastructure, carrying a reference to the database driver whether that rule needs it or not |
| What catches a mistake, such as Domain using the database class | The compiler: a project without the reference does not build (proof below) | Only a reviewer who notices it. Microsoft names the risk of this gap: without project-level organisation "there's no clear indication of which classes in which folders should depend on which others", which "frequently leads to spaghetti code" (Microsoft, n.d.-a) |
| Source | Microsoft describes how applications grow from folders in one project into one project per layer, so layers "can enforce restrictions on which layers can communicate" (Microsoft, n.d.-a) | Microsoft names folders as the single project way (Microsoft, n.d.-a) |

### Data access: the repository pattern or CQRS by feature

| | The repository pattern: one repository per kind of data behind an interface | CQRS by feature: one handler per use case, data through `IAppDbContext` |
|---|---|---|
| Files for one new feature | One method on an existing repository plus a service that calls it | A command or query and a handler, in their own folder |
| What a beginner must learn | One shape | Two shapes, command and query |
| How a rule is unit tested without PostgreSQL | Stub the repository. The EF Core team names this its advice when a test double is really needed, with the warning that it "can incur significant cost to implement and maintain", because every query needs its own method (Microsoft, n.d.-b) | The handler gets an `ConnectDbContext` on SQLite in memory. The EF Core team allows this, but warns that "testing against SQLite does not guarantee the same results" as the real database (Microsoft, n.d.-b) |
| What catches a mistake | A reviewer: a repository or service can grow into one class that does everything (opinion) | The folder per use case keeps one use case in one place. A reviewer still has to spot a handler that grows too big (opinion) |
| Where it fits | Many screens that read and write the same data in the same way (CRUD). Microsoft calls this the traditional approach, "straightforward" and "suited for basic create, read, update, and delete (CRUD) operations" (Microsoft, n.d.-c) | Reads and writes that differ, for example a list of suggestions against saving a profile. Microsoft names the same split: read and write operations "often have different performance and scaling requirements" (Microsoft, n.d.-c) |

### What each option is for, and what the sources warn about

- **The repository pattern** is for hiding the database behind one interface per kind of data, so code above it can be tested without EF Core. That is its real strength (Microsoft, n.d.-b). The price is a method per query and a layer the team must keep in step, and EF Core's `DbContext` already does much of the work of a repository.
- **CQRS** is for separating the code that changes data from the code that only reads it. Fowler warns that "for most systems CQRS adds risky complexity", and that many systems fit a simple read and write model better (Fowler, 2011). Microsoft says the same: it "might not be suitable when the domain or the business rules are simple" (Microsoft, n.d.-c). Both warnings are mostly about the heavy form, with a separate read database, messaging and data that is only up to date later.
- **What Connect actually uses is the light form.** One database, one handler per use case, no separate read database, no messaging and no extra library. Microsoft describes this as simplified CQRS: "a single data source or database, but two logical models" (Microsoft, n.d.-d). Bogard calls the matching way of organising code vertical slices: code grouped per feature, with as few shared layers such as repositories and services as possible (Bogard, 2018).
- **The risk that stays.** Bogard adds that vertical slices assume a team that recognises when a handler does too much and knows how to refactor (Bogard, 2018). Four of the five students are new to .NET, so that is the real risk for Connect, not the heavy CQRS that Fowler warns about. The review checks it: one handler does one use case.
- **Testing.** The EF Core team's first advice is to test against the real database, and to use a test double only where that is not possible (Microsoft, n.d.-b). SQLite is not PostgreSQL: Connect uses citext, uuid ids, unique indexes and ExecuteDeleteAsync, which SQLite handles differently or not at all. So a handler is unit tested with SQLite in memory for its logic, and every rule that depends on PostgreSQL itself gets a database test against a real PostgreSQL (`<Feature>DatabaseTests`, the postgres service of the pipeline of #266).

**What the sources changed.** The team assumed that CQRS makes testing easier than a repository. The EF Core documentation does not support that: it advises the real database first and a repository second, and calls SQLite a fake with limits. Testing is no reason for CQRS. The reasons that stay are the split per feature and the fact that Connect uses the light form, which avoids most of the complexity the sources warn about.

**Recommendation in two lines.** Four projects, because the compiler guards the layers for a team that is new to .NET. CQRS by feature in its light form, because the code splits per use case, with more files per feature and a reviewer watching for handlers that grow too big as the risks the team accepts.

### Preference of each team member

One line per team member: the option they prefer for each choice, and why.

- Wesley Oerlemans: four separate projects and CQRS by feature, because the compiler then stops a wrong reference between the layers, and each use case keeps its own folder instead of one class that keeps growing.

## Decision

Four layer projects and CQRS by feature, decided by the team on 1 October 2026, over one project with four folders and over the repository pattern. Per `docs/architecture/architecture.md`, the reason is that the code splits by feature instead of by one class per kind of data.

The structure, as the code on dev and task 3.1 (#259) have it, and as `docs/team/codeConventions.md` backend rules 7 and 8 now state it:

- Four layer projects, `api/Connect.Presentation`, `api/Connect.Application`, `api/Connect.Domain` and `api/Connect.Infrastructure`, and the start project `api/Connect`, which starts the server and connects the layers.
- References point inward: Connect uses Presentation and Infrastructure. Presentation uses Application. Infrastructure uses Application, because it implements contracts of Application such as `IAppDbContext`. Application uses Domain. Domain uses nothing.
- Test projects per layer: `Connect.Application.Tests`, `Connect.Infrastructure.Tests` and `Connect.Presentation.Tests`, each using only the project it tests, plus `Connect.Tests` for tests across projects. There is no `Connect.Domain.Tests` (task 3.1, #259), so a rule in Domain is tested without any database in `Connect.Application.Tests/Domain`.
- One folder per use case in `api/Connect.Application/Features/<Feature>/<UseCase>/`, with its command or query and its handler. The handler reaches the data through `IAppDbContext` in `api/Connect.Application/Abstractions`, implemented by `ConnectDbContext` in Infrastructure. No repository classes, no `IXRepository` contracts, no Repositories folder, no use case written as a Service class.
- A handler is unit tested with an in-memory SQLite `ConnectDbContext` in `Connect.Application.Tests`. A rule that depends on PostgreSQL is tested against a real PostgreSQL in `Connect.Infrastructure.Tests` (`<Feature>DatabaseTests`). A rule in Domain is tested without any database.

### Proof that the compiler guards the layers

Run on 8 October 2026 at 23:13 with the .NET SDK 10.0.401, on a throwaway copy of `api` from dev, in a folder outside the repository that was deleted afterwards. The same line was added to two projects:

```csharp
typeof(Connect.Infrastructure.Persistence.ConnectDbContext).Name
```

Domain, which has no reference to Infrastructure:

```
Connect.Domain/LayerProbe.cs(5,51): error CS0234: The type or namespace name 'Infrastructure' does not exist in the namespace 'Connect' (are you missing an assembly reference?) [Connect.Domain/Connect.Domain.csproj]
Build FAILED.
    1 Error(s)
```

The project Connect, which references Infrastructure:

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Rule 8 follows the code

The first text of rule 8 said Infrastructure uses only Domain and Presentation uses Infrastructure. The code on dev, the open merge request !22 and task 3.1 (#259) all have Infrastructure using Application and a start project Connect that connects Presentation and Infrastructure. Microsoft's Clean Architecture does the same: Infrastructure implements contracts of the core, "and so Infrastructure should have a reference to the Application Core project" (Microsoft, n.d.-a). Wesley Oerlemans decided on 8 October 2026 that the code leads here, so rule 8 now follows it.

### Nick Hoebe's create user example

The example on dev (merge request !11) puts the handler `CreateUserCommandHandler` in Infrastructure, using `ConnectDbContext` directly, with a contract `ICreateUserHandler` and a `UserService` in Application in front of it. It was written before the decision of 1 October. Two parts of it stay: the start project and the registration per layer (`AddApplication`, `AddInfrastructure`, `AddPresentation`). Two parts do not fit the decision: a use case as a Service class, and a handler outside Application. Merge request !22 replaces the example with the empty structure, and the first real feature (#14, tasks 3.3 and 3.4) builds in the shape above.

## Consequences

- **For the team new to .NET.** More files per feature and two shapes to learn. The compiler refuses a wrong reference, so a beginner cannot break the layers without noticing.
- **For the maintainer after January 2027.** Every use case is one folder with a name that says what it does, so a change starts in one place.
- **For the foundation.** The consequence weighed heaviest is a platform that is switched off because nobody can change it. A structure the compiler checks lowers that risk more than a rule only a reviewer checks.
- **One project with four folders.** The Structure table shows the difference that decided it: a wrong reference, for example Domain using the database class, is only caught by a reviewer, while four separate projects let the compiler refuse the build. For a team new to .NET that compiler check outweighed having only folders to learn.
- **The repository pattern.** The Data access table shows a repository or service can grow into one class that does everything, with a reviewer as the only check, against the folder per use case of CQRS. It lost because the code splits by feature instead of by one class per kind of data, as the Decision states.
- Task 11.4 (#1465) rewords the tasks of every next sprint that still use the repository or service wording.
- The Definition of Done line on the layers and the slice template say the same as this file: a handler reaches the data only through `IAppDbContext`.
- `docs/team/codeConventions.md` links this file under backend rules 7 and 8.

## Agreed by

| Name | Date agreed | Remark |
|---|---|---|
| Bayon Eerhart | 1 October 2026 | Decided by the whole team |
| Nick Hoebe | 1 October 2026 | Decided by the whole team |
| Wesley Oerlemans | 1 October 2026 | Decided by the whole team |
| Gurpreet Singh | 1 October 2026 | Decided by the whole team |
| Kelvin van Voorst | 1 October 2026 | Decided by the whole team |

## References

Bogard, J. (2018, April 19). *Vertical slice architecture*. Retrieved October 8, 2026, from https://www.jimmybogard.com/vertical-slice-architecture/

Fowler, M. (2011, July 14). *CQRS*. Retrieved October 8, 2026, from https://martinfowler.com/bliki/CQRS.html

Microsoft. (n.d.-a). *Common web application architectures*. Microsoft Learn. Retrieved October 8, 2026, from https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

Microsoft. (n.d.-b). *Choosing a testing strategy*. EF Core, Microsoft Learn. Retrieved October 8, 2026, from https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy

Microsoft. (n.d.-c). *CQRS pattern*. Azure Architecture Center, Microsoft Learn. Retrieved October 8, 2026, from https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs

Microsoft. (n.d.-d). *Applying simplified CQRS and DDD patterns in a microservice*. .NET Microservices Architecture, Microsoft Learn. Retrieved October 8, 2026, from https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/apply-simplified-microservice-cqrs-ddd-patterns

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

# 002 The structure of the server: four layer projects and CQRS by feature

**Status.** Decided by the team on 1 October 2026, per `docs/team/openItems.md` and `docs/architecture/architecture.md`. Written down here on 8 October 2026. Who was there is not recorded in the team documents, so the part Agreed by is still open.
**Issue.** 11 The team agrees on the layered architecture (#12), tasks 11.1 (#253) and 11.2 (#254).
**Who decided.** The whole team. Drafted by Wesley Oerlemans.

## Context

After January 2027 the students leave, and someone else must understand and change Connect. Four of the five students are new to .NET. So the server needs a structure a beginner can follow without breaking it by accident, and that a later developer can read on one page. The database is already decided: PostgreSQL 16 with EF Core (26 September 2026, `docs/architecture/techStack.md`). It is not reopened here.

### One real request through the four layers

Making an account, `POST /api/accounts`, the first slice (#14):

1. **Presentation** reads the form, checks it with FluentValidation and calls one handler. It knows nothing about the database.
2. **Application** holds the handler `CreateAccountHandler` with its `CreateAccountCommand`. It decides: is this email address already taken, then hash the password and save. It reaches the data only through the contract `IAppDbContext`.
3. **Domain** holds the account itself and its rules, for example what a valid email address is. It uses nothing else.
4. **Infrastructure** holds `AppDbContext`, the EF Core class that implements `IAppDbContext` and talks to PostgreSQL.

## Options

### Structure: four separate projects or one project with four folders

| | Four separate projects | One project with four folders |
|---|---|---|
| Files for one new feature | The same files, spread over the projects | The same files, in folders of one project |
| What a beginner must learn | Project references and five project files | Only folders |
| What catches a mistake, such as Domain using the database class | The compiler: a project without the reference does not build (proof below) | Only a reviewer who notices it |
| Source | Microsoft describes how applications grow from folders in one project into one project per layer, so layers "can enforce restrictions on which layers can communicate" (Microsoft, n.d.-a) | Microsoft names folders as the single project way (Microsoft, n.d.-a) |

### Data access: the repository pattern or CQRS by feature

| | The repository pattern: one repository per kind of data behind an interface | CQRS by feature: one handler per use case, data through `IAppDbContext` |
|---|---|---|
| Files for one new feature | One method on an existing repository plus a service | A command or query and a handler, in their own folder |
| What a beginner must learn | One shape | Two shapes, command and query |
| How a rule is unit tested without the database | Stub the repository. Microsoft calls this the recommended way to test without the production database (Microsoft, n.d.-b) | The handler gets an `AppDbContext` on SQLite in memory. Microsoft names this as the other way, and strongly advises against the EF Core in-memory provider (Microsoft, n.d.-b) |
| What catches a mistake | A reviewer, a repository can grow into a class that does everything | The folder per use case keeps a use case in one place. Opinion, no source |
| Fit with Connect | Fits simple read and write screens. Microsoft says CQRS "might not be suitable when the domain or the business rules are simple" (Microsoft, n.d.-c) | Fits when reads and writes differ, as in matching and the event places later. Opinion, no source |

**What the sources changed.** The team assumed that CQRS makes testing easier than a repository. The EF Core documentation says the opposite: the repository pattern is its recommended way to test without the database, and SQLite in memory is the alternative. Both ways test a rule without PostgreSQL, so this is no reason against CQRS, but also no reason for it.

**Recommendation in two lines.** Four projects, because the compiler guards the layers for a team that is new to .NET. CQRS by feature, because the code then splits by feature, with the extra files per feature as the risk the team accepts.

## Decision

Four layer projects and CQRS by feature, decided by the team on 1 October 2026, over one project with four folders and over the repository pattern. Per `docs/architecture/architecture.md`, the reason is that the code splits by feature instead of by one class per kind of data.

The structure, as `docs/team/codeConventions.md` backend rules 7 and 8 state it:

- Presentation, Application, Domain and Infrastructure are four projects, plus the test project Connect.Tests.
- Domain uses nothing. Application uses Domain. Infrastructure uses Domain. Presentation uses Application and Infrastructure. Connect.Tests uses all four.
- One folder per use case in Application, with its command or query and its handler. No repository classes, no `IXRepository` contracts, no Repositories folder.
- A handler is unit tested with an in-memory SQLite `AppDbContext`, a rule in Domain without any database.

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

### Open point: the code and rule 8 differ

The code on dev on 8 October 2026 has a fifth project, `api/Connect`, which starts the server and references the four layers. Infrastructure references Application, not only Domain, and Presentation references only Application. Microsoft's Clean Architecture does the same: Infrastructure implements contracts of the core, "and so Infrastructure should have a reference to the Application Core project" (Microsoft, n.d.-a). `IAppDbContext` lives in Application, so `AppDbContext` in Infrastructure needs that reference.

- TODO the team: choose whether rule 8 changes to the code (a start project Connect, Infrastructure uses Application), or the code changes to rule 8. The owner of #259 (merge request !22) builds the projects.

## Consequences

- **For the team new to .NET.** More files per feature and two shapes to learn. The compiler refuses a wrong reference, so a beginner cannot break the layers without noticing.
- **For the maintainer after January 2027.** Every use case is one folder with a name that says what it does, so a change starts in one place.
- **For the foundation.** The consequence weighed heaviest is a platform that is switched off because nobody can change it. A structure the compiler checks lowers that risk more than a rule only a reviewer checks.
- Task 11.4 (#1465) rewords the tasks of every next sprint that still use the repository wording.
- `docs/team/codeConventions.md` links this file under backend rules 7 and 8.

## Agreed by

| Name | Date agreed | Remark |
|---|---|---|
| Bayon Eerhart | | |
| Nick Hoebe | | |
| Wesley Oerlemans | | |
| Gurpreet Singh | | |
| Kelvin van Voorst | | |

## References

Microsoft. (n.d.-a). *Common web application architectures*. Microsoft Learn. Retrieved October 8, 2026, from https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

Microsoft. (n.d.-b). *Testing without your production database system*. EF Core, Microsoft Learn. Retrieved October 8, 2026, from https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database

Microsoft. (n.d.-c). *CQRS pattern*. Azure Architecture Center, Microsoft Learn. Retrieved October 8, 2026, from https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

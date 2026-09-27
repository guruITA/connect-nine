# Stack research, Young Amstelland Connect

This document serves the HvA level 3 criterion Analysis & Advice (AA).

It holds the research behind the stack decision recorded in `docs/architecture/techStack.md`:
React for the frontend, .NET with ASP.NET Core and C# for the backend, and PostgreSQL 16 as the
database. It reads BoKSA plus the official documentation for each technology and weighs it against
the platform requirements in `docs/architecture/techStack.md` and `docs/project/productVision.md`:
a community and talent platform with profiles, matching, events, and an admin dashboard, a user
base that includes minors, no license cost for the foundation (NFR3 Hosting at no license cost),
and a five-person team.

The research was done on 22 September 2026, when the database was still Microsoft SQL Server. The
section on SQL Server is kept as the research behind that first choice. On 26 September 2026 the
database became PostgreSQL 16 with EF Core and Npgsql, and the section on PostgreSQL says what is
known about it from the documents and the board, and what research is still to do.

## React

**What it is.** React is a JavaScript library, not a full application framework. Its own
documentation describes apps as built from small, reusable components, each "a piece of the UI
(user interface) that has its own logic and appearance," composed into a tree with data passed down
through props and local state kept where it belongs (React, n.d.-b). BoKSA's Component Architecture
& Framework Basics article, part of the Designing & Realising track every Full-Stack Developer
profile student works through, names React alongside Vue as the two component-based frameworks
worth learning well (HBO-ICT, n.d.-b).

**Documented strengths.** BoKSA states that picking up one component-based framework well teaches
ideas that "transfer directly to any other component-based framework you meet later" (HBO-ICT,
n.d.-b), which is the point of the exercise regardless of which one a team picks. The same article
treats reading the official docs directly as part of the skill, since "frameworks evolve quickly"
(HBO-ICT, n.d.-b). Client-side routing (React Router) and a large ecosystem of maintained libraries
are both named in BoKSA's key points for this article.

**Documented weaknesses.** React's own guidance is that a team should not build on the bare library
alone: "If you want to build a new app or website with React, we recommend starting with a
framework" such as Next.js or React Router, because React by itself does not include routing, data
fetching, or build tooling (React, n.d.-a). A team choosing React takes on a second layer of tooling
decisions on top of learning components, props, and state.

**For this project, good and bad.** Good: the profile pages, the matching screen, the event
listings, and the admin dashboard are naturally component-shaped work, and building them in React is
also the assessed BoKSA skill for the Designing & Realising track, so the choice serves the
coursework and the product at once. Bad: most of the team does not know React yet, so the extra
tooling choices cost early sprint velocity. The board has now made those choices in issue 03 Create
an account, task 3.5: Vite as the build tool, React Router for the pages, TypeScript in strict mode,
CSS Modules, ESLint and Prettier, Vitest with React Testing Library, and one shared helper
`frontend/src/api/client.ts` for every call to the server. Neither React's documentation nor
BoKSA's article addresses the platform's security and privacy weight directly. How user-submitted
content is shown safely is checked by the Security Specialist on top of these sources, through the
security documents in `docs/security` (issues 12, 13, 18 and 19 of the epic Security analysis).

## .NET with ASP.NET Core and C#

**What it is.** ASP.NET Core is, in Microsoft's own words, "a cross-platform, high-performance,
open-source framework for building modern web apps using .NET," built for large-scale development
and able to "handle any size workload" (Microsoft, n.d.-d). C# is .NET's primary language,
documented separately with its own language reference and tour (Microsoft, n.d.-a).

**Documented strengths.** The ASP.NET Core overview lists integrated dependency injection,
environment-based configuration, Kestrel as a cross-platform HTTP server, Minimal APIs for building
REST endpoints with little boilerplate, SignalR for real-time features, and built-in security
features for authentication as key parts of the framework (Microsoft, n.d.-d). The same page names
React among the client-side frameworks ASP.NET Core works with, which is a documented pairing for
exactly this stack (Microsoft, n.d.-d). BoKSA's Language & Runtime article, part of the mandatory
foundational knowledge every profile student covers, frames understanding a language's runtime,
compiler versus interpreter versus JIT, and stack, heap, and garbage collection as knowledge that
helps explain a whole class of performance issues before debugging one (HBO-ICT, n.d.-e), which
applies directly to C#'s own JIT-compiled, garbage-collected runtime.

**Documented weaknesses.** The ASP.NET Core overview page fetched for this research carried its own
note that the version shown was not the latest and that the release referenced was already out of
support. That is a real property of how fast Microsoft moves through versions. The board answers it
with .NET 10, the long term support version, supported until November 2028, while .NET 8 stops
getting security fixes on 10 November 2026 (issue 11 The team agrees on the layered architecture,
task 11.3, and `docs/team/codeConventions.md`, backend rule 1). C#'s own documentation spans a wide
tour of language concepts, type system, object-oriented programming, pattern matching, exceptions,
and asynchronous programming with LINQ (Microsoft, n.d.-a), which is more upfront surface than a
smaller, single-paradigm language.

**For this project, good and bad.** Good: built-in dependency injection, configuration, and
authentication patterns line up with the platform's weight toward security and privacy for a user
base that includes minors, and with the Authentication & Authorization Basics article (HBO-ICT,
n.d.-a) and the Input Validation & Secure Coding article (HBO-ICT, n.d.-d) the whole team covers
regardless of stack. Minimal APIs fit the community interaction, matching, and events modules well,
and cross-platform support means the team runs the server in Linux containers on the foundation's
server.

SignalR is a documented strength of the framework, but not a fit for this team. It needs its own
persistent hub connection and, at production scale, a backplane to fan messages out across server
instances, which is infrastructure this stack does not otherwise need. A feature that reads as
real-time, a badge notification or a new match, is served the same way as the rest of the platform:
an HTTP endpoint the frontend asks on an interval, or a Server-Sent Events endpoint where waiting
actually matters, both plain Minimal API endpoints. Wesley Oerlemans decided on 24 September 2026
that the backend uses Minimal APIs and no SignalR. No issue on the board uses SignalR, and
`docs/team/codeConventions.md` makes Minimal APIs the rule (backend rule 9).

Bad: ASP.NET Core and C# are the part of the stack fewest team members know today, so the
platform's core logic sits behind a learning curve at the same time the Definition of Done (a
passing linter, no compile errors, passing tests before a merge counts as done, in
`docs/team/definitionOfDone.md`) applies from sprint 1. The board keeps the first steps small for
that reason: issue 11 walks one request through the four layers in a team moment before anyone
writes code.

## Microsoft SQL Server, the first choice

This section is the research behind the choice of 22 September 2026. It is kept because it
explains why the database changed.

**What it is.** SQL Server is Microsoft's relational database engine. Its 2022 release notes
describe it as a platform that "gives you choices of development languages, data types, on-premises
or cloud environments, and operating systems" (Microsoft, n.d.-g), with a technical documentation
set covering installation, security, performance tuning, and business intelligence tooling such as
SSMS, SSRS, SSAS, and SSIS (Microsoft, n.d.-f).

**Documented strengths.** The 2022 feature list spans analytics, availability, security,
performance, and platform integrations such as Azure Synapse Link and PolyBase data virtualization
(Microsoft, n.d.-g). BoKSA's Database Integration article, part of the Designing & Realising track,
lists the relational-database skillset every profile student is assessed on regardless of vendor:
subqueries, aggregate functions with GROUP BY and HAVING, transactions, locking, constraints, user
rights, and ORM use for CRUD (HBO-ICT, n.d.-c).

**Documented weaknesses.** Licensing is the clearest documented cost. Microsoft's own pricing page
lists SQL Server 2022 Enterprise edition at $15,123 per two-core pack and Standard edition at $3,945
per two-core pack or $989 per server, with an Enterprise Software Assurance subscription at $5,434
per year (Microsoft, n.d.-e). Two editions are free: Developer, which "includes all the
functionality of Enterprise edition, but is licensed for use as a development and test system, not
as a production server," and Express, described as "the entry-level, free database, ideal for
learning and building desktop and small server data-driven applications" (Microsoft, n.d.-c).
Microsoft's own compute-capacity documentation caps Express at "the lesser of 1 socket or 4 cores"
(Microsoft, n.d.-b), a real ceiling if Express were the one running in production.

**What this meant for the project.** The only free edition documented as suitable for serving a
small application in production is Express, with its ceiling, while every edition without it costs
four figures at minimum. That was the tension with NFR3 Hosting at no license cost that the stack
documents recorded from the start.

## PostgreSQL 16, the database since 26 September 2026

**What the board fixes.** Every environment runs PostgreSQL 16: the official image `postgres:16` in
docker compose on a laptop (issue 03 task 3.2), the service `database` on the test site in
`deploy/docker-compose.test.yml`, its own PostgreSQL 16 in the pipeline for the integration tests
(issue 03 task 3.8), and the StatefulSet `connect-database` with one pinned PostgreSQL 16 tag in
production (issue 95 Connect and its database described in Terraform). The server reaches it only
through EF Core with the provider Npgsql, and the package EFCore.NamingConventions turns C# names
into snake_case table names (issue 03 task 3.3). Email addresses use the type citext, so upper and
lower case count as the same address (`docs/team/codeConventions.md`, database rule 8).

**Why it fits, from the documents.** The stack documents of 23 and 24 September 2026 already
weighed PostgreSQL against SQL Server. They recorded that PostgreSQL has no paid edition, so the
licensing tension of the section above does not arise, and that it runs as a plain container image
with no edition to choose, which is lighter on the foundation's server. BoKSA's Database Integration checklist (transactions, constraints, user
rights, parameterized ORM access) applies to PostgreSQL as it does to any relational database
(HBO-ICT, n.d.-c), so the assessed database skill carries over. Wesley Oerlemans and Nick Hoebe have
used PostgreSQL before (`docs/architecture/techStack.md`). Because EF Core was the data access layer
from 24 September 2026 and no code existed yet, the change of engine touched no code.

**Weaknesses and what is still to check.** The course context pointed to SQL Server, and Nick Hoebe
has used SQL Server most directly. For PostgreSQL this research has not yet read the official
documentation at its source the way it did for SQL Server. Two facts are still to be checked there
and added to the references below: the license of PostgreSQL for use by the foundation, and the
support end date of PostgreSQL 16. Issue 92 k3s and Terraform for production, compose for test
checks that every tool of production is free to use for the foundation, with a link and a date per
tool, and PostgreSQL belongs in that list.

## Research the board still does

The board turns the open stack questions into issues that measure or compare before they decide.
Each writes its result in `docs/decisions` or `docs/research`:

- Issue 92, task 92.1: docker compose, k3s and MicroK8s compared on what the DevOps workshop asks,
  and the license of Terraform and of OpenTofu checked at the source.
- Issue 88 The team picks the code quality tool: SonarQube Community Build, SonarQube Cloud, and
  GitLab Code Quality with the .NET analyzers, compared on cost, the HvA GitLab, memory and
  blocking.
- Issue 98 The server counts its requests and errors, task 98.1: prometheus-net.AspNetCore against
  the OpenTelemetry Prometheus exporter.
- Issue 101 All log lines of Connect in one place, task 101.2: Grafana Alloy against Promtail, from
  the current Grafana documentation.
- Issue 108 Two ways of paging the showcases, measured: paging by page number against paging by
  cursor.
- Issue 110 The cost of password hashing, measured: the strength of the password hash against the
  time a login takes.
- Issue 111 Two end to end test tools compared on one flow: Playwright against Cypress.

## What the stack means for this team

The stack decision recorded in `docs/architecture/techStack.md` was confirmed by Wesley Oerlemans on
22 September 2026, and the reasoning behind it, also from Wesley on 22 September 2026, is this: the
main goal of choosing React and .NET is learning, not the fastest possible route to a working
platform. Most of the team does not know React or .NET yet. Wesley's next internship stage is in
React and .NET, so building this platform with them now serves his personal development directly.
Nick Hoebe already knows .NET and wants to expand his knowledge of it further, which gives the team
one person who can unblock the other four when they get stuck on the backend. Gurpreet Singh
already has basic React knowledge, which gives the frontend the same kind of starting point.

That is a real trade-off, not a free choice. Every strength and weakness above that touches the
team's unfamiliarity with React and .NET points the same way: an unfamiliar stack costs velocity, in
research time before feature work, in code review that catches basic framework mistakes rather than
design issues, and in a Definition of Done that does not get easier because the framework is new.
The production tools add to it: k3s, Terraform, Prometheus, Grafana and Loki are new to the whole
team (issue 93 k3s on the production server), so the board keeps production small and puts it in
sprint 4, after the product itself runs on the test site. Against that sits the learning gain the
team is choosing on purpose: three of five members building fluency in a stack that at least one of
them needs after this semester, with Nick Hoebe's .NET knowledge as the team's one point of help
against the velocity cost. Whether that trade holds up in practice is for the team to check at the
sprint retrospectives, not something this research can settle in advance.

## Whether this research used live web access

Yes. BoKSA and the official React, ASP.NET Core, C#, and SQL Server documentation listed below were
read directly for this research, on 22 September 2026. The facts on PostgreSQL come from the board
and from the earlier stack documents. Their official sources are still to be read, as the section
on PostgreSQL says.

## References

HBO-ICT. (n.d.-a). *Authentication & authorization basics*. BoKSA. Retrieved September 22,
2026, from https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/design-and-realise/authentication-and-authorization-basics/

HBO-ICT. (n.d.-b). *Component architecture & framework basics*. BoKSA. Retrieved September
22, 2026, from https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/design-and-realise/component-architecture-and-frameworks/

HBO-ICT. (n.d.-c). *Database integration*. BoKSA. Retrieved September 22, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/design-and-realise/database-integration/

HBO-ICT. (n.d.-d). *Input validation & secure coding*. BoKSA. Retrieved September 22, 2026,
from https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/design-and-realise/input-validation-and-secure-coding/

HBO-ICT. (n.d.-e). *Language & runtime*. BoKSA. Retrieved September 22, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/design-and-realise/language-and-runtime/

Microsoft. (n.d.-a). *C# documentation*. Microsoft Learn. Retrieved September 22, 2026, from
https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/

Microsoft. (n.d.-b). *Compute capacity limits by edition of SQL Server*. Microsoft Learn.
Retrieved September 22, 2026, from https://learn.microsoft.com/en-us/sql/sql-server/compute-capacity-limits-by-edition-of-sql-server

Microsoft. (n.d.-c). *Editions and supported features of SQL Server 2022*. Microsoft Learn.
Retrieved September 22, 2026, from https://learn.microsoft.com/en-us/sql/sql-server/editions-and-components-of-sql-server-2022

Microsoft. (n.d.-d). *Overview of ASP.NET Core*. Microsoft Learn. Retrieved September 22,
2026, from https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core

Microsoft. (n.d.-e). *SQL Server 2022 pricing and licensing*. Retrieved September 22, 2026,
from https://www.microsoft.com/en-us/sql-server/sql-server-2022-pricing

Microsoft. (n.d.-f). *SQL Server technical documentation*. Microsoft Learn. Retrieved
September 22, 2026, from https://learn.microsoft.com/en-us/sql/sql-server/sql-server-technical-documentation

Microsoft. (n.d.-g). *What's new in SQL Server 2022*. Microsoft Learn. Retrieved September
22, 2026, from https://learn.microsoft.com/en-us/sql/sql-server/what-s-new-in-sql-server-2022

React. (n.d.-a). *Creating a React app*. Retrieved September 22, 2026, from
https://react.dev/learn/creating-a-react-app

React. (n.d.-b). *Quick start*. Retrieved September 22, 2026, from https://react.dev/learn

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to draft
the formulation of sentences, the structure of wording, readability, and clarity from
content that we had provided. The content itself, including our experiences, reflections,
learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the
drafts and decided as a team which text to include.

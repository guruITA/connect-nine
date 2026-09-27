# Stack research, Young Amstelland Connect

This document serves the HvA level 3 criterion Analysis & Advice (AA).

Research behind the stack decision recorded in `techStack.md`: React for the frontend,
.NET with ASP.NET Core and C# for the backend, Microsoft SQL Server as the database's main
goal. This document reads BoKSA plus the official documentation for each technology and
weighs it against the platform requirements set out in `techStack.md` and `productVision.md`:
a community and talent platform with profiles, matching, events, and an admin dashboard,
a user base that includes minors, the team's own hosting on a student budget, and a
five-person team.

## React

**What it is.** React is a JavaScript library, not a full application framework. Its own
documentation describes apps as built from small, reusable components, each "a piece of the
UI (user interface) that has its own logic and appearance," composed into a tree with data
passed down through props and local state kept where it belongs (React, n.d.-b). BoKSA's
Component Architecture & Framework Basics article, part of the Designing & Realising track
every Full-Stack Developer profile student works through, names React alongside Vue as the
two component-based frameworks worth learning well (HBO-ICT, n.d.-b).

**Documented strengths.** BoKSA states that picking up one component-based framework well
teaches ideas that "transfer directly to any other component-based framework you meet later"
(HBO-ICT, n.d.-b), which is the point of the exercise regardless of which one a team picks.
React's own documentation treats reading the official docs directly as part of the skill,
since "frameworks evolve quickly" (HBO-ICT, n.d.-b). Client-side routing (React Router) and
a large ecosystem of maintained libraries are both named in BoKSA's key points for this
article.

**Documented weaknesses.** React's own guidance is that a team should not build on the bare
library alone: "If you want to build a new app or website with React, we recommend starting
with a framework" such as Next.js or React Router, because React by itself does not include
routing, data fetching, or build tooling (React, n.d.-a). That means a team choosing React
takes on a second layer of tooling decisions the library itself does not make, on top of
learning components, props, and state.

**For this project, good and bad.** Good: the platform's profile pages, matching interface,
event listings, and admin dashboard are naturally component-shaped work, and building them in
React is also the assessed BoKSA skill for the Designing & Realising track, so the choice
serves the coursework and the product at once. Bad: because most of the team does not know
React yet (see below), the extra layer of framework and tooling choices that React's own docs
say a new project needs is exactly the kind of decision a five-person team without React
experience has to research and agree on before writing feature code, at a cost to early
sprint velocity. Neither React's documentation nor BoKSA's article addresses the platform's
security and privacy weight directly, since profiles and community interaction hold data for
minors, so verifying how the team's chosen router and rendering pattern handle
user-submitted content is work the Security Specialist still has to do on top of what these
sources cover.

## .NET with ASP.NET Core and C#

**What it is.** ASP.NET Core is, in Microsoft's own words, "a cross-platform,
high-performance, open-source framework for building modern web apps using .NET," built for
large-scale development and able to "handle any size workload" (Microsoft, n.d.-d). C# is
.NET's primary language, documented separately with its own language reference and tour
(Microsoft, n.d.-a).

**Documented strengths.** The ASP.NET Core overview lists integrated dependency injection,
environment-based configuration, Kestrel as a cross-platform HTTP server, Minimal APIs for
building REST endpoints with little boilerplate, SignalR for real-time features, and built-in
security features for authentication as key parts of the framework (Microsoft, n.d.-d). The
same page states ASP.NET Core "integrates seamlessly" with client-side frameworks including
React by name, which is a documented pairing for exactly this stack (Microsoft, n.d.-d).
BoKSA's Language & Runtime article, part of the mandatory foundational knowledge every profile
student covers, frames understanding a language's runtime, compiler versus interpreter versus
JIT, and stack, heap, and garbage collection as knowledge that helps explain a whole class of
performance issues before debugging one (HBO-ICT, n.d.-e), which applies directly to C#'s own
JIT-compiled, garbage-collected runtime.

**Documented weaknesses.** The ASP.NET Core overview page fetched for this research carried
its own note that the version shown was not the latest and that the specific release referenced
was already out of support, which is a real property of how fast Microsoft version-cycles this
documentation, not just an artifact of this research. C#'s own documentation spans a wide "tour"
of language concepts, type system, object-oriented programming, pattern matching, exceptions,
and asynchronous programming with LINQ (Microsoft, n.d.-a), which is more upfront surface than a
smaller, single-paradigm language.

**For this project, good and bad.** Good: built-in dependency injection, configuration, and
authentication patterns line up with the platform's non-functional weight toward security and
privacy for a user base that includes minors, and they line up with the Authentication &
Authorization Basics article (HBO-ICT, n.d.-a) and the Input Validation & Secure Coding
article (HBO-ICT, n.d.-d) the whole team already has to cover regardless of stack. Minimal
APIs fit the community interaction, matching, and events modules well, and cross-platform
support means the team is not locked into Windows-only hosting on a student budget. SignalR is
listed as a documented strength of the framework above, but is not a fit for a team keeping
the backend to Minimal APIs: SignalR needs its own persistent hub connection and, at
production scale, a backplane to fan messages out across server instances, which is
infrastructure this stack does not otherwise need. A feature that reads as real-time, a badge
notification or a new match, is served the same way the rest of the platform is, an HTTP
endpoint the frontend polls on an interval, or a Server-Sent Events endpoint for the cases
where polling latency actually matters, both a plain Minimal API endpoint rather than a second
connection type. Wesley Oerlemans decided on 2026-09-24 that the backend uses Minimal APIs, no
SignalR, for the team to confirm at sprint 1 planning, week of 21 September 2026.

Bad: ASP.NET Core
and C# are the part of the stack fewest team members know today, so the platform's core logic,
matching, the admin dashboard, events, sits behind a learning curve at the same time the team's
Definition of Done (a passing linter, no compile errors, passing unit tests before a merge
counts as done, per `techStack.md`) applies from sprint 1.

## Microsoft SQL Server

**What it is.** SQL Server is Microsoft's relational database engine. Its 2022 release notes
describe it as a platform that "gives you choices of development languages, data types,
on-premises or cloud environments, and operating systems" (Microsoft, n.d.-g), with a
technical documentation set covering installation, security, performance tuning, and
business intelligence tooling such as SSMS, SSRS, SSAS, and SSIS (Microsoft, n.d.-f).

**Documented strengths.** The 2022 feature list spans analytics, availability, security,
performance, and platform integrations such as Azure Synapse Link and PolyBase data
virtualization (Microsoft, n.d.-g). BoKSA's Database Integration article, part of the
Designing & Realising track, lists the relational-database skillset every profile student is
assessed on regardless of vendor: subqueries, aggregate functions with GROUP BY and HAVING,
transactions, locking, constraints, user rights, and ORM use for CRUD (HBO-ICT, n.d.-c). SQL
Server's own documentation is written directly against that same feature set, so choosing it
does not add scope beyond what any relational database would ask of the team.

**Documented weaknesses.** Licensing is the clearest documented cost. Microsoft's own pricing
page lists SQL Server 2022 Enterprise edition at $15,123 per two-core pack and Standard
edition at $3,945 per two-core pack or $989 per server, with an Enterprise Software Assurance
subscription at $5,434 per year (Microsoft, n.d.-e). Two editions are free: Developer, which
"includes all the functionality of Enterprise edition, but is licensed for use as a
development and test system, not as a production server," and Express, described as "the
entry-level, free database, ideal for learning and building desktop and small server
data-driven applications" (Microsoft, n.d.-c). Microsoft's own compute-capacity documentation
caps Express at "the lesser of 1 socket or 4 cores" (Microsoft, n.d.-b), a real ceiling if
Express were the one running in production.

**For this project, good and bad.** Good: Developer edition is free and carries the full
Enterprise feature set for building and testing, and BoKSA's Database Integration checklist
(transactions, constraints, user rights, parameterized ORM access) applies to whatever edition
the team runs, so the team's assessed database skill carries over now that SQL Server is
decided. Bad: the only free edition Microsoft documents as suitable for actually serving a
small application is Express, and it is capped at one socket or four cores, while every
edition without that ceiling costs four figures at minimum. That is a real tension with
hosting the platform on a student budget, and it is the cost `techStack.md` records against
the decision, alongside team familiarity.

PostgreSQL is worth weighing alongside this, not as a separate stack decision but as an open
question on the same database layer. `techStack.md` records Entity Framework Core as the
decided data access layer, and EF Core's provider abstraction keeps SQL Server and PostgreSQL
both reachable behind the same `DbContext`, so choosing SQL Server now does not close off
PostgreSQL later with much rework. PostgreSQL carries none of the licensing tension this
section documents above, since it has no paid edition and no compute-capacity ceiling to weigh
against a student budget, which also makes it lighter to run as a plain container on whatever
host the team picks, covered in `infrastructure.md`. Against that sits team familiarity: SQL
Server is the stack the course context and this decision point to, and Nick Hoebe, the team's
most experienced .NET developer, has used SQL Server directly and through ORMs, while
PostgreSQL is comparatively untested by the team on this project. This tradeoff is open,
owner: the whole team with the client, due sprint 1 planning, week of 21 September 2026.

## What the stack means for this team

The stack decision recorded in `techStack.md` was confirmed by Wesley Oerlemans on 2026-09-22,
and the reasoning behind it, also from Wesley on 2026-09-22, is this: the main goal of
choosing React and .NET is learning, not the fastest possible route to a working platform.
Most of the team does not know React or .NET yet. Wesley's next internship stage is in React
and .NET, so building this platform with them now serves his personal development directly.
Nick Hoebe already knows .NET and wants to expand his knowledge of it further, which gives the
team one person who can unblock the other four when they get stuck on the backend. Gurpreet
Singh already has basic React knowledge, which gives the frontend the same kind of starting
point Nick's .NET knowledge gives the backend.

That is a real trade-off, not a free choice. Every strength and weakness section above that
touches the team's unfamiliarity with React and .NET points the same way: an unfamiliar stack
costs velocity, in research time before feature work, in code review that catches basic
framework mistakes rather than design issues, and in a Definition of Done (passing linter,
no compile errors, passing tests) that does not get easier just because the framework is new.
Against that sits the learning gain the team is explicitly choosing: three of five members
building fluency in a stack that at least one of them, Wesley, needs for what comes after this
semester, with Nick Hoebe's existing .NET knowledge as the team's one point of leverage against
the velocity cost. Whether that trade holds up in practice is something for the team to check
at sprint retrospectives, not something this research can settle in advance.

## Whether this research used live web access

Yes. BoKSA and the official React, ASP.NET Core, C#, and SQL Server documentation listed below
were read directly for this research, on 2026-09-22.

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

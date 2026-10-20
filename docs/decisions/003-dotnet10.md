# 003 The .NET version of the server: .NET 10

**Status.** Accepted. Proposed on 8 October 2026 and agreed by the whole team at the sprint 1 review on 9 October 2026. Each team member adds their line from `dotnet --list-sdks` below.
**Issue.** 11 The team agrees on the layered architecture (#12), task 11.3 (#255).
**Who decides.** The whole team. Drafted by Wesley Oerlemans.

## Context

Microsoft gives every .NET version security fixes for a fixed time. A version without fixes keeps running, but a hole found after its end date is never repaired. Connect stores data of young people under 18 and is kept by someone outside the team after January 2027, so the server must run on a version that gets fixes well past that date.

## Options

From the .NET support policy page of Microsoft, https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core, read on 8 October 2026. A long term support version (LTS) gets fixes for three years, a standard term version (STS) for two years.

| Version | Type | Released | Security fixes end | Risk for Connect |
|---|---|---|---|---|
| .NET 8 | LTS | 14 November 2023 | 10 November 2026 | Runs out in the middle of the project, a month after this file |
| .NET 9 | STS | 12 November 2024 | 10 November 2026 | Runs out on the same day as .NET 8 |
| .NET 10 | LTS | 11 November 2025 | 14 November 2028 | Newer, so fewer examples and answers exist for beginners |

## Decision

.NET 10. It is the only one of the three that gets fixes after the project ends, until 14 November 2028, almost two years into the time of the maintainer. .NET 8 and .NET 9 both stop on 10 November 2026, so choosing either means an upgrade before the first sprint review with real users.

Every project targets `net10.0` in its project file, as the projects on dev already do. Task 3.1 (#259) keeps it that way for every new project.

## Consequences

- **For the foundation.** Connect gets security fixes from Microsoft until 14 November 2028. After that the server must move to a newer version.
- **For the team and the maintainer.** The version is set in every `.csproj` as `<TargetFramework>net10.0</TargetFramework>`, and in the images `mcr.microsoft.com/dotnet/sdk:10.0` and `aspnet:10.0` of the pipeline and the Dockerfile.
- **The next upgrade.** Plan it no later than May 2028, six months before the end date. The next LTS version, .NET 12, is expected in November 2027 if Microsoft keeps its yearly rhythm. That date is an expectation, not a fact from the support page.
- **Checked against the team documents.** `docs/team/codeConventions.md` backend rule 1 says .NET 10 is supported "until November 2028" and that .NET 8 stops on 10 November 2026. Both match the Microsoft page. The exact end date of .NET 10 is 14 November 2028.

## Agreed by

Each member runs `dotnet --list-sdks` and writes the line that starts with 10.

| Name | Date agreed | Line from dotnet --list-sdks |
|---|---|---|
| Bayon Eerhart | 9 October 2026 | |
| Nick Hoebe | 9 October 2026 | |
| Wesley Oerlemans | 9 October 2026 | 10.0.401 |
| Gurpreet Singh | 9 October 2026 | |
| Kelvin van Voorst | 9 October 2026 | |

## References

Microsoft. (n.d.). *.NET and .NET Core official support policy*. Retrieved October 8, 2026, from https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

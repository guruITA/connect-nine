# 001 Code conventions: names and layout

**Status.** Accepted. Proposed on 8 October 2026 and agreed by the whole team at the sprint 1 review on 9 October 2026.
**Issue.** 01 The repository, the pipeline and the team rules (#1), task 1.6 (#7).
**Who decides.** The whole team, at a stand up or a meeting. Drafted by Wesley Oerlemans.

Every later decision file copies this form: Status, Context, Options, Decision, Consequences, Agreed by.

## Context

Four of the five students are new to React or .NET, and after January 2027 someone outside the team keeps Connect. The rules for names and layout are in `docs/team/codeConventions.md`, taken from section 3b of the Figma board. Most of them follow the standard of the tool. Four of them are a real choice between two ways that both work. The team confirms those four here once, before the first slice (#14) adds code, so nobody reopens them in a review.

This file does not decide the split of the server into layers. That is #12, in `docs/decisions/002-layeredArchitecture.md`.

## Options

### 1. Folders of the frontend

| | By kind: `pages`, `components`, `api`, `hooks`, `styles` | Per feature: `features/account`, `features/profiles`, ... |
|---|---|---|
| Where a new file goes | One question: is it a page, a piece of screen, a server call or a hook | Two questions: which feature, then which kind |
| Fits | A small app with few pages (sprint 1 and 2 have the account and the profile) | A large app where one feature has many files |
| Source | React itself has no preference and names both ways (React, n.d.) | Same source |

### 2. Names in the database

| | snake_case through EFCore.NamingConventions: `member_profile.created_at` | PascalCase, the EF Core default: `"MemberProfile"."CreatedAt"` |
|---|---|---|
| Typing SQL by hand in psql or pgAdmin | Works without quotes | Every name needs double quotes, because PostgreSQL turns a name without quotes into lower case (PostgreSQL, n.d.) |
| Cost | One package and one line `UseSnakeCaseNamingConvention()` | Nothing to install |

### 3. Server addresses

| | Plural nouns under /api: `POST /api/accounts`, `GET /api/members/{id}` | Verbs: `POST /api/createAccount` |
|---|---|---|
| What the HTTP method does | The method says the action, the address says the thing | The action is said twice |
| Source | Microsoft's API design guide asks for nouns, and plural nouns for collections (Microsoft, n.d.) | No source found that recommends it |

### 4. Commit messages

| | Conventional Commits with the issue number at the end: `feat(backend): add account endpoint (#14)` | Free text |
|---|---|---|
| Reading the history | The type shows at a glance what changed, the number links GitLab to the issue | Every author writes it differently |
| Source | Conventional Commits 1.0.0 (Conventional Commits, n.d.) | None |

### 5. The rest of section 3b, walked through for objections

C# names (the Microsoft conventions), one component per file, secrets only in `.env`, the layout of `.editorconfig`, ProblemDetails for errors and the answer shape `{data, meta}`. These follow the standard of the tool or were agreed in the audit of 27 September 2026. The team only says here whether anyone objects.

## Decision

Decided: the first column of every table, which is what `docs/team/codeConventions.md` already says (frontend rule 7, database rules 1 and 6, backend rule 10, general rule 7). The team agreed it at the sprint 1 review on 9 October 2026, with no objection to point 5.

Why the other options lost, from the tables above:

- Folders per feature fit a large app where one feature has many files. Connect is small in sprint 1 and 2, and by kind asks only one question for a new file.
- PascalCase in the database needs double quotes around every name in SQL typed by hand.
- Verbs in the server addresses say the action twice, and no source recommends them.
- Free text commit messages are written differently by every author and do not link GitLab to the issue.

## Consequences

- The four choices are closed for the rest of the project. Changing one later is a new decision file that replaces this one.
- `docs/team/codeConventions.md` stays the rulebook. When it disagrees with this file after the agreement, the code conventions are changed in the same merge request.
- The frontend folders by kind may get crowded when Connect grows past sprint 3. Moving to folders per feature is then a new decision.
- The tables of ASP.NET Identity keep the names Identity gives them, such as `AspNetUsers` and `AspNetRoles`, as the code of !36 (#261) creates them, and so do their indexes `RoleNameIndex` and `UserNameIndex`. Their columns and keys are snake_case like every other table. This is not a new choice that reopens point 2: the code came first, and where the code and this file differ, the documents follow the code. A query on one of them writes the table name in double quotes: `SELECT email FROM "AspNetUsers"`.

## Agreed by

| Name | Date agreed | Objection |
|---|---|---|
| Bayon Eerhart | 9 October 2026 | |
| Nick Hoebe | 9 October 2026 | |
| Wesley Oerlemans | 9 October 2026 | |
| Gurpreet Singh | 9 October 2026 | |
| Kelvin van Voorst | 9 October 2026 | |

## References

Conventional Commits. (n.d.). *Conventional Commits 1.0.0*. Retrieved October 8, 2026, from https://www.conventionalcommits.org/en/v1.0.0/

Microsoft. (n.d.). *Web API design best practices*. Azure Architecture Center, Microsoft Learn. Retrieved October 8, 2026, from https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design

PostgreSQL. (n.d.). *4.1.1. Identifiers and key words*. PostgreSQL 16 documentation. Retrieved October 8, 2026, from https://www.postgresql.org/docs/16/sql-syntax-lexical.html#SQL-SYNTAX-IDENTIFIERS

React. (n.d.). *File structure*. Retrieved October 8, 2026, from https://legacy.reactjs.org/docs/faq-structure.html

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

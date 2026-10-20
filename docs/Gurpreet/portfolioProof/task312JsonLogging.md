# Proof: task 17.2 Every log line is JSON with the request id (#312)

Merge request: still to open, branch `feature/312-json-logging` into `dev`. Task [#312](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/312) of issue [17 Connect says plainly whether it is healthy (#23)](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/23).
Profile: [Full Stack Software Developer](https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/), level 3. Portflow: [Designing and Realising](https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/design-and-realise/), OR1, OR2, OR4 and OR5.

## What I make, in short

The server of Young Amstelland Connect writes down what happens in its log. Until now every line was plain text, and nothing tied the lines of one visit together. When something goes wrong on the test server, the team has to scroll and guess which lines belong to which visit. In this task every line becomes JSON with the request id of the visit, each visit gets one short line with only its path, status and duration, and no line ever holds personal data or a secret.

## OR1 Designing and building to a plan

BoKSA asks: "You design and implement systematically, justify choices in relation to alternatives, and adapt based on new insights."

1. **The plan, written on 20 October 2026, before I wrote the code.**

   **What one log line holds.** One JSON object per line with `Timestamp` (UTC), `EventId`, `LogLevel`, `Category`, `Message`, `State` (the values of the message template) and `Scopes`. ASP.NET Core opens a scope for every request with `RequestId`, `RequestPath` (without the query string), `TraceId` and `SpanId`, so every line written during a visit carries them. An error line also gets `Exception`.

   **What never goes in a line.** A password, a full email address, a token or anything else from the query string, a header, a request body, a connection string. An account is logged by its uuid, never by its email address.

   **The levels, and why.**

   | Category | Level | Why |
   |---|---|---|
   | `Default` | Information | Our own lines and the start of the server stay visible |
   | `Microsoft.AspNetCore` | Warning | The framework writes several lines per visit at Information, too many to find anything |
   | `Microsoft.AspNetCore.Hosting.Diagnostics` | Warning | Its "Request starting" line holds the full address with the query string, so a token in the address would reach the log |
   | `Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware` | Information | This is the one request line per visit that I do want |
   | `Microsoft.EntityFrameworkCore.Database.Command` | Warning | Every SQL command at Information is noise, and a slow or failed command still shows at Warning |

   **The files, in build order.**

   | Step | File | What |
   |---|---|---|
   | 1 | `api/Connect.Tests/Fixtures/CapturingLoggerProvider.cs` | Test helper that keeps every line with its message, values and scopes, so a test can read what the server logged |
   | 2 | `api/Connect.Tests/Logging/LoggingTests.cs` | The tests below, written first, red before step 3 to 5 |
   | 3 | `api/Connect/Program.cs` | `ClearProviders`, then `AddJsonConsole` with `IncludeScopes` and a UTC timestamp, and `UseHttpLogging` as the first middleware |
   | 4 | `api/Connect.Presentation/DependencyInjection.cs` | `AddHttpLogging` with only `RequestPath`, `RequestMethod`, `ResponseStatusCode` and `Duration`, and `CombineLogs`. In `AddPresentation`, because rule 14 of the code conventions lets `Program.cs` only call the layers |
   | 5 | `api/Connect/appsettings.json` | The levels of the table above |
   | 6 | `docs/team/onboarding.md`, section Logs | What a line looks like, what never goes in it and why, how to find one visit by `RequestId` |

   The task names `api/Connect.Presentation/Program.cs` and `api/Connect.Presentation.Tests/LoggingTests.cs`. The real start project is `api/Connect`, and a test that starts the whole server belongs in `api/Connect.Tests`, so I use those paths.

2. **The alternatives, and why they lost.**

   - **Plain text lines** (what we had). Easy to read for a person, but a program cannot search them by `RequestId`, and the scopes are not printed at all.
   - **Serilog.** Popular and flexible, but it is an extra package with its own configuration, and everything this task needs is built into .NET: the JSON console logger and the HTTP logging middleware. One dependency less to keep up to date.
   - **The HTTP logging middleware with all fields.** It can write headers, the query string and request and response bodies. That is exactly where passwords, emails and tokens live, so I only switch on the four fields above.

3. **What I changed after the first real output.** To fill in after the first run.

## OR2 Choosing standards and methods

To fill in.

## OR4 Validating the solution

To fill in.

## OR5 Impact on users and society

To fill in.

## TODO

What is still needed before this proof is complete. Tick a line when it is done.

- [ ] Me: after !50 of Nick is on `dev`, add the test that no password or full email address is in any line after `POST /api/accounts`, and make sure an error reaching the error handler of !50 is logged once with its request id.
- [ ] Me: fill in OR1 point 3, OR2, OR4 and OR5.
- [ ] Me: write the section Logs in `docs/team/onboarding.md` (build step 6): what a line looks like, what never goes in it and why, how to read the test site logs and find one visit by `RequestId`, with twenty pasted log lines without personal data.
- [ ] Me: check and write down that only team members with a login on the server can read the logs, and that no address reaches them.
- [ ] Reviewer: find the `RequestId` on the lines of one request with the curl step.
- [ ] Me: open the merge request and link it and this file in Portflow under OR1, OR2, OR4 and OR5.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

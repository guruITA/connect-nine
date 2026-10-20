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

3. **What I changed after the first real output.**

   - **The field names.** My test expected the request line to hold `RequestPath`, `RequestMethod`, `StatusCode` and `Duration`, the names of the options. The real line holds `Method`, `PathBase`, `Path`, `StatusCode` and `Duration`. I changed the test to the real names, after checking that no other field (query, header, body) is in it.
   - **Two tests were green before my code.** `GetHealth_TokenInQueryString_NoLineHoldsTheToken` and `GetHealth_DatabaseUnreachable_LogsErrorLineWithRequestId` already passed on `dev`, because `Microsoft.AspNetCore` on `Warning` already hid the line "Request starting". A test that never failed proves nothing, so I set `Microsoft.AspNetCore.Hosting.Diagnostics` to `Information` on purpose: the token test failed, with `abc123secret` in the "Request starting" line. I set it back and made the level explicit in `appsettings.json`, so nobody turns it on by lowering `Microsoft.AspNetCore`.
   - **The request id in the answer.** I assumed the `traceId` of an error answer would be the `RequestId` of the log. It is not: the `RequestId` looks like `0HNPEKQLR24NN:00000001`, the `traceId` like `00-5e23629a8f68c058946e48e752c131e3-bde9ffba6cf23a10-00`. The middle part of the `traceId` is the `TraceId` that every line carries in its scopes. I checked it with curl and the real log, and wrote in `docs/architecture/api.md` and `docs/team/onboarding.md` to search by that part.
   - **Two rules that only a reviewer checked.** Rule 15 (message templates) and "EnableSensitiveDataLogging is never used" had no check. I set CA2254 to `warning` in `.editorconfig`, so `$"..."` in a log call stops the build, and added the test `DbContext_ServerStarted_NeverLogsSensitiveData`.

## OR2 Choosing standards and methods

BoKSA asks: "You independently select relevant standards and methods from the field and substantiate why they fit the context."

| Standard | What I used from it | Why it fits |
|---|---|---|
| [OWASP Logging Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Logging_Cheat_Sheet.html) | The part Data to exclude: among others access tokens, authentication passwords, database connection strings and sensitive personal data | The platform is for young people of 15 to 30, so the list of what stays out is the most important rule of this task. It is why the request line has only four fields and why an account is logged by its uuid |
| [Console log formatting](https://learn.microsoft.com/en-us/dotnet/core/extensions/console-log-formatter) (Microsoft) | The JSON formatter with `IncludeScopes` and `UseUtcTimestamp` | Built into .NET, so no extra package. Without `IncludeScopes` the `RequestId` is not in the line at all |
| [HTTP logging in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/) (Microsoft) | `LoggingFields` with only `RequestPath`, `RequestMethod`, `ResponseStatusCode` and `Duration`, and `CombineLogs` | One line per visit instead of several, and every other field (query, headers, bodies) stays off |
| Message templates and [CA2254](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2254) (Microsoft) | A fixed template with named values, checked by the build | The values land as separate fields in `State`, so they can be searched. It is rule 15 of our code conventions |

Line in my code that follows OWASP:

```csharp
options.LoggingFields = HttpLoggingFields.RequestPath
    | HttpLoggingFields.RequestMethod
    | HttpLoggingFields.ResponseStatusCode
    | HttpLoggingFields.Duration;
```

**Why structured logging fits this team.** Nobody of the team watches the test server all day. When something goes wrong, someone logs in days later with one clue: an error answer with a `traceId`, or a time. With JSON lines that carry the `TraceId` and `RequestId`, one `grep` gives every line of that visit. With plain text lines they would scroll and guess which lines belong together.

## OR4 Validating the solution

BoKSA asks: "You validate the solution and draw conclusions for improvement."

| Rule of #312 | Test or check | Fails when the rule is removed? |
|---|---|---|
| Every line is JSON with scopes and a UTC timestamp | `Logging_ServerStarted_WritesJsonWithScopesAndUtcTimestamp` | Yes, it was red before the code |
| One request line per visit, each with its own `RequestId` | `GetHealth_TwoRequests_EachGetsOneRequestLineWithItsOwnRequestId` | Yes, it was red before the code |
| The request line holds only method, path, status and duration | `GetHealth_TokenInQueryString_RequestLineHoldsOnlyPathMethodStatusAndDuration` | Yes, it was red before the code |
| No query string in any line | `GetHealth_TokenInQueryString_NoLineHoldsTheToken` | Yes, red with `Hosting.Diagnostics` on `Information` (tried on purpose) |
| An error line with a `RequestId` when the database is down | `GetHealth_DatabaseUnreachable_LogsErrorLineWithRequestId` | Green before my code too, see OR1 point 3 |
| `EnableSensitiveDataLogging` is never used | `DbContext_ServerStarted_NeverLogsSensitiveData` | Yes, red with `EnableSensitiveDataLogging()` added on purpose |
| Log lines use a message template | CA2254 in `.editorconfig` | Yes, the build stopped with `error CA2254` on a `$"..."` log call added on purpose |

`dotnet test api/Connect.slnx` on 20 October 2026, with a local PostgreSQL 16:

```
Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1 - Connect.Application.Tests.dll (net10.0)
Passed!  - Failed:     0, Passed:     8, Skipped:     0, Total:     8 - Connect.Presentation.Tests.dll (net10.0)
Passed!  - Failed:     0, Passed:     8, Skipped:     0, Total:     8 - Connect.Tests.dll (net10.0)
Passed!  - Failed:     0, Passed:     9, Skipped:     0, Total:     9 - Connect.Infrastructure.Tests.dll (net10.0)
```

**The twenty real lines.** They are in `docs/team/onboarding.md`, part 12. What I checked in them: `abc123secret` is in no line, the database password is in no line, every request line has a `RequestId`, and the two visits with the database stopped each have an `Error` line and a request line with the same `RequestId` and `TraceId`. The `TraceId` matched the middle part of the `traceId` that curl got back.

**A line with more in it than I wanted.** The `Error` line of the health check holds the whole stack trace, with the paths of the source files on my laptop, and the address of the database (`127.0.0.1:5498`). The password is not in it. I kept it: the team needs the error and the address to find out why the database is down, and the log is only for the team. The web server Kestrel also adds a scope `ConnectionId` to every visit, which I did not plan; it holds no data of a person, so I left it.

**What I found that I did not fix.** When the server cannot start, for example because the port is in use, .NET prints the crash as plain text, not as JSON. That text comes from the runtime itself, not from a logger, so my settings do not reach it. One JSON line `Hosting failed to start` comes before it, so a search still finds the moment.

## OR5 Impact on users and society

BoKSA asks: "In your design and implementation, you consciously take into account the broader impact on end users and society, while forming your own ethical judgment."

**Why the uuid and not the email address.** An email address points to one young person, and often to their full name. A uuid like `3f2a...` means nothing outside our database. When the team needs to know which account had a problem, the uuid is enough: whoever is allowed can look it up in the database, which has its own access rules. Whoever reads only the log learns nothing about the person.

**My judgment.** A richer line finds a bug faster: with the query string, the headers or the body I could see exactly what a user sent. But it would put tokens, emails and passwords of young people in a file that nobody cleans up. I drew the line at method, path, status and duration for every visit, and the error with its stack trace only when something goes wrong. Everything a user types stays out. If that is not enough to find a bug, the team can reproduce the visit locally with the path and the time.

How long the lines stay on the server is decided in [#315](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/315). Whether that is long enough to find a bug and short enough for the data of a minor, I add when it is known.

## TODO

What is still needed before this proof is complete. Tick a line when it is done.

- [ ] Me: after !50 of Nick is on `dev`, add the test that no password or full email address is in any line after `POST /api/accounts`, and make sure an error reaching the error handler of !50 is logged once with its request id.
- [ ] Me: after [#315](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/315), add to OR5 how long lines stay on the server and my judgment on it.
- [x] Me: write the section Logs in `docs/team/onboarding.md` (build step 6): what a line looks like, what never goes in it and why, how to find one visit by `RequestId`, with twenty pasted log lines without personal data.
- [ ] Me: after task 3.9 ([#267](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/267)) puts the server on the test server, check that only team members with a login and the user deploy of the pipeline can read its log, and that no address reaches it, and add how to read it to part 12 of `docs/team/onboarding.md`.
- [ ] Reviewer: do Check it yourself in part 12 of `docs/team/onboarding.md` and find the `RequestId` on the lines of one request.
- [ ] Me: open the merge request and link it and this file in Portflow under OR1, OR2, OR4 and OR5.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

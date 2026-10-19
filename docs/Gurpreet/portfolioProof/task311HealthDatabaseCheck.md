# Proof: task 17.1 The health address checks the database (#311)

Merge request [!65](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/merge_requests/65), branch `feature/311-health-database-check` into `dev`. Task [#311](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/311) of issue [17 Connect says plainly whether it is healthy (#23)](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/23).
Profile: [Full Stack Software Developer](https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/), level 3. Portflow: [Designing and Realising](https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/design-and-realise/), OR1, OR2, OR4 and OR5.

## What I made, in short

We, team Connect Nine, build Young Amstelland Connect for the WailSalutem Foundation. This proof is about my own part in it: task [#311](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/311). The platform keeps everything (accounts, profiles) in a database. If the database is down, the platform is useless: nobody can log in or make an account.

The platform has one address, `/api/health`, that other systems keep asking: "does it work?" The deploy job ([#313](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/313)) asks it after every deploy, and later the readiness probe of k3s ([#1051](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/1051)) will ask it too.

The problem was that this address always said "healthy", even when the database was down. I compare it with a shop with an "Open" sign on the door that always says "Open", also when the till is broken. In this task I made sure someone checks the till first. If the till works, the sign says "Open". If not, it says "Closed for now". The sign never says why, because everyone walking by can read it. The reason goes in a notebook behind the counter that only the team reads, which is the log.

What this gives the foundation and the people who use the platform:

- The team sees within a few seconds when the platform does not really work, instead of hearing it from a user.
- A new version that cannot reach its database does not count as "online and fine".
- Strangers learn nothing about the inside of the platform: no error text, no passwords, no data of young people.

## Words I use in this document

| Word | What I mean with it |
|---|---|
| Server | The program of the platform that runs on a computer on the internet and gives the website its data |
| Database (PostgreSQL) | Where the platform stores its data. PostgreSQL is the database program the team chose |
| Address, endpoint | A web address of the server, like `/api/health`. Another program asks it and gets an answer |
| Health check | A small test the server does on itself: "can I reach the database?" |
| 200, 503, 405 | Standard answer codes on the web. 200 = all fine. 503 = not available right now. 405 = this kind of request is not allowed here |
| Pipeline, deploy job | The automatic steps in GitLab that test a new version and put it online |
| k3s, Kubernetes | What will keep the platform running later, in production. It restarts the server when it crashes |
| Probe (readiness, liveness) | The two questions k3s asks: "is the server ready for visitors?" and "is the server still alive?" |
| Connection | A line between the server and the database, a bit like a phone call |
| Pool | Open connections the server keeps and reuses, so it does not have to "call" again every time |
| Timeout | How long the server waits for an answer before it gives up. In my check: three seconds |
| Package, dependency | Code from other people that a project downloads and uses |
| Log | The notebook of the server, where it writes down what happened. Only the team can read it |
| Layer (Presentation, Infrastructure) | The server is split in parts. Presentation talks to the outside world, Infrastructure talks to the database |
| Test | A small program that checks one rule automatically, every time |
| RFC, OWASP | RFCs are the official rules of the web. OWASP is a well known source for security advice |
| ASP.NET Core, HealthCheckService | ASP.NET Core is the framework the server is built with. HealthCheckService is the part of it that runs all health checks |
| Npgsql | The piece of code that lets .NET talk to PostgreSQL |
| `Cache-Control: no-store` | A line in the answer that says: do not keep this answer, ask again every time |
| ProblemDetails | A fixed shape for error answers on the web |
| traceId | A number in the answer for this one request. Once [#312](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/312) puts it in the log too, the team can use it to find the matching log line |
| curl | A program to call a web address yourself and see the answer |

## OR1 Designing and building to a plan

BoKSA asks: "You design and implement systematically, justify choices in relation to alternatives, and adapt based on new insights."

1. **The plan: how a request goes through the layers, from start to end.**

   | Step | Where (layer) | What happens, and why it is in that layer |
   |---|---|---|
   | 1 | `Program.cs` | The server starts and asks every layer to register its parts. |
   | 2 | `api/Connect.Infrastructure/DependencyInjection.cs` (Infrastructure) | I register one check with the name `database` and a timeout of three seconds. Infrastructure registers its own services, so the rest of the server does not need to know how the check works. Nothing touches the database yet. |
   | 3 | `api/Connect.Presentation/Health/Endpoints/HealthEndpoints.cs` (Presentation) | The address `GET /api/health` already existed and always said "healthy". Now it asks `HealthCheckService` for a report and gives it to `HealthAnswerWriter` (step 9). It stays open without login. Only GET: a `POST` gets `405` with `Allow: GET`. |
   | 4 | The pipeline, the test site or k3s | Someone asks `GET /api/health`. |
   | 5 | `HealthCheckService` (ASP.NET Core) | It runs the only check on the list, `database`, and gives it three seconds. |
   | 6 | `api/Connect.Infrastructure/Health/DatabaseHealthCheck.cs` (Infrastructure) | The check opens one new connection to PostgreSQL with `Timeout=3` and `Pooling=false`, so it really logs in and never reuses an old connection. Then it closes it. It reads and changes nothing. Talking to the database is Infrastructure. |
   | 7 | `DatabaseHealthCheck.cs` | Connection worked: Healthy. Connection failed or took too long: Unhealthy, with the error. |
   | 8 | The log of the server | The real error goes to the log, for example `Npgsql.NpgsqlException: Failed to connect to 127.0.0.1:5499`. Only the team can read it. |
   | 9 | `api/Connect.Presentation/Health/Endpoints/HealthAnswerWriter.cs` (Presentation) | It always sets `Cache-Control: no-store`. Healthy gives `200 {"status":"healthy"}`. Unhealthy gives `503` with only the name of the failing check: `"failing": ["database"]`, never the error itself. The HTTP answer is Presentation. |
   | 10 | The pipeline, the test site or k3s | `200`: the deploy counts as done, and k3s sends visitors. `503`: the deploy fails, and k3s marks the server "not ready" but does not restart it. |

   I put the database part in Infrastructure and the HTTP answer in Presentation, as the team agreed in [decision 002](../../decisions/002-layeredArchitecture.md). So I can test the answer without a database, and the check without HTTP. The tests follow that split: `Connect.Presentation.Tests` tests the answer, `Connect.Infrastructure.Tests` the check against a real PostgreSQL, and `Connect.Tests` the whole app. `docs/architecture/api.md` describes both answers for the team.

2. **The alternatives, and why they lost.**

   - **A second address**, such as `/api/health/database`. Everyone who asks, the deploy job ([#313](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/313)), the browser test ([#1059](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/1059)) and k3s, wants one answer: does the platform work? A second address only means asking twice. The usual reason for it is k3s, which asks two things: "is the server alive?" and "is it ready for visitors?". Compare it with the shop: an employee can be present (in k3s words: alive) but not able to help (the till is broken, so not ready). You do not replace the employee for that, because it does not fix the till; you only hang up "Closed for now". [#1051](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/1051) already does this with one address: k3s asks `/api/health` to see if the server is ready, and only checks that port 8080 is open to see if it is alive. So a database that is down never makes k3s restart the server.
   - **A `SELECT 1` query.** `SELECT 1` is the smallest question you can ask a database: "give me the number 1". If it answers, it works. Compare it with a phone call: my check calls, the database picks up and logs me in, and I hang up. Then I already know it is there. `SELECT 1` would also ask "are you there?" and wait for "yes", which tells me nothing new. When the database is down, picking up already fails. The task also asks for a check that reads nothing.
   - **The package `AspNetCore.HealthChecks.NpgSql`.** It is the common choice, and Microsoft's documentation points to it. Its advantage is many ready checks, but up to sprint 5 the board has only one: the database ([#311](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/311) leaves out email and other services). Its latest version, [9.0.0 from December 2024](https://www.nuget.org/packages/AspNetCore.HealthChecks.NpgSql/9.0.0), is also built for .NET 8 and Npgsql 8, while the platform runs on .NET 10 and Npgsql 10. It may still work, but nobody built or tested it for our version. My teammate Nick Hoebe gave the same advice: write the check ourselves. My way adds no new package: `NpgsqlConnection` comes from Npgsql, which the project already has through Entity Framework, so the foundation has one dependency less to keep up to date. If more checks come later, switching is small: only `DatabaseHealthCheck.cs` and one line in `DependencyInjection.cs` change.

3. **What I changed after a test.** The task asked for `Database.CanConnectAsync`. My test `CheckHealthAsync_DatabaseNeverAnswers_ReturnsUnhealthyWithinFourSeconds` uses a server that never answers. With `CanConnectAsync` the check waited **15.45 seconds**, because Npgsql ignores the cancel and waits its own 15 seconds. The task wants a 503 within five seconds. A shorter timeout for `CanConnectAsync` would count for the whole app, also for logging in. So the check opens its own `NpgsqlConnection` with `Timeout=3`, only for itself. Now the same test passes, and the app answers `503` after 3.55 seconds. The real error also reaches the log now; `CanConnectAsync` hid it.

4. **What I changed after the review.** The review found that the check still said "healthy" after the database went down. Npgsql reuses a connection from its pool without asking the server. I saw it myself: `200`, database off, and still `200` three times. My tests missed it, because none of them stopped the database after a healthy check. Now the check uses `Pooling=false`, so it really connects every time. The new test `CheckHealthAsync_DatabaseGoesDownAfterHealthyCheck_ReturnsUnhealthy` checks once, takes the database away and checks again. Without `Pooling=false` it failed (`Expected: Unhealthy, But was: Healthy`). With it, it passes.

## OR2 Choosing standards and methods

BoKSA asks: "You independently select relevant standards and methods from the field and substantiate why they fit the context."

| Standard | What I used from it | Why it fits |
|---|---|---|
| [Health checks in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks) (Microsoft, read on 9 October 2026) | The built-in parts: `IHealthCheck` (the shape every check has), `AddCheck` (register a check with a name and a timeout) and `HealthCheckService` (runs the checks) | It is built into ASP.NET Core, so no extra package. I did not use `MapHealthChecks`: it reacts to every kind of request, also a `POST`, and writes its own answer. With `MapGet` a `POST` gets 405, and I decide what the answer says. |
| [RFC 9110 HTTP Semantics](https://www.rfc-editor.org/rfc/rfc9110) | 200 (section 15.3.1), 405 (15.5.6) and 503 (15.6.4) | When a kind of request is not allowed (405), the answer must say which kind is: `/api/health` sends `Allow: GET`. |
| [RFC 9457 Problem Details for HTTP APIs](https://www.rfc-editor.org/rfc/rfc9457) | The shape of the 503 answer, marked as `application/problem+json` so programs know it is an error in this standard shape | A standard shape for HTTP errors (`type`, `title`, `status`), so tools can read it. Our [code conventions](../../team/codeConventions.md) ask for it too: under "Answers of the server", rule 3 says every error is ProblemDetails, and rule 2 names exactly this 503 of `/api/health`. |
| [OWASP Error Handling Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Error_Handling_Cheat_Sheet.html) | "a generic response is returned by the application but the error details are logged server side for investigation, and not returned to the user" | The address is open to everyone. The answer only names `failing: ["database"]`. The real Npgsql error goes to the log. Otherwise the database address with its password could end up in the answer. |

**Why 503 and not 500 or 200.** RFC 9110 says 503 means the server cannot handle the request right now, because of a temporary condition. A database that is down is exactly that: the server itself works, but it cannot do its job until the database is back. A 500 would say that something unexpected went wrong inside the server itself, which is not the case. A 200 with "unhealthy" in the body would make the deploy job and k3s read "all fine", because they look at the status code first.

## OR4 Validating the solution

BoKSA asks: "You systematically validate whether the solution does what it is supposed to do, critically evaluate quality, and draw conclusions for improvement."

Each test checks one rule automatically, every time the tests run:

| Test | Rule it proves |
|---|---|
| `HealthAnswerWriterTests.WriteAsync_NotUnhealthy_WritesShortHealthyBody` | When the check says healthy, or degraded (works, but not perfectly), the answer is 200 with exactly `{"status":"healthy"}` |
| `HealthAnswerWriterTests.WriteAsync_UnhealthyWithSecretInException_LeavesOutEveryDetail` | The 503 answer never contains the error text, a description, how long the check took, or a password |
| `HealthAnswerWriterTests.WriteAsync_AnyStatus_SetsNoStore` | Every answer tells browsers and other systems not to keep it (`Cache-Control: no-store`) |
| `HealthEndpointTests` (Presentation) | Without logging in you get 200; when the database fails you get 503 naming `database`; a `POST` gets 405 |
| `HealthDatabaseTests` (Infrastructure) | Against a real database: healthy. When nobody listens at the database address: unhealthy. When the database never answers: unhealthy within four seconds. When the database goes away after a healthy check: unhealthy |
| `Connect.Tests.Health.Endpoints.HealthEndpointTests` | The whole app with a database address where nobody listens (port 5499, like a database that is down) answers 503 naming `database` within five seconds, and the password `secret` from that address never shows up in the answer |

All tests, run with `dotnet test` on 19 October 2026 against a real PostgreSQL 16 database:

```text
Passed!  - Failed: 0, Passed: 1, Total: 1 - Connect.Application.Tests.dll
Passed!  - Failed: 0, Passed: 8, Total: 8 - Connect.Presentation.Tests.dll
Passed!  - Failed: 0, Passed: 2, Total: 2 - Connect.Tests.dll
Passed!  - Failed: 0, Passed: 9, Total: 9 - Connect.Infrastructure.Tests.dll
```

**The test that fails when its rule is removed.** To check that a test really catches a mistake, I broke the rule on purpose: I put the error text, with the password in it, into the 503 answer. `WriteAsync_UnhealthyWithSecretInException_LeavesOutEveryDetail` failed, as it should:

```text
Expected: not String containing "secret"
But was:  "{...,"failing":["database"],"detail":["Host=db;Username=connect;Password=secret"]}"
```

With the rule back, it passes again.

**The real app**, called with curl:

```text
Database up:                       HTTP/1.1 200 OK, Cache-Control: no-store, {"status":"healthy"}
Database down:                     HTTP/1.1 503 Service Unavailable, Content-Type: application/problem+json, Cache-Control: no-store
                                   {"type":"https://tools.ietf.org/html/rfc9110#section-15.6.4","title":"Connect is not healthy","status":503,"failing":["database"],"traceId":"00-..."}
POST /api/health:                  HTTP/1.1 405 Method Not Allowed, Allow: GET
```

For the same call, the real error is only in the server log: `Npgsql.NpgsqlException (0x80004005): Failed to connect to 127.0.0.1:5499`.

What the address answers in each situation, measured on my laptop with the real app:

| Situation | What happens in the check (step 6 of OR1) | Answer | Time |
|---|---|---|---|
| The database works | The connection works | `200` | about 0.01 s |
| The database is off | The connection is refused right away | `503` | about 0.02 s |
| The database hangs (accepts the connection, never answers) | After three seconds the check gives up | `503` | 3.55 s |
| The database goes down after a healthy check | `Pooling=false` makes it connect again instead of reusing the old connection | `503` | right away |

**What is not proven yet.**

- **Finding the log line of one request.** When the check fails, the server writes the error in its log. But I checked: that log line does not hold the `traceId` from the answer. So for now the team can only find the line by its time. [#312](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/312) fixes this: it gives every log line a request id.
- **The test site.** I tested everything on my own laptop. Whether it also works on the test site, I can only show after the merge, when the pipeline puts it there.
- **The tests in the pipeline.** The pipeline of the team does not run `dotnet test` yet. So my 20 green tests ran on my laptop, not in GitLab. That is a weak point of the pipeline, not of my code.

> Write here: what you conclude about the quality, and what you would improve.

## OR5 Impact on users and society

BoKSA asks: "In your design and implementation, you consciously take into account the broader impact on end users and society, while forming your own ethical judgment."

Facts: the address is open to everyone. A stranger only learns whether the platform and its database are up, nothing about the error, the version or the data of young people.

I made sure the answer protects the people behind the platform: it never shows a password, the database address, the error text or how long the check took. Two tests prove this: `WriteAsync_UnhealthyWithSecretInException_LeavesOutEveryDetail` and the whole-app test in `Connect.Tests`. The first one failed when I put the error in the answer on purpose, so it really catches a leak. The real error only goes to the log, which only the team can read. In the log line I checked, there was no password either, only the address and port of the database.

Other facts: every call opens one connection to the database. The planned rate limit of [#1163](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/1163) (sprint 4) leaves this address out.

> Write here: your own judgment on the trade-off. Open, behind a key, or answering less? And what would change your mind?

## TODO

What is still needed before this proof is complete. Tick a line when it is done.

- [ ] Me: fill in the "Write here" parts under OR4 and OR5.
- [ ] Me: after the merge, add the green pipeline and the curl answer of the test server under OR4.
- [ ] Me: after [#312](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/312), check that the log line of a failed check carries the request id.
- [ ] Me: link merge request [!65](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/merge_requests/65) and this file in Portflow under OR1, OR2, OR4 and OR5.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

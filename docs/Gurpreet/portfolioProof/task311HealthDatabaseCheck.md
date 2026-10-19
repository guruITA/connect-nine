# Proof: task 17.1 The health address checks the database (#311)

Merge request [!65](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/merge_requests/65), branch `feature/311-health-database-check` into `dev`. Task [#311](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/311) of issue [17 Connect says plainly whether it is healthy (#23)](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/23).
Profile: [Full Stack Software Developer](https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/), level 3. Portflow: [Designing and Realising](https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/design-and-realise/), OR1, OR2, OR4 and OR5.

## What I made, in short

We, team Connect Nine, build Young Amstelland Connect for the WailSalutem Foundation. This proof is about my own part in it: task [#311](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/311). The platform keeps everything (accounts, profiles) in a database. If the database is down, the platform is useless: nobody can log in or make an account.

The platform has one address, `/api/health`, that other systems keep asking: "does it work?" The pipeline asks it after every deploy, and later k3s will ask it too.

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
| k3s, Kubernetes | What will keep the platform running in production from sprint 4. It restarts the server when it crashes |
| Probe (readiness, liveness) | The two questions k3s asks: "is the server ready for visitors?" and "is the server still alive?" |
| Connection | A line between the server and the database, a bit like a phone call |
| Pool | Open connections the server keeps and reuses, so it does not have to "call" again every time |
| Timeout | How long the server waits for an answer before it gives up. In my check: three seconds |
| Package, dependency | Code from other people that a project downloads and uses |
| Log | The notebook of the server, where it writes down what happened. Only the team can read it |
| Layer (Presentation, Infrastructure) | The server is split in parts. Presentation talks to the outside world, Infrastructure talks to the database |
| Test | A small program that checks one rule automatically, every time |
| RFC, OWASP | RFCs are the official rules of the web. OWASP is a well known source for security advice |

## The problem

Before this task, `GET /api/health` always answered `200 {"status":"healthy"}`, also when the database was down. The deploy job ([#313](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/313)) and later the readiness probe of k3s ([#1051](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/1051)) trust that answer. So the platform looked fine while nobody could log in.

Now the address only says "healthy" when the server can really reach the database at that moment.

## OR1 Designing and building to a plan

BoKSA asks: "You design and implement systematically, justify choices in relation to alternatives, and adapt based on new insights."

1. **The plan: how a request goes through the layers, from start to end.**

   | Step | Where (layer) | What happens, and why it is in that layer |
   |---|---|---|
   | 1 | `Program.cs` | The server starts and asks every layer to register its parts. |
   | 2 | `api/Connect.Infrastructure/DependencyInjection.cs` (Infrastructure) | I register one check with the name `database` and a timeout of three seconds. Infrastructure registers its own services, so the rest of the server does not need to know how the check works. Nothing touches the database yet. |
   | 3 | `api/Connect.Presentation/Health/Endpoints/HealthEndpoints.cs` (Presentation) | The address `GET /api/health` already existed and always said "healthy". Now it asks `HealthCheckService` for a report and gives it to the writer. It stays open without login. Only GET: a `POST` gets `405` with `Allow: GET`. |
   | 4 | The pipeline, the test site or k3s | Someone asks `GET /api/health`. |
   | 5 | `HealthCheckService` (ASP.NET Core) | It runs the only check on the list, `database`, and gives it three seconds. |
   | 6 | `api/Connect.Infrastructure/Health/DatabaseHealthCheck.cs` (Infrastructure) | The check opens one new connection to PostgreSQL with `Timeout=3` and `Pooling=false`, so it really logs in and never reuses an old connection. Then it closes it. It reads and changes nothing. Talking to the database is Infrastructure. |
   | 7 | `DatabaseHealthCheck.cs` | Connection worked: Healthy. Connection failed or took too long: Unhealthy, with the error. |
   | 8 | The log of the server | The real error goes to the log, for example `Npgsql.NpgsqlException: Failed to connect to 127.0.0.1:5499`. Only the team can read it. |
   | 9 | `api/Connect.Presentation/Health/Endpoints/HealthAnswerWriter.cs` (Presentation) | It always sets `Cache-Control: no-store`. Healthy gives `200 {"status":"healthy"}`. Unhealthy gives `503` with only the name of the failing check: `"failing": ["database"]`, never the error itself. The HTTP answer is Presentation. |
   | 10 | The pipeline, the test site or k3s | `200`: the deploy counts as done, and k3s sends visitors. `503`: the deploy fails, and k3s marks the server "not ready" but does not restart it. |

   I put the database part in Infrastructure and the HTTP answer in Presentation, because that is how the team split the server ([decision 002](../../decisions/002-layeredArchitecture.md)). Because of that split I can test the answer without a database, and the check without HTTP.

   The tests follow the same split: `Connect.Presentation.Tests` tests the answer without a database, `Connect.Infrastructure.Tests` tests the check against a real PostgreSQL, and `Connect.Tests` starts the whole app. `docs/architecture/api.md` describes both answers, why the address stays open, and the short body as an agreed exception.

2. **The alternatives the task names, and why they lost.**

   - **A second address for the database**, for example `/api/health/database` next to `/api/health`. The deploy job ([#313](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/313)), the browser test ([#1059](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/1059)) and k3s want one plain answer: does the platform really work? Two addresses means asking twice, and a second open address shows the same information twice. The usual reason for two addresses is Kubernetes: a database that is down must not make it restart the server, because a restart does not fix the database. The team already solved that in [#1051](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/1051) with one address. The readiness probe ("is it ready?") asks `/api/health`. The liveness probe ("is it alive?") only checks that port 8080 is open. A database that is down makes the server "not ready", not "dead". So a second address adds nothing.
   - **A hand written `SELECT 1` check.** Opening a connection already logs in to PostgreSQL. A database that is down or refuses logins already fails the check. A query on every call is extra work that tells nothing more, and the task asks for a check that reads nothing. `SELECT 1` was also a possible fix for the pool problem below. I chose `Pooling=false`, because then the check still only opens and closes one connection.
   - **The ready made package `AspNetCore.HealthChecks.NpgSql`.** In general this is the more common choice, and Microsoft's documentation points to it. Its main advantage is many ready checks. I searched every issue on the board: up to sprint 5 the platform has exactly one check, the database ([#311](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/311) leaves out email and other services). So that advantage is not used. The two real risks of writing it myself, the wait of 15 seconds and the pool, are found, fixed and proven by tests. With the package I would still have to set the timeout and test it again. After the handover the foundation has one dependency less to update and scan. If the platform ever needs more checks, switching is small: only `DatabaseHealthCheck.cs` and one line in `DependencyInjection.cs` change. The address, the answer and their tests stay the same, because every check uses the same interface `IHealthCheck`.

3. **What I changed after a test.** The task asked for `Database.CanConnectAsync`. I tested it with `CheckHealthAsync_DatabaseNeverAnswers_ReturnsUnhealthyWithinFourSeconds`: a server that accepts the connection and never answers, like a database that hangs. With `CanConnectAsync` the check took **15.45 seconds**. Npgsql (the PostgreSQL driver) ignores the cancel of the health check while it connects, and waits for its own default of 15 seconds. That breaks the task: down after three seconds, and a 503 within five seconds. `CanConnectAsync` also hides the error, so the log could not say why. The check now opens its own `NpgsqlConnection` with `Timeout=3`. The same test passes, and the real app answers `503` after 3.55 seconds.

4. **What I changed after the review.** The review found that the check still said "healthy" after the database went down. Npgsql keeps closed connections in a pool (a set of connections it reuses) and hands one back without asking the server. I reproduced it: the real app answered `200`, I stopped the database, and the next three calls still answered `200`. My tests all started with a database that was never reachable, so none of them showed this. The check now sets `Pooling=false`, so every check opens a real new connection. The new test `CheckHealthAsync_DatabaseGoesDownAfterHealthyCheck_ReturnsUnhealthy` puts a relay in front of the real database, checks once, takes the relay away and checks again. Without `Pooling=false` it failed with `Expected: Unhealthy, But was: Healthy`. With it, it passes, and the real app answers `503` right after the database stops.

## OR2 Choosing standards and methods

BoKSA asks: "You independently select relevant standards and methods from the field and substantiate why they fit the context."

| Standard | What I used from it | Why it fits |
|---|---|---|
| [Health checks in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks) (Microsoft, read on 9 October 2026) | `IHealthCheck`, `AddHealthChecks().AddCheck<T>(name, timeout:)` and `HealthCheckService.CheckHealthAsync` | It is built into ASP.NET Core, so no extra package. I did not use `MapHealthChecks`: it answers every HTTP method and writes its own body. With `MapGet` a `POST` gets 405 and I decide what the body says. |
| [RFC 9110 HTTP Semantics](https://www.rfc-editor.org/rfc/rfc9110) | 200 (section 15.3.1), 405 (15.5.6) and 503 (15.6.4) | A 405 must carry an `Allow` header. The 405 of `/api/health` sends `Allow: GET`. |
| [RFC 9457 Problem Details for HTTP APIs](https://www.rfc-editor.org/rfc/rfc9457) | The 503 body, content type `application/problem+json` | A standard shape for HTTP errors (`type`, `title`, `status`), so tools can read it. |
| [OWASP Error Handling Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Error_Handling_Cheat_Sheet.html) | "a generic response is returned by the application but the error details are logged server side for investigation, and not returned to the user" | The address is open to everyone. The answer only names `failing: ["database"]`. The real Npgsql error goes to the log. Otherwise a connection string with a password could leak. |

**Why 503 and not 500 or 200.** RFC 9110 says 503 means the server cannot handle the request right now, because of a temporary condition. A database that is down is exactly that: the server itself works, but it cannot do its job until the database is back. A 500 would say there is a bug in the server, which is not true. A 200 with "unhealthy" in the body would make the deploy job and k3s read "all fine", because they look at the status code first.

## OR4 Validating the solution

BoKSA asks: "You systematically validate whether the solution does what it is supposed to do, critically evaluate quality, and draw conclusions for improvement."

| Test | Rule it proves |
|---|---|
| `HealthAnswerWriterTests.WriteAsync_NotUnhealthy_WritesShortHealthyBody` | Healthy and Degraded give 200 with exactly `{"status":"healthy"}` |
| `HealthAnswerWriterTests.WriteAsync_UnhealthyWithSecretInException_LeavesOutEveryDetail` | The 503 holds no exception, description, duration or password |
| `HealthAnswerWriterTests.WriteAsync_AnyStatus_SetsNoStore` | Every answer has `Cache-Control: no-store` |
| `HealthEndpointTests` (Presentation) | 200 without login, 503 naming `database`, `POST` gives 405 |
| `HealthDatabaseTests` (Infrastructure) | Healthy against the real database, Unhealthy when nothing listens, Unhealthy within four seconds when the database never answers, Unhealthy when the database goes down after a healthy check |
| `Connect.Tests.Health.Endpoints.HealthEndpointTests` | The whole app with `Port=5499` answers 503 naming `database` within five seconds, without `secret` in the body |

`dotnet test` on 19 October 2026, against PostgreSQL 16:

```text
Passed!  - Failed: 0, Passed: 1, Total: 1 - Connect.Application.Tests.dll
Passed!  - Failed: 0, Passed: 8, Total: 8 - Connect.Presentation.Tests.dll
Passed!  - Failed: 0, Passed: 2, Total: 2 - Connect.Tests.dll
Passed!  - Failed: 0, Passed: 9, Total: 9 - Connect.Infrastructure.Tests.dll
```

**The test that fails when its rule is removed.** I put the message of the exception into the 503 body. `WriteAsync_UnhealthyWithSecretInException_LeavesOutEveryDetail` failed:

```text
Expected: not String containing "secret"
But was:  "{...,"failing":["database"],"detail":["Host=db;Username=connect;Password=secret"]}"
```

With the rule back, it passes again.

**The curl answers** of the real app:

```text
Database up:                       HTTP/1.1 200 OK, Cache-Control: no-store, {"status":"healthy"}
Database down:                     HTTP/1.1 503 Service Unavailable, Content-Type: application/problem+json, Cache-Control: no-store
                                   {"type":"https://tools.ietf.org/html/rfc9110#section-15.6.4","title":"Connect is not healthy","status":503,"failing":["database"],"traceId":"00-..."}
POST /api/health:                  HTTP/1.1 405 Method Not Allowed, Allow: GET
```

The server log for the same call holds the real error: `Npgsql.NpgsqlException (0x80004005): Failed to connect to 127.0.0.1:5499`.

What the address answers in each situation, measured on my laptop with the real app:

| Situation | What happens in the check (step 6 of OR1) | Answer | Time |
|---|---|---|---|
| The database works | The connection works | `200` | about 0.01 s |
| The database is off | The connection is refused right away | `503` | about 0.02 s |
| The database hangs (accepts the connection, never answers) | After three seconds the check gives up | `503` | 3.55 s |
| The database goes down after a healthy check | `Pooling=false` makes it connect again instead of reusing the old connection | `503` | right away |

**What is not proven yet.** The request id of [#312](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/312) in the log line: [#312](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/312) is still open. The `traceId` in the answer already finds the log line. The pipeline and the test server: only after the merge. The pipeline does not run `dotnet test` yet, so these results come from my own laptop.

> Write here: what you conclude about the quality, and what you would improve.

## OR5 Impact on users and society

BoKSA asks: "In your design and implementation, you consciously take into account the broader impact on end users and society, while forming your own ethical judgment."

Facts: the address is open to everyone. A stranger only learns whether the platform and its database are up, nothing about the error, the version or the data of young people. Every call opens one connection to the database. The planned rate limit of [#1163](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/1163) (sprint 4) leaves this address out.

> Write here: your own judgment on the trade-off. Open, behind a key, or answering less? And what would change your mind?

## TODO

What is still needed before this proof is complete. Tick a line when it is done.

- [ ] Me: fill in the "Write here" parts under OR4 and OR5.
- [ ] Me: after the merge, add the green pipeline and the curl answer of the test server under OR4.
- [ ] Me: after [#312](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/312), check that the log line of a failed check carries the request id.
- [ ] Me: link merge request [!65](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/merge_requests/65) and this file in Portflow under OR1, OR2, OR4 and OR5.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

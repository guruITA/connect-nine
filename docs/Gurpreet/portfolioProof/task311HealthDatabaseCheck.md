# Proof: task 17.1 The health address checks the database (#311)

Merge request: the merge request of branch `feature/311-health-database-check` into `dev`. Issue: 17 Connect says plainly whether it is healthy (#23).
Profile: Full Stack Software Developer, level 3. Link the merge request and this file in Portflow under the codes below.

What I made: `GET /api/health` now answers `200 {"status":"healthy"}` only when the server can reach PostgreSQL. When it cannot, it answers `503` as ProblemDetails that names only the failing check. The check is `api/Connect.Infrastructure/Health/DatabaseHealthCheck.cs`, the answer is `api/Connect.Presentation/Health/Endpoints/HealthAnswerWriter.cs`, and `docs/architecture/api.md` describes both answers.

## Which codes this task proves

The BoKSA proof of the task names OR1, OR2, OR4 and OR5.

| Code | Level 3 criterion (short) | Fit | Why |
|---|---|---|---|
| OR1 | Systematic, choices against alternatives, adjust on new insights | strong | The plan per layer, the alternatives, and the change after a measured test |
| OR2 | Select standards and methods and explain why they fit | strong | Microsoft health checks, RFC 9110 for 200, 503 and 405, OWASP on error details |
| OR4 | Validate systematically, evaluate quality critically, conclude what to improve | strong | Unit, integration and end-to-end tests, a test that fails when its rule is removed, curl answers |
| OR5 | Impact on users and society, own ethical judgment | good | An open address tells a stranger when the database is down |

## OR1 Designing and building to a plan

School asks: "You design and realize in a systematic way, justify choices in relation to alternatives and adjust based on new insights."

1. **The plan, in build order.**

   | File | New or changed | Why it is in that layer |
   |---|---|---|
   | `api/Connect.Infrastructure/Health/DatabaseHealthCheck.cs` | new | Talking to PostgreSQL is Infrastructure. The check opens one connection and closes it, and reads and changes nothing. |
   | `api/Connect.Infrastructure/DependencyInjection.cs` | changed | Infrastructure registers its own services: the check `database` with a timeout of three seconds. |
   | `api/Connect.Presentation/Health/Endpoints/HealthAnswerWriter.cs` | new | The HTTP answer is Presentation: 200 or 503, `Cache-Control: no-store`, and no details. |
   | `api/Connect.Presentation/Health/Endpoints/HealthEndpoints.cs` | changed | The existing address now asks `HealthCheckService` and hands the report to the writer. `MapGet` keeps every other method at 405. |
   | Tests in `Connect.Presentation.Tests`, `Connect.Infrastructure.Tests` and `Connect.Tests` | new and changed | Each layer is tested where it lives. `Connect.Tests` starts the whole app. |
   | `docs/architecture/api.md` | changed | Both answers, why the address stays open, and the short body as an agreed exception. |

2. **The alternatives the task names.** A second address for the database, a hand written `SELECT 1` check, and a ready made package (`AspNetCore.HealthChecks.NpgSql`).

   > Write here: why each of the three lost.

3. **What I changed after a test.** The task asked for `Database.CanConnectAsync`. I measured it with the test `CheckHealthAsync_DatabaseNeverAnswers_ReturnsUnhealthyWithinFourSeconds`, which starts a server that accepts the connection and never answers, like a database that hangs. With `CanConnectAsync` the check took **15.45 seconds**, because Npgsql keeps waiting for its own default timeout of 15 seconds and ignores the cancel of the health check. That breaks the rule of the task: down after three seconds, and a 503 within five seconds. `CanConnectAsync` also swallows the error, so the log could not say why. The check now opens its own `NpgsqlConnection` with `Timeout=3`. With that, the same test passes, and the real app answers `503` after 3.55 seconds.

## OR2 Choosing standards and methods

School asks: "You select relevant standards and methods and explain why they fit."

- **Health checks in ASP.NET Core** (Microsoft, read on 9 October 2026, https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks). Used: `IHealthCheck`, `AddHealthChecks().AddCheck<T>(name, timeout:)` and `HealthCheckService.CheckHealthAsync`. Not used: `MapHealthChecks`, because it answers on every HTTP method and writes its own body. With `MapGet` a `POST` gets 405 and the body stays ours.
- **RFC 9110 HTTP Semantics** (https://www.rfc-editor.org/rfc/rfc9110). 200 in section 15.3.1, 405 in 15.5.6 and 503 in 15.6.4. RFC 9110 says a 405 must carry an `Allow` header. Our 405 sends `Allow: GET`.
- **RFC 9457 Problem Details** for the 503 body, with the content type `application/problem+json`.
- **OWASP Error Handling Cheat Sheet** (https://cheatsheetseries.owasp.org/cheatsheets/Error_Handling_Cheat_Sheet.html). It says: "a generic response is returned by the application but the error details are logged server side for investigation, and not returned to the user", and "Using error messages that do not provide implementation details is important to avoid information leakage." That is why the answer names only `failing: ["database"]` and the Npgsql error goes to the log.

> Write here: why 503 fits a database that is down, in your own words (RFC 9110 section 15.6.4 speaks of a temporary condition).

## OR4 Validating the solution

School asks: "You validate the solution systematically, evaluate the quality critically and draw conclusions for improvement."

| Test | Rule it proves |
|---|---|
| `HealthAnswerWriterTests.WriteAsync_NotUnhealthy_WritesShortHealthyBody` | Healthy and Degraded give 200 with exactly `{"status":"healthy"}` |
| `HealthAnswerWriterTests.WriteAsync_UnhealthyWithSecretInException_LeavesOutEveryDetail` | The 503 holds no exception, description, duration or password |
| `HealthAnswerWriterTests.WriteAsync_AnyStatus_SetsNoStore` | Every answer has `Cache-Control: no-store` |
| `HealthEndpointTests` (Presentation) | 200 without login, 503 naming `database`, `POST` gives 405 |
| `HealthDatabaseTests` (Infrastructure) | Healthy against the real database, Unhealthy when nothing listens, Unhealthy within four seconds when the database never answers |
| `Connect.Tests.Health.HealthEndpointTests` | The whole app with `Port=5499` answers 503 naming `database` within five seconds, without `secret` in the body |

`dotnet test` on 19 October 2026, against PostgreSQL 16:

```text
Passed!  - Failed: 0, Passed: 1, Total: 1 - Connect.Application.Tests.dll
Passed!  - Failed: 0, Passed: 8, Total: 8 - Connect.Presentation.Tests.dll
Passed!  - Failed: 0, Passed: 2, Total: 2 - Connect.Tests.dll
Passed!  - Failed: 0, Passed: 8, Total: 8 - Connect.Infrastructure.Tests.dll
```

**The test that fails when its rule is removed.** I put the message of the exception into the 503 body. `WriteAsync_UnhealthyWithSecretInException_LeavesOutEveryDetail` failed:

```text
Expected: not String containing "secret"
But was:  "{...,"failing":["database"],"detail":["Host=db;Username=connect;Password=secret"]}"
```

With the rule back, it passes again.

**The curl answers** of the real app:

```text
Database up:          HTTP/1.1 200 OK, Cache-Control: no-store, {"status":"healthy"}
Database down:        HTTP/1.1 503 Service Unavailable, Content-Type: application/problem+json, Cache-Control: no-store
                      {"type":"https://tools.ietf.org/html/rfc9110#section-15.6.4","title":"Connect is not healthy","status":503,"failing":["database"],"traceId":"00-..."}
Database never answers: 503 after 3.55 seconds
POST /api/health:     HTTP/1.1 405 Method Not Allowed, Allow: GET
```

The server log for the same call holds the real error: `Npgsql.NpgsqlException (0x80004005): Failed to connect to 127.0.0.1:5499`.

**What is not proven yet.** The request id of #312 in the log line: #312 is still open. The `traceId` in the answer already finds the log line. The pipeline and the test server: only after the merge.

> Write here: what you conclude about the quality, and what you would improve.

## OR5 Impact on users and society

School asks: "You weigh the impact on end users and society and form your own ethical judgment."

Facts: the address is open to everyone. A stranger learns only whether Connect and its database are up, nothing about the error, the version or the data of young people. Every call opens one connection to the database.

> Write here: your own judgment on the trade-off. Open, behind a key, or answering less? And what would change your mind?

## TODO

What is still needed before this proof is complete. Tick a line when it is done.

- [ ] Me: fill in the "Write here" parts under OR1, OR2, OR4 and OR5.
- [ ] Me: after the merge, add the green pipeline and the curl answer of the test server under OR4.
- [ ] Me: after #312, check that the log line of a failed check carries the request id.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

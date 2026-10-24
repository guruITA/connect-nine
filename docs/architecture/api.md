# Server addresses (API)

Every address of the Connect server: who may call it, what goes in and every answer. The frontend developer looks up an endpoint here without reading the server code. Each issue that adds or changes an address adds its lines here, in the same merge request (Definition of Done).

The general rules for answers are in `docs/team/codeConventions.md`, part Added after the audit of 27 September 2026, and in `docs/architecture/architecture.md`, part Answers of the server.

## Open to everyone

These addresses work without logging in (`AllowAnonymous`). Every other address needs a logged in member.

| Address | Why it is open |
|---|---|
| `GET /api/health` | The deploy job, a monitor and the client check it without an account. |
| Every address outside `/api` | It is the screen, which decides itself what a visitor may see (see The screen and unknown addresses). |

## GET /api/health

Says whether Connect is really up: the server runs and it can reach the database (#311, issue 17 Connect says plainly whether it is healthy, GitLab #23).

- Who may call it: everyone, also a visitor who is not logged in.
- What goes in: nothing. Any other method than GET gets `405 Method Not Allowed`.
- Code: `api/Connect.Presentation/Health/Endpoints/HealthEndpoints.cs`, the answer in `HealthAnswerWriter.cs` next to it, and the database check in `api/Connect.Infrastructure/Health/DatabaseHealthCheck.cs`.

### What the server checks

Only the database. The check opens one connection to PostgreSQL and closes it again, and reads and changes nothing. When the database does not answer within three seconds, the check counts it as down. Email and other services are not checked here.

### Answer when Connect is healthy

`200 OK`, with the header `Cache-Control: no-store`:

```json
{"status":"healthy"}
```

### Answer when the database cannot be reached

`503 Service Unavailable`, with the header `Cache-Control: no-store`, as ProblemDetails (RFC 9457) with the content type `application/problem+json`:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.4",
  "title": "Connect is not healthy",
  "status": 503,
  "failing": ["database"],
  "traceId": "00-137a3514cea8ab6a381ba29c192e5320-d30638e12c00fa57-00"
}
```

`failing` names only the checks that failed. The answer never holds an error text, a description, a duration, a version or a connection string, because the address is open to everyone. The real error goes to the log of the server. Since #312 that log line carries the `TraceId` and `RequestId` of the request in its scopes. The `TraceId` is the middle part of the `traceId` in the answer, so the answer can be matched to its log line.

### Agreed exception to the answer form

Every other successful answer is `{"data": ..., "meta": {...}}`. This address answers the short body above instead, because deploy jobs and monitors read exactly that body. The team agreed on this exception in `docs/team/codeConventions.md`, rule 2 of Answers of the server.

### Sources

- Health checks in ASP.NET Core, Microsoft, read on 9 October 2026: https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks
- RFC 9110 HTTP Semantics, sections 15.3.1 (200), 15.5.6 (405) and 15.6.4 (503): https://www.rfc-editor.org/rfc/rfc9110
- RFC 9457 Problem Details for HTTP APIs: https://www.rfc-editor.org/rfc/rfc9457
- OWASP Error Handling Cheat Sheet: https://cheatsheetseries.owasp.org/cheatsheets/Error_Handling_Cheat_Sheet.html

## `POST /api/accounts`

Creates an account from a JSON request body:

```json
{"email":"user@example.com","password":"example-password"}
```

The endpoint has no explicit authorization requirement. It is anonymous unless
an application-wide authorization policy requires authentication.

Returns:

- HTTP `201 Created` with the account response produced by
  `CreateAccountResponseMapper` and a `Location` header of `/api/accounts/{id}`.
- HTTP `400 Bad Request` when validation fails. Request validation returns
  validation Problem Details with field errors; command validation returns
  Problem Details with the error description as its title.
- HTTP `409 Conflict` when the command reports a conflict, with Problem Details
  containing the error description as its title.
- HTTP `503 Service Unavailable` when an exception reaching the exception-handling
  middleware is, or contains, a database exception (`DbException`). Returns
  Problem Details titled `Connect is not reachable right now`.
- HTTP `500 Internal Server Error` for other mapped errors or unhandled exceptions,
  with Problem Details titled `An unexpected error occurred.`

## `POST /api/member-profiles`

Saves the profile of the young person who is logged in (issue 07 Create a profile, task 7.2).
The account comes from the session and never from the body: a field `accountId` in the body is
ignored. Until log in (#15) adds the session, the account comes from the header
`X-Temporary-Account-Id`, which the server only accepts in Development with
`TemporaryAccount:Enabled` set to `true`. Everywhere else nobody is logged in yet, so the address
answers `401`.

The body:

```json
{"firstName":"Sam","ageBand":"From18To21","region":"Amstelveen","bio":"I like football."}
```

`firstName` and `region` are required and at most 60 characters. `ageBand` is exactly
`From15To17`, `From18To21` or `From22To30`. `bio` may be empty and is at most 280 characters.
Spaces around the text are taken off before the checks, and an empty `bio` is saved as `null`.

The five answers:

- `201 Created`, with the `Location` `/api/member-profiles/{id}`:

  ```json
  {"data":{"id":"01a15623-496f-7b12-9fc3-0fbd4ef1b1d3"},"meta":{}}
  ```

- `400 Bad Request`, one message per wrong field, and nothing is saved:

  ```json
  {"title":"One or more validation errors occurred.","status":400,"errors":{"firstName":["Fill in your first name"],"ageBand":["Choose your age band"]}}
  ```

- `401 Unauthorized` when nobody is logged in, without a body.
- `409 Conflict` when the account already has a profile, also for two saves at the same moment:

  ```json
  {"title":"You already have a profile","status":409}
  ```

- `503 Service Unavailable` when the database cannot be reached, without any error text:

  ```json
  {"title":"Connect is not reachable right now","status":503}
  ```

The first name, the region and the bio are in no log line.

## The screen and unknown addresses

The server also serves the built screen (`web/dist`, copied into `wwwroot` by the job deploy-test), so the test site has one address and needs no CORS (#267).

- `GET` of a file of the screen, such as `/assets/index-<hash>.js`: the file, without a login.
- `GET` or `HEAD` of any other address outside `/api`, such as `/` or `/create-account`: `200` with `index.html`. The screen picks the page from the address itself.
- A missing file outside `/api`, such as `/missing.svg`: `404` without a body.
- Any method on an address under `/api` that does not exist: `404` with `application/problem+json`:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "This address does not exist",
  "status": 404,
  "traceId": "00-<trace id>-<span id>-00"
}
```

- A known address with another method keeps its `405`, for example `POST /api/health`.
- Code: `api/Connect.Presentation/Screen/ScreenPipeline.cs`, called as `app.UseScreen()` in `api/Connect/Program.cs` before authentication. It answers only when no address answered with a body. A `MapFallback` was not used, because routing then picks the fallback over the `405` of a known address.

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to draft
the formulation of sentences, the structure of wording, readability, and clarity from
content that we had provided. The content itself, including our experiences, reflections,
learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the
drafts and decided which text to include.

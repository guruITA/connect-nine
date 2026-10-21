# Proof: a safe and consistent HTTP error boundary (#262)

MR [!50](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/merge_requests/50), task [#262](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/262). Profile: Full Stack Software Developer, level 3. Portflow candidates: OR1, OR2, OR4, OR5, AA3 and AA4.

## Result

The Presentation boundary maps expected `Error` values to `ProblemDetails` and handles unexpected exceptions centrally in `ExceptionHandlingMiddleware`. Public responses are generic and safe; diagnostic details remain server-side.

## OR1 — planned boundary and changed insight

The flow is: request validation, handler result, expected-error mapper, or exception middleware. [`ErrorProblemDetailsMapper.cs`](../../../api/Connect.Presentation/Common/Errors/ErrorProblemDetailsMapper.cs), [`ExceptionHandlingMiddleware.cs`](../../../api/Connect.Presentation/Common/Errors/ExceptionHandlingMiddleware.cs), [`ErrorProblemDetailsMapperTests.cs`](../../../api/Connect.Presentation.Tests/Common/Errors/ErrorProblemDetailsMapperTests.cs), [`ExceptionHandlingMiddlewareTests.cs`](../../../api/Connect.Presentation.Tests/Common/Errors/ExceptionHandlingMiddlewareTests.cs) and the endpoint tests are the core evidence.

The mapping distinguishes validation, unauthorized, not found, conflict and failure. A nested `DbException` produces the documented generic 503 policy; other unhandled exceptions produce a generic 500. If the response has started, middleware rethrows because replacing a partial response is unsafe.

## OR2 and AA4 — standards and recommendation

The recommendation follows the team’s response decision and the Problem Details convention. It rejects bare text errors and ad-hoc `{"error": ...}` shapes because clients need a stable standard shape and internal details must not leak. The broad database policy is intentionally described as a safe classification policy, not as proof that every database problem is temporary.

## OR4 — validation

The mapper test `ToProblemDetails_MapsErrorCodeToStatus` covers validation 400, unauthorized 401, not found 404, conflict 409 and failure 500. `ToProblemDetails_UsesGenericTitleForFailure` checks the generic title. Middleware tests cover a generic exception as 500 and a database exception as 503, with Problem Details content and no secret in the body. Whole-application tests exercise the same boundary through the running host: conflict becomes 409, a database exception becomes 503, invalid input becomes 400, and a valid request becomes 201. The supplied output also showed the middleware logging a request id while returning 503 without the exception text to the caller.

## OR5 and AA3 — privacy and ethics

The endpoint may be public, so exception text, passwords, connection details and stack traces must not enter the response. The endpoint test passes a password and a synthetic database exception, then asserts that the response is 503 and does not contain the password. The captured test output shows the stack trace in the server-side log, including the request id, while the HTTP result remains generic. The ethical judgment is to sacrifice diagnostic detail in the public body for privacy and security, while retaining server-side diagnostics for the team. Duplicate account responses still expose a membership signal and need a separate privacy discussion.

## Limits

Generic messages prove safe exposure, not perfect diagnosis or a complete database-failure taxonomy. Claim logging only where tests or captured output show it. Do not call every `DbException` temporary network unreachability.

## Evidence checklist

- [x] Attach the mapper/middleware test output; see the [shared test run](task262ValidationAndTesting.md#test-run) and the [backend-tests pipeline job](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/jobs/1345780).
- [ ] Add one deliberately leaked-detail failure if it was actually recorded.
- [x] Link [`docs/security/securityExceptions.md`](../../security/securityExceptions.md) and [`docs/architecture/api.md`](../../architecture/api.md).
- [ ] Add reviewer/team evidence before claiming stakeholder involvement.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

# Proof: application results make expected outcomes explicit (#262)

MR [!50](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/merge_requests/50), task [#262](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/262). Profile: Full Stack Software Developer, level 3. Portflow candidates: OR1, OR2, OR4, AA2 and AA4.

## Result

The Application layer now returns an application-owned `Result` or `Result<T>`. It does not return HTTP results, choose status codes or construct `ProblemDetails`. Expected business outcomes such as a duplicate account are visible in the handler contract; unexpected failures remain exceptions for the Presentation boundary to handle.

## OR1 — planned design, alternatives and changed insight

The plan was to keep the use case independent of transport and persistence details. I compared direct return values plus exceptions with `Result`/`Result<T>`, and compared application-owned results with returning ASP.NET results from Application. The latter would make the inner layer depend on HTTP and make reuse and unit testing harder.

The implementation is in [`Result.cs`](../../../api/Connect.Application/Common/Results/Result.cs), [`Result{T}.cs`](../../../api/Connect.Application/Common/Results/Result{T}.cs), [`Error.cs`](../../../api/Connect.Application/Common/Results/Error.cs), [`ErrorCode.cs`](../../../api/Connect.Application/Common/Results/ErrorCode.cs), [`CreateAccountCommandHandler.cs`](../../../api/Connect.Application/Accounts/CreateAccount/CreateAccountCommandHandler.cs) and [`IAccountRegistrationService.cs`](../../../api/Connect.Application/Accounts/CreateAccount/IAccountRegistrationService.cs). The handler returns `Result<CreateAccountResult>` and delegates registration; Infrastructure decides whether a known duplicate becomes `Error.Conflict`. Unknown exceptions are deliberately not converted into a misleading business result.

The result invariants are concrete: `Result.Success()` carries `Error.None`; `Result.Failure` rejects `Error.None`; `Result<T>.Success` rejects null; and a failed `Result<T>` has no value. This is stronger evidence than simply having a result class because invalid states are rejected at construction time.

## OR2 — professional method

Decision 005 records the result pattern as an architectural decision with context, alternatives, decision and consequences. The method fits because the problem is a boundary decision, not merely a coding preference: the application must remain reusable while Presentation owns HTTP.

## OR4 — validation

[`ResultTests.cs`](../../../api/Connect.Application.Tests/Common/Results/ResultTests.cs) contains `Success_ContainsValueAndNoError`, `Failure_ContainsErrorAndNoValue`, `Failure_WithNoError_IsRejected` and `Success_WithNullValue_IsRejected`. [`CreateAccountHandlerTests.cs`](../../../api/Connect.Application.Tests/Accounts/CreateAccount/CreateAccountHandlerTests.cs) contains `HandleAsync_DelegatesRegistrationToService`, proving that the Application handler delegates instead of reaching into Infrastructure. The supplied run reports 6 Application tests passed. A deliberately broken-rule output was not supplied, so that remains an open strengthening step.

## AA2 and AA4 — critical advice

The advice was to use `Result` in Application and translate it once in Presentation. The main risks considered were HTTP coupling, hidden exception control flow and inconsistent error vocabularies. The consequence is a small amount of result code and an explicit mapping step, in exchange for a contract that can be tested without ASP.NET.

## Limits

This proof does not prove the complete HTTP mapping or database behavior; those are covered by the other #262 proofs. It also does not prove that every future use case needs the same abstraction.

## Evidence checklist

- [x] Attach final test output and execution context; see the [shared test run](task262ValidationAndTesting.md#test-run) and the [backend-tests pipeline job](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/jobs/1345780).
- [x] Link the exact commits introducing the result types and handler: `d122068`, `8181c65` and `9e1807b`.
- [ ] Add the failed assertion from a deliberately invalid result, if that experiment was captured.
- [ ] Link this file and MR !50 in Portflow only under supported criteria.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

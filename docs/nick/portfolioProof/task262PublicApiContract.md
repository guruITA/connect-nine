# Proof: the account endpoint owns a public API contract (#262)

MR [!50](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/merge_requests/50), task [#262](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/262). Profile: Full Stack Software Developer, level 3. Portflow candidates: OR1, OR2, OR4, OR5, AA4 and AA5.

## Result

Presentation owns the request and response contract. It validates `CreateAccountRequest`, invokes the application handler, maps `CreateAccountResult` to `CreateAccountResponseData` and returns the shared `data`/`meta` envelope. It does not return a domain entity or application record directly.

## OR1 — request-to-response design

The flow is visible in [`CreateAccountRequest.cs`](../../../api/Connect.Presentation/Accounts/DTO/CreateAccountRequest.cs), [`CreateAccountRequestValidator.cs`](../../../api/Connect.Presentation/Accounts/Validators/CreateAccountRequestValidator.cs), [`AccountEndpoints.cs`](../../../api/Connect.Presentation/Accounts/Endpoints/AccountEndpoints.cs), [`CreateAccountResponseMapper.cs`](../../../api/Connect.Presentation/Accounts/Mappers/CreateAccountResponseMapper.cs), [`Response.cs`](../../../api/Connect.Presentation/Common/Response/Response.cs) and [`DefaultMeta.cs`](../../../api/Connect.Presentation/Common/Response/DefaultMeta.cs). Decision 006 records the alternatives: direct/custom response shapes versus a stable success envelope and Problem Details, and entities/application records versus Presentation-owned DTOs.

The main changed insight was that a mapper is a deliberate boundary, not unnecessary repetition: it prevents internal fields and naming from becoming accidental public API. The endpoint remains responsible for HTTP status codes; the mapper only maps data.

## OR2 — standards and methods

The proof uses the team’s response decision, Problem Details conventions and standard HTTP semantics. The final version should link the relevant API documentation and state why a consistent contract fits browser clients and future callers.

## OR4 — validation

[`CreateAccountRequestValidatorTests.cs`](../../../api/Connect.Presentation.Tests/Accounts/Validators/CreateAccountRequestValidatorTests.cs) checks empty/invalid email, empty password, password composition and a valid strong password. [`AccountEndpointTests.cs`](../../../api/Connect.Tests/Accounts/Endpoints/AccountEndpointTests.cs) checks `PostAccount_ValidRequest_ReturnsCreatedEnvelopeWithoutPassword`, `PostAccount_InvalidRequest_ReturnsValidationProblemWithCamelCaseKeys`, `PostAccount_ConflictResult_ReturnsConflictProblemDetails` and `PostAccount_DatabaseException_ReturnsServiceUnavailableProblemDetails`. The invalid-request test asserts both the `400` response and `handlerCalled == false`; the valid-response test asserts `201`, `data.id`, `data.email`, empty `meta` and absence of the submitted password. The supplied run reports 21 Presentation tests and 14 whole-application tests passed.

## OR5 and AA4 — public data and trade-offs

The public DTO contains only the fields selected for the account response. The trade-off is a little mapping code in exchange for preventing persistence and application models from becoming accidental contracts. Duplicate-email disclosure should be described as an explicit privacy/usability decision, not silently treated as harmless.

## AA5 — communication

The API documentation explains the contract for developers and the DTO/endpoint names make the boundary visible to maintainers. To claim the full level-3 communication criterion, add evidence that the decision was explained or adapted for audiences with different interests, such as the team and a non-technical stakeholder.

## Validation-error naming

The global property-name resolver and the proposed boundary-local camel-case conversion are different designs. If the extension is not implemented and tested, this proof must label it as a recommendation. If implemented, add tests for dotted paths, collection indexes, acronyms, duplicate messages and explicit aliases, and remove the global side effect.

## Limits

This proves the account endpoint and shared response pattern used by the slice, not a complete API-wide contract. It does not prove an email-verification privacy flow.

## Evidence checklist

- [x] Attach endpoint test output and the invalid-request handler assertion; see the [shared test run](task262ValidationAndTesting.md#test-run), the [backend-tests pipeline job](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/jobs/1345780) and `AccountEndpointTests.cs`.
- [x] Link the exact API documentation section: [`docs/architecture/api.md`](../../architecture/api.md).
- [ ] Add audience/feedback evidence before claiming full AA5 or OR3.
- [x] Keep validation naming proposals separate from delivered code.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

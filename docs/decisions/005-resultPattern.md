# 005 Application result pattern

**Status.** Proposed on 13 October 2026, waiting for the team.
**Issue.** The common pattern for expected application outcomes.
**Who decides.** The whole team. Drafted by Nick Hoebe.

## Context

Application use cases need to describe a successful business outcome or an expected
business failure. They should not need to know that the caller is HTTP, choose an HTTP
status code, or construct an ASP.NET Core result. Expected failures should also be
visible in the handler's contract instead of being hidden in exception control flow.

Connect already uses `Result` and `Result<T>` for this boundary. Presentation translates
the result into the HTTP response described in `docs/decisions/006-responsePattern.md`.

## Options

### 1. Return data directly and throw for expected failures

| | Direct data and exceptions | `Result` and `Result<T>` |
|---|---|---|
| Application knows about HTTP | It often does, because the exception or result is shaped for the endpoint | It does not; it returns an application-owned outcome |
| Expected conflict or not-found flow | Hidden in exception control flow | Visible in the handler's return type and branch |
| Tests | Assert thrown exception types and messages | Assert `IsSuccess`, `Value` and semantic `ErrorCode` |
| Boundary mapping | Each endpoint may map a different exception | Presentation maps one shared error vocabulary |

### 2. Return HTTP results from Application

This would make the inner layer depend on ASP.NET Core and would make use-case tests
depend on transport concerns. It also makes it harder to reuse a use case from another
caller.

## Decision

Application handlers return:

- `Result` when the operation has no success value.
- `Result<T>` when the operation returns a value.

Both types are immutable records with a private constructor. A successful result has
`IsSuccess == true` and `Error.Code == ErrorCode.None`. A successful `Result<T>` must
contain a non-null value. A failed result has `IsSuccess == false`, no value in
`Result<T>`, and an error whose code is not `None`. `Result.Failure(Error.None)` and
`Result<T>.Failure(Error.None)` are rejected so an outcome cannot be both success and
failure.

Handlers use the factory methods:

```csharp
return Result<CreateAccountResult>.Success(
    new CreateAccountResult(account.Id, email));

return Result<CreateAccountResult>.Failure(
    Error.Conflict("This email address already has an account"));
```

`ErrorCode` is semantic and application-owned. It currently contains `Validation`,
`NotFound`, `Conflict`, `Unauthorized` and `Failure`, in addition to `None`. Application
chooses the code and description; it does not choose an HTTP status code or construct
`ProblemDetails`.

Expected business outcomes are returned as results. Unexpected exceptions are allowed to
travel to the Presentation boundary, where the global exception middleware handles them.
The database adapter may translate a known database constraint into an application
`Error.Conflict`; an unknown database exception remains an exception.

## Consequences

- Application remains independent of HTTP and can be tested with plain result values.
- Expected outcomes are explicit in handler contracts and are not normal exception flow.
- Presentation has one shared vocabulary to translate into HTTP errors.
- New expected business outcomes require an appropriate `ErrorCode` and Presentation
  mapping, with tests for the result and the HTTP behavior.
- The response mapping rules and JSON envelope are documented separately in
  `docs/decisions/006-responsePattern.md`.

## Agreed by

| Name | Date agreed | Objection |
|---|---|---|
| Bayon Eerhart | | |
| Nick Hoebe | | |
| Wesley Oerlemans | | |
| Gurpreet Singh | | |
| Kelvin van Voorst | | |

## References

- `docs/team/codeConventions.md`, Backend rules 7–13.
- `docs/architecture/architecture.md`, Application results and HTTP errors.
- `api/Connect.Application/Common/Results/Result.cs`.
- `api/Connect.Application/Common/Results/Result{T}.cs`.
- `api/Connect.Application/Common/Results/Error.cs`.
- `api/Connect.Application.Tests/Common/Results/ResultTests.cs`.
- `docs/decisions/006-responsePattern.md`.

## AI use declaration

AI was used as a writing support tool while working on this document. AI was used to
inspect the existing implementation and tests, and to improve the structure, wording,
readability and clarity of the documentation. The code behavior and decision content
were checked against the repository before inclusion.

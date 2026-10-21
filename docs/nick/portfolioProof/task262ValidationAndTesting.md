# Proof: validation, test strategy and remaining risk (#262)

MR [!50](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/merge_requests/50), task [#262](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/262). Profile: Full Stack Software Developer, level 3. Portflow candidates: OR2, OR4, AA1, AA2, AA4, MS1 and MS4.

## Result

The slice is validated at its meaningful boundaries: result invariants, request validation, endpoint behavior, error mapping, registration conflicts, database constraints and application composition. The proof reports both green evidence and what remains unproven.

## Test run

The supplied local run used:

```text
dotnet test C:\Users\Nickh\Documents\Repos\School\Year-4\Connect-Nine\api\Connect.slnx --no-restore --disable-build-servers -m:1 -v:minimal
```

It completed successfully in 44.7 seconds:

```text
Test summary: total: 102; failed: 0; succeeded: 102; skipped: 0; duration: 43,5s
Build succeeded in 44,7s
```

Project totals were 6 Application tests, 61 Infrastructure tests, 21 Presentation tests and 14 whole-application tests. The output also captured account success/conflict/validation responses, a database exception mapped to 503, and database-unreachable health responses without exposing the exception publicly. The execution date was 21 October 2026; the test output should be retained with the final portfolio evidence.

## OR4 — systematic validation

Use a rule table with four columns: rule, focused test/check, observed result, and limitation. The current evidence supports these rows:

| Rule | Focused test/check | Observed result | Limitation |
|---|---|---|---|
| Result cannot contain an invalid success/failure combination | `ResultTests` | 4 result-invariant tests passed | No captured mutation run |
| Invalid request does not reach the handler | `PostAccount_InvalidRequest_ReturnsValidationProblemWithCamelCaseKeys` | 400 and `handlerCalled == false` | Only account creation is covered |
| Known duplicate becomes a conflict | `RegisterAsync_WhenIdentityReportsDuplicate_ReturnsConflict`, database unique-violation tests and endpoint conflict test | Application conflict and HTTP 409 | Concurrency is tested at the database layer, not as a full HTTP race |
| Unexpected database exception becomes a safe 503 | Middleware and endpoint database-exception tests | 503 Problem Details; password absent | Broad `DbException` classification |
| Real database constraints are enforced | `AccountDatabaseTests` against PostgreSQL | Case-insensitive uniqueness, check constraints and concurrent insert behavior | Requires configured PostgreSQL |
| Application composition is wired | Whole-application endpoint tests | 14 whole-application tests passed | Does not prove deployment behavior |

The final evidence includes the supplied `dotnet test` output, command, date and database context. It reports 102 passed, 0 failed and 0 skipped. The output is useful because it names the four test assemblies and captures whole-application log behavior, but it does not prove coverage of every branch.

## OR2 and AA1 — method and adjustment

The test strategy combines fast unit tests for results, validators and mapping with integration/whole-application tests and real PostgreSQL tests for provider-specific behavior. This is an adjustment to the problem definition: persistence correctness cannot be established by a lightweight provider alone.

## AA2 and AA4 — risks and advice

The critical assumptions to challenge are: that a passing in-memory test represents PostgreSQL, that a service-level uniqueness check is enough under concurrency, that an exception’s public text is safe, and that a proposed helper counts as implemented. The recommendation is to use the cheapest test at each boundary and reserve real PostgreSQL for constraints, migrations, Identity behavior and provider-specific semantics.

## MS1 — transparent handover

The proof keeps the work transferable by naming the commands, test projects, database prerequisites, source boundaries and known gaps. A new developer should be able to see which test protects which rule and which evidence still needs to be collected.

## MS4 — joint result

The tests connect Application, Infrastructure, Presentation and the host composition. Decisions 005–006, API documentation and security documentation make the slice usable by teammates rather than leaving the behavior implicit in code.

## Known limits

Unless separately tested, do not claim full HTTP-to-Identity coverage, password-hash verification, race testing, test-server behavior or production readiness. Future improvements 001–003 and the validation-result extension remain proposals until their code and tests exist.

## Evidence checklist

- [x] Paste the full test summary with execution context; see [the test run above](#test-run).
- [ ] Record at least one removed-rule experiment for each major boundary, only if performed.
- [x] Add the real PostgreSQL configuration and provider comparison: [`TestConfiguration.cs`](../../../api/Connect.Infrastructure.Tests/TestConfiguration.cs), [`docker-compose.yml`](../../../docker-compose.yml), the InMemory package in [`Connect.Infrastructure.Tests.csproj`](../../../api/Connect.Infrastructure.Tests/Connect.Infrastructure.Tests.csproj), and the real-provider tests in [`AccountDatabaseTests.cs`](../../../api/Connect.Infrastructure.Tests/Persistence/AccountDatabaseTests.cs).
- [x] Add pipeline evidence: [backend-tests job 1345780](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/jobs/1345780). A separate test-server deployment check is still not evidenced here.
- [ ] Add a reviewer or teammate handover check for MS1/MS4.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

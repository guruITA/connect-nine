# Proof: task 1.13 The code convention on shared test setup (#1547)

Merge request !79, branch `docs/1544-null-forgiving-operator`, together with tasks 1.10 to 1.12 (#1544, #1545, #1546). Issue: 01 The code of Connect in one safe place (#1). File: `docs/team/codeConventions.md`, backend rule 20.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk, counted.** On 21 October 2026 I counted the setup that test classes write out themselves, on dev at ba289a5 and on the tip of my open merge requests:
   - `CreateDbContext`, which builds the options with `UseNpgsql` and `UseSnakeCaseNamingConvention`, in four database test classes: `AccountDatabaseTests` and `HealthDatabaseTests` on dev, my `ProfileDatabaseTests` (!67) and my `TagDatabaseTests` (!72)
   - `FindPostgresException`, the check that PostgreSQL refused a write, in three: `AccountDatabaseTests`, `ProfileDatabaseTests` and `TagDatabaseTests`
   - `SaveAccount` in five files, `SaveProfile` in four and `SaveTag` in three
   - my `MemberProfileEndpointTests` (!68) keeps its own `SaveAccount`, `CreateFactory`, `ClientFor`, `ReadJson` and `UnreachableDatabaseFactory`, while `ProfileTestHelpers` (!72) holds helpers with the same names. Only `ReadJson` is still the same in both, the others have drifted apart.
2. **The risk has already happened once.** My `ProfileDatabaseTests` and `TagDatabaseTests` were written before !50 and build their setup themselves, so the move of !50 to `TestsConnection` did not reach them. They still read `DefaultConnection`, which on a laptop is the database the server runs on, and they build and pass. #1541 now has to change each copy. Once they use one helper, the next change of the setup is one change.
3. **The rule.** Setup that more than one test class needs lives in one helper of the test project, which the test or its `[SetUp]` calls. A value the check depends on is passed to the helper in the test, so what a test checks stays readable in the test and a helper only takes away setup.
4. **The source.** Microsoft's guide on testing against a real database puts that setup in one fixture with a `CreateContext()` method, because "your test suite has a single database that's shared between multiple tests across multiple test classes" (Microsoft, 2022).
5. **Kept transferable.** A teammate who writes a new database test calls one helper next to `TestConfiguration`, instead of copying the options from a class that may already be out of date.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rule comes from.** Nick's message to me of 21 October 2026: "For example, we could make the `DbContext` setup reusable across test classes, similar to the existing `TestConfiguration` helper, and extract common test data creation or exception checks where appropriate." And: "The goal is to reduce duplication without hiding the actual test logic behind too many abstractions." Rule 20 says both parts.
2. **The rule follows the agreed code.** The team agreed that Nick's .NET code sets the way. His `TestConfiguration` on dev is a helper of exactly this kind, and the rule puts the `ConnectDbContext` helper next to it.
3. **Nothing of a teammate has to change now.** A test class that builds its own setup stays until its file changes for another reason, so `AccountDatabaseTests` and `HealthDatabaseTests` can move to the helper when their owners touch them.
4. **My own tests follow it.** My database and endpoint tests move onto the helpers with the rebase of #1542. That is step 1 of my note on #1541 (note 1998081), and my answer to Nick on !77 (note 1998152).

## TODO

- [x] Nick: confirm in the review that rule 20 says what he proposed in his message of 21 October 2026. Nick ticked this in !79 on 25 October 2026 and approved it the same day.

## References

Microsoft. (2022). *Testing against your production database system*. Microsoft Learn. Retrieved October 21, 2026, from https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

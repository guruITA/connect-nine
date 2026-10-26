# Proof: task 1.10 The code convention on the null-forgiving operator (#1544)

Merge request !79, branch `docs/1544-null-forgiving-operator`, together with task 1.11 (#1545). Issue: 01 The code of Connect in one safe place (#1). File: `docs/team/codeConventions.md`, backend rule 17.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk.** Every project of Connect has `<Nullable>enable</Nullable>` and `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` (backend rule 2), so a value that can be empty and is used without a check stops the build. The operator `!` after a value switches that warning off for that value without checking anything. The mistake then shows up later, as a NullReferenceException while a member uses Connect. Rule 17 asks to write the check instead.
2. **Measured, not assumed.** I built five small programs with the .NET 10 SDK (10.0.401) and the same two settings. Each looks up an account by an email address that is not in the list, or reads a property nobody set:
   - `found.Email` without a check: the build failed with `error CS8602: Dereference of a possibly null reference.`
   - `found!.Email`: the build succeeded, and the run stopped with `System.NullReferenceException: Object reference not set to an instance of an object.`
   - `found ?? throw new InvalidOperationException("No account with this email address.")`: the build succeeded, and the run stopped with that sentence, which says what went wrong.
   - a property `public Account Account { get; set; } = null!` that nobody set: the build succeeded, and reading it gave the same NullReferenceException.
   - the same property as `public required Account Account { get; set; }`: the build failed with `error CS9035: Required member 'Profile.Account' must be set in the object initializer or attribute constructor.`

   So `!` moves an error from the build, where a developer sees it, to the moment a member uses Connect. `?? throw` and `required` keep it with the developer.
3. **The source.** Microsoft's page on the operator says: "The null-forgiving operator has no effect at run time. It only affects the compiler's static flow analysis by changing the null state of the expression." (Microsoft, n.d.-a). That is what the second and fourth program showed.
4. **When `!` is still the right tool.** The same page gives testing argument validation as its first use: a test passes `null!` on purpose to see the guard throw. That is the one case rule 17 keeps, with `Result<string>.Success(null!)` in `ResultTests` as the example.
5. **Fields of a test.** Nick's note on !68 (MS4, point 1) was on `private CreateProfileCommandHandler handler = null!;`, a field of a test that `[SetUp]` fills. I built a test project with NUnit 4.4.0 and NUnit.Analyzers 4.10.0, the versions of our test projects, three times:
   - the field without a value, filled in `[SetUp]`: the build succeeded and the test passed
   - the same without NUnit.Analyzers: the build failed with `error CS8618: Non-nullable field 'handler' must contain a non-null value when exiting constructor.`
   - the field without a value and no `[SetUp]` that fills it: the build failed with the same CS8618 and with `error CS0649: Field 'HandlerTests.handler' is never assigned to`

   So NUnit.Analyzers lets a `[SetUp]` field go without `= null!`, and only when `[SetUp]` really fills it. Rule 17 says so, with `AccountDatabaseTests` as the example.
6. **The other place `!` stays.** Microsoft's guide on EF Core and nullable reference types says about a query over an optional navigation: "it is necessary to use the null-forgiving operator (!) to inform the compiler that an actual null value isn't possible" (Microsoft, n.d.-b). EF Core turns such a query into SQL, so the value is never read in .NET. Rule 17 names it as the second case, with `link.Tag!.Name` as the example.
7. **Kept transferable.** The rule names what to write instead, with names from our own code, so a teammate who is new to .NET can follow it without asking. It is checked by the reviewer, because there is no tool that flags every `!` after a value.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rule comes from.** Nick's review of my !68 (note 1992389, 21 October 2026) on `CreateProfileCommandHandlerTests`: "No null! please". Rule 17 makes that a rule for the whole team, with what to write instead.
2. **The rule follows the agreed code.** The team agreed that Nick's .NET code sets the way. His `Profile` in `api/Connect.Domain/Users/Profile.cs` already has `public required Account Account { get; set; }` instead of `= null!`, so rule 17 names it as the example. His `AccountDatabaseTests` declares its `[SetUp]` fields without a value, which is the example for the fields of a test.
3. **What is on dev now.** On dev at 0a4248d2 there are two `!` after a value, both in Nick's tests: `Result<string>.Success(null!)` in `ResultTests`, a test of a guard that the rule keeps, and a cast of a logged value in `AccountRegistrationServiceTests`. Both stay. A `!` that is already in the code changes only when its file changes for another reason, the same way the stub classes were handled in task 3.12 (#1543), so no merge request of a teammate has to change for this rule.
4. **My own code follows it.** My own open merge requests follow the rule too, each with one commit of 21 October 2026: !67 (87e150fc), !68 (7b4dcf55), !70 (ef857443), !71 (23686653), !72 (af3a607d), !73 (fe82da8f), !74 (c8171392), !75 (d84be071) and !76 (05778d9d). In !76, which holds the whole profile chain, 59 lines lose their `!`, 19 of them `[SetUp]` fields that now have no value, and in !74 three lines, one of them a field. The other 40 in !76 are rewritten as the rule says: `result is { IsSuccess: true, Value: { } profile }` in the endpoints, one check after the validator in the address that saves a profile, `?.` in a test where a missing value already fails the check, and `?? throw new AssertionException(...)` where the check is `Is.Null`, because `?.` would let that pass without a value. What stays is `link.Tag!.Name` inside the EF Core queries, eight lines in !76. These branches were cut before the pipeline had a build and test job (#266), so their own pipeline does not test them. I built each one here with the .NET 10 SDK, ran `dotnet format --verify-no-changes`, and ran every test project on PostgreSQL 16.15, each on a database of its own: no warnings, and every test green except Nick's older `AccountTableTests` in the base of these branches, which fails the same way on the branches before my commit and which !50 replaced on dev with `AccountDatabaseTests`.
5. **No clash with open work.** !58 changes backend rule 16, right above the new rule, and !59 and !77 change other parts of the same file. My open merge requests also add rows to `docs/wesley/README.md`. I held every open merge request against this branch with `git merge-tree`: the ones that merge with dev merge with this branch too, and !68 and !70 to !76, which already need a rebase onto dev (#1542), get no new conflicting file from it.

## TODO

- [ ] Nick: confirm in the review that rule 17 says what he asked for in his review of !68, and that `required` is what he wants for a property that must be set.
- [ ] A teammate who did not write the rule: read rule 17 and write in the review what was unclear.

## References

Microsoft. (n.d.-a). *! (null-forgiving) operator (C# reference)*. Microsoft Learn. Retrieved October 21, 2026, from https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving

Microsoft. (n.d.-b). *Working with nullable reference types*. Microsoft Learn. Retrieved October 21, 2026, from https://learn.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

# Proof: task 1.10 The code convention on the null-forgiving operator (#1544)

Merge request !79, branch `docs/1544-null-forgiving-operator`. Issue: 01 The code of Connect in one safe place (#1). File: `docs/team/codeConventions.md`, backend rule 17.
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
3. **The source.** Microsoft's page on the operator says: "The null-forgiving operator has no effect at run time. It only affects the compiler's static flow analysis by changing the null state of the expression." (Microsoft, n.d.). That is what the second and fourth program showed.
4. **When `!` is still the right tool.** The same page gives testing argument validation as its first use: a test passes `null!` on purpose to see the guard throw. That is the one case rule 17 keeps, with `Result<string>.Success(null!)` in `ResultTests` as the example.
5. **Kept transferable.** The rule names what to write instead, with names from our own code, so a teammate who is new to .NET can follow it without asking. It is checked by the reviewer, because there is no tool that flags every `!` after a value.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **The rule follows the agreed code.** The team agreed that Nick's .NET code sets the way. His `Profile` in `api/Connect.Domain/Users/Profile.cs` already has `public required Account Account { get; set; }` instead of `= null!`, so rule 17 names it as the example.
2. **What is on dev now.** On dev at 0a4248d2 there are two `!` after a value, both in Nick's tests: `Result<string>.Success(null!)` in `ResultTests`, a test of a guard that the rule keeps, and a cast of a logged value in `AccountRegistrationServiceTests`. Both stay. A `!` that is already in the code changes only when its file changes for another reason, the same way the stub classes were handled in task 3.12 (#1543), so no merge request of a teammate has to change for this rule.
3. **No clash with open work.** !58 changes backend rule 16, right above the new rule, and !59 and !77 change other parts of the same file. My open merge requests also add rows to `docs/wesley/README.md`. I held every open merge request against this branch with `git merge-tree`: the ones that merge with dev merge with this branch too, and !68 and !70 to !76, which already need a rebase onto dev (#1542), get no new conflicting file from it.

## TODO

- [ ] Nick: confirm in the review that rule 17 fits the way he builds, and that `required` is what he wants for a property that must be set.
- [ ] A teammate who did not write the rule: read rule 17 and write in the review what was unclear.

## References

Microsoft. (n.d.). *! (null-forgiving) operator (C# reference)*. Microsoft Learn. Retrieved October 21, 2026, from https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

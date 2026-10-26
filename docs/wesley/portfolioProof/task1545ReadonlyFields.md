# Proof: task 1.11 The code convention on fields for what the constructor gets (#1545)

Merge request !79, branch `docs/1544-null-forgiving-operator`, together with task 1.10 (#1544). Issue: 01 The code of Connect in one safe place (#1). File: `docs/team/codeConventions.md`, backend rule 18.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk.** A class can take what it needs in a primary constructor, such as `CreateProfileCommandHandler(IAppDbContext dbContext)`, and use `dbContext` in every method. That parameter is not readonly. A method can give it another value by mistake, and from then on every later call on that object works with the wrong dependency. Rule 18 puts the value in a `private readonly` field, which nothing can change after the object is made.
2. **Measured, not assumed.** I built four small programs with the .NET 10 SDK (10.0.401), Nullable and TreatWarningsAsErrors, each a handler with a primary constructor that gets an `IAppDbContext`:
   - a method that gives the parameter another context: the build succeeded, and the run printed the name of the other context instead of the one the handler got
   - the same on a `private readonly` field: the build failed with `error CS0191: A readonly field cannot be assigned to (except in a constructor or init-only setter of the type in which the field is defined or a variable initializer)`
   - a field, and the parameter used in a method as well: the build failed with `error CS9124: Parameter 'IAppDbContext dbContext' is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.`
   - the field alone: the build succeeded, and the run used the context the handler got

   So once a class has the field, the compiler keeps the rule, and the mistake of the first program cannot be made.
3. **The source.** Microsoft's page on primary constructors: "Primary constructor parameters can be assigned to." (Microsoft, n.d.). That is what the first program showed.
4. **When no field is needed.** A parameter that only goes to the base class, such as `options` of `ConnectDbContext`, is never used in a method, so it needs no field. That is the "when possible" of the rule.
5. **Kept transferable.** The rule shows the one line to write, with the names of our own code, so a teammate who is new to .NET can follow it without asking.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rule comes from.** Nick's review of my !68 (note 1992230, 21 October 2026) on `CreateProfileCommandHandler`: "Could u put the dbContext in a private readonly property? Right now your using primary constructor, and the dependency is used as an parameter. This can sometimes cause issues, I therefor recommend using a private readonly property." Rule 18 makes that a rule for the whole team, so the next class is written that way from the start.
2. **The rule follows the agreed code.** The team agreed that Nick's .NET code sets the way. His `CreateAccountCommandHandler` and `AccountRegistrationService` on dev already have the fields `_registrationService`, `_userManager` and `_logger`, named as rule 4 says, and rule 18 names them as the example.
3. **What is on dev now.** On dev at 0a4248d2, five classes have a primary constructor. Two have the field, one only passes its parameter to the base class, and two use their parameters directly. Those two stay until their file changes for another reason, so no merge request of a teammate has to change for this rule.
4. **My own code follows it.** My open merge requests got one commit each on 21 October 2026, the same commits as for rule 17: `CreateProfileCommandHandler` (from !68, where Nick asked), `GetCurrentProfileQueryHandler` (from !70), `GetPublicProfileQueryHandler` (from !71), `AddProfileTagCommandHandler` and `ListSuggestedTagsQueryHandler` (from !72) keep `_dbContext`, `TemporaryAccountHandler` (from !68) keeps `_environment`, and `TestDataSeeder` (!74) keeps `_userManager`, `_configuration` and `_logger`. `SqliteAppDbContext` and `ConnectDbContext` only pass `options` to their base class and need no field. Each branch builds with no warnings, and its tests are green as described in the proof of task 1.10 (#1544).
5. **No clash with open work.** Rule 18 sits right below rule 17 in the same merge request. I held every open merge request against this branch with `git merge-tree`: the ones that merge with dev merge with this branch too, and !68 and !70 to !76, which already need a rebase onto dev (#1542), get no new conflicting file from it.

## TODO

- [x] Nick: confirm in the review that rule 18 says what he asked for in his review of !68. Nick ticked this in !79 on 21 October 2026 and approved !79 on 25 October 2026.

## References

Microsoft. (n.d.). *Declare primary constructors for classes and structs*. Microsoft Learn. Retrieved October 21, 2026, from https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/primary-constructors

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

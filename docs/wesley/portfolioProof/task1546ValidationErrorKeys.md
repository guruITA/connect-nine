# Proof: task 1.12 The code convention on the keys of a validation answer (#1546)

Merge request !79, branch `docs/1544-null-forgiving-operator`, together with tasks 1.10, 1.11 and 1.13 (#1544, #1545, #1547). Issue: 01 The code of Connect in one safe place (#1). File: `docs/team/codeConventions.md`, backend rule 19.
Profile: Full Stack Software Developer, level 3. Portflow: Analysing and Advising AA2, Managing and Collaborating MS4.

A point that needs someone else is marked TODO with who and what.

## AA2 Questioning sources and my own assumptions

School asks: "You show an inquisitive and critical attitude by questioning sources, AI results, the problem statement and your own assumptions."

1. **The question.** Nick asked the team on 21 October 2026 how to camel-case the keys of the errors FluentValidation gives, with four options: keep the global resolver, use `JsonNamingPolicy.CamelCase` in it, set the `DictionaryKeyPolicy` of the JSON options, or an extension on `ValidationResult` that each endpoint calls. His words: "For `Addresses[0].PostalCode`, the desired key is `addresses[0].postalCode`. I suggest option 4. Any concerns?"
2. **Held against a run, not taken on trust.** I built a small program with the .NET 10 SDK (10.0.401), FluentValidation 12.0.0, Nullable and TreatWarningsAsErrors. Its validator has a flat field (`FirstName`), a nested rule (`RuleFor(r => r.Address.PostalCode)`), a collection with a child validator (`RuleForEach(r => r.Addresses)`) and a name with capitals (`URLValue`). The keys each option gave:
   - the resolver on dev: `firstName`, `postalCode`, `addresses[0].postalCode`, `uRLValue`
   - option 2, `JsonNamingPolicy.CamelCase` in the resolver: `firstName`, `postalCode`, `addresses[0].postalCode`, `urlValue`
   - option 4, the extension without the resolver: `firstName`, `address.postalCode`, `addresses[0].postalCode`, `urlValue`

   So the resolver drops the parent of a nested rule, as Nick wrote. The collection kept its parent with every option, because FluentValidation adds the path of a child validator itself.
3. **What the run showed that the question did not say.** With the resolver still set, the extension gave `postalCode` and `uRLValue` as well: the parent is already gone before the extension runs. Option 4 only works when the resolver goes, so rule 19 says both. Without the resolver, `validation.ToDictionary()` answered `FirstName` and `Address.PostalCode`, the C# names, which is why the rule asks an endpoint test to check each key.
4. **The sources.** FluentValidation's documentation: "By default, the name of the property extracted from the MemberExpression passed to RuleFor", and a collection gets its index "within square brackets (for example Foo.BarList[5].Baz)" (FluentValidation, n.d.). Microsoft lists the web defaults of System.Text.Json, which ASP.NET Core uses, with "PropertyNamingPolicy = CamelCase" (Microsoft, 2024). So a key in camelCase is the name the field already has in the JSON the screen sends.
5. **Not claiming what the code does not do yet.** On dev at ba289a5 the resolver is set in `api/Connect.Presentation/DependencyInjection.cs` and `AccountEndpoints` calls `validation.ToDictionary()`. Rule 19 says those stay until the extension is on dev, instead of describing code that is not there. The validator tests with `FluentValidation.TestHelper` (rule 8 of the part Tests and mocking, !77) found their field with and without the resolver, so they do not change.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rule comes from.** Nick's question of 21 October 2026, and his proposal, option 4. The team agreed that Nick's .NET code sets the way, so rule 19 writes down the way he proposed, with what the run added to it.
2. **My answer.** On !68 (note 1998153) I answered that I have no concerns, with the two points of the run: the resolver has to go, and an endpoint that still calls `ToDictionary()` then answers `FirstName`.
3. **What stays the same for the team.** Every field we have today is flat, so all four options give the same keys: `email` and `password` (`AccountEndpointTests` on dev), `firstName`, `ageBand`, `region` and `bio` (`MemberProfileEndpointTests`, !68) and `name` (`MemberProfileTagEndpointTests`, !72). The screen reads them in `accounts.ts` on dev and in `memberProfiles.ts` and `memberProfileTags.ts` in my open merge requests. Nothing in the frontend has to change.
4. **My own code follows it.** My `MemberProfileEndpoints` (!68) and `MemberProfileTagEndpoints` (!72) switch to the extension once it is on dev, with the rebase of #1542. That is step 2 of my note on #1541 (note 1998081).
5. **What Gurpreet's review found.** In his review of !79 on 23 October 2026, Gurpreet Singh read rules 12 and 19 side by side and saw two things. Backend rule 12 still said `Results.ValidationProblem`, while rule 19 and the code on dev use `TypedResults`, a second way in one file. And rule 19 said the extension comes later without saying what a new endpoint does until then. Rule 12 now says `TypedResults` for every error answer, with Microsoft's "Returning TypedResults is preferred to returning Results.", and names the one place on dev that still uses `Results`, the health answer of #311, which follows once its file changes. Rule 19 now says that until the extension is on dev a new endpoint answers the way `AccountEndpoints` does and its test checks each key, that a form with a nested field needs the extension first, and that the merge request of the extension changes every endpoint in the same pass, so the code never holds both ways.

## TODO

- [ ] Nick: confirm in the review that rule 19 says what he proposed on 21 October 2026, and name the task that brings the extension to dev.

## References

FluentValidation. (n.d.). *Overriding the message*. FluentValidation documentation. Retrieved October 21, 2026, from https://docs.fluentvalidation.net/en/latest/configuring.html

Microsoft. (2024). *How to instantiate JsonSerializerOptions with System.Text.Json*. Microsoft Learn. Retrieved October 21, 2026, from https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/configure-options

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

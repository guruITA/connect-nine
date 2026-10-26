# Proof: task 1.23 The code convention on JsonPropertyName (#1565)

Merge request !116, branch `docs/1559-review-rules`. Issue: the code conventions with a reason for every rule (#1549). File: `docs/team/codeConventions.md`, backend rule 21.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk.** Five of my answer records carried `[JsonPropertyName]` for every property: `CreateMemberProfileResponseData`, `MemberProfileResponseData`, `PublicMemberProfileResponseData`, `TagResponse` and `SuggestedTagsMeta`. The names then live in two places, and a renamed property can drift from its key.
2. **What the source says.** Microsoft: the web defaults of System.Text.Json, which ASP.NET Core uses, include "PropertyNamingPolicy = CamelCase" (Microsoft, 2024).
3. **Measured.** Without the attributes, the endpoint tests that read each key pass: 159 tests on !68, and every test project on PostgreSQL on each branch up to 291 tests on !76. `JsonIgnore` for a value that is left out stays, because it does something camelCase does not.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rule comes from.** Nick Hoebe's reviews of !70 on 24 October 2026 and of !68 on 25 October 2026: "`JsonPropertyName` is not needed aspnet core automatically converts c# object property names from and to camelCase. The `JsonPropertyName` that I added were also not needed. This is true for all dto records."
2. **His records stay as they are.** `CreateAccountResponseData` and `Response` keep their attributes until their file changes for another reason. I asked him on !68 whether he takes them out himself.
3. **When an attribute is still right.** Only where the JSON name must differ from the camelCase of the C# name, with the reason beside it and an endpoint test on the key.

## TODO

- [ ] A teammate who did not write the rule: read backend rule 21 in the review and write what was unclear. The answer stays in the review, and a change it leads to comes before anyone approves.
- [ ] Wesley: after the merge, tick the boxes of #1565 with the evidence in a comment. No change to this branch for it.

## References

Microsoft. (2024). *How to instantiate JsonSerializerOptions with System.Text.Json*. Microsoft Learn. Retrieved October 26, 2026, from https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/configure-options

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

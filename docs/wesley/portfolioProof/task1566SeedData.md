# Proof: task 1.24 The code convention on seed data (#1566)

Merge request !116, branch `docs/1559-review-rules`. Issue: the code conventions with a reason for every rule (#1549). File: `docs/team/codeConventions.md`, database rule 11.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk.** Two kinds of seed data are coming: lists every database needs, such as the starter tags and later a list of regions, and rows only for a test, such as the test member of #276. Mixed up, a list never reaches production, or a test account does.
2. **What the source says.** Microsoft: model managed data is "most useful for static data that's not expected to change outside of migrations and does not depend on anything else in the database, for example ZIP codes", and for "Temporary data for testing" it advises another way (Microsoft, n.d.-a).
3. **Measured on the branches.** The twenty starter tags of !72 come with `HasData` in `ConnectDbContext` and are written by the migration `AddTagTables`, and the database test `CountAsync_TheStarterTags_CountsTwenty` passes on PostgreSQL. The test member of !74 is added at start by `TestDataSeeder` and never in Production.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rule comes from.** Nick Hoebe's review of !74 on 24 October 2026: "EF Core does have it's own data seeding strategies, so for more permanent production seeding we should consider using that. For example the regions for a profile, or other custom lookup data."
2. **It follows the code.** The rule names the two ways the branches already use, so no code changes for it.
3. **Why test data never goes in a migration.** A migration runs on every database, production included, and a test account there would be a real account with a known password.

## TODO

- [ ] A teammate who did not write the rule: read database rule 11 in the review and write what was unclear. The answer stays in the review, and a change it leads to comes before anyone approves.
- [ ] Wesley: after the merge, tick the boxes of #1566 with the evidence in a comment. No change to this branch for it.

## References

Microsoft. (n.d.-a). *Data seeding*. Microsoft Learn. Retrieved October 26, 2026, from https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

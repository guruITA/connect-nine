# Proof: task 1.25 The code convention on a migration after a rebase (#1567)

Merge request !116, branch `docs/1559-review-rules`. Issue: the code conventions with a reason for every rule (#1549). File: `docs/team/codeConventions.md`, database rule 12.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk.** EF Core applies migrations in the order of their name, which starts with the date and time they were made. A branch that waits while newer migrations land on dev keeps a migration with an older name.
2. **Measured.** The migration of !76 was made on 20 October 2026 as `20261020105652_AddParentConsentTable`. On 26 October 2026, with those files on a new database, `dotnet ef database update` ran it third, before `AddTagTables` creates the extension `citext`, and it stopped with `42704: type "citext" does not exist`. Its designer file also still held the old shape of `profile_tag`. Made again with `dotnet ef migrations add`, it is `20261026053226_AddParentConsentTable` with the same `Up` and `Down`, all five migrations apply in order, and `has-pending-model-changes` finds no change.
3. **What the source says.** Microsoft: "resolve it by re-creating your migration" and "Don't resolve parallel migrations by sorting or renaming their files" (Microsoft, n.d.-b).

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rule comes from.** Gurpreet Singh's review of !76 on 23 October 2026: "20261020105652 sorts before AddMemberDetailsToProfile (20261021...), so the FK to profile breaks on a fresh db." On the rebuilt branch the first error was the missing `citext` instead, from the same cause.
2. **The check for a review.** `dotnet ef database update` on an empty database, which the pipeline's tests already do for every branch.
3. **Why a renamed file is not enough.** The designer file of a migration holds the model at that point, so a new name on old files keeps a model without the migrations it should build on.

## TODO

- [ ] A teammate who did not write the rule: read database rule 12 in the review and write what was unclear. The answer stays in the review, and a change it leads to comes before anyone approves.
- [ ] Wesley: after the merge, tick the boxes of #1567 with the evidence in a comment. No change to this branch for it.

## References

Microsoft. (n.d.-b). *Migrations in team environments*. Microsoft Learn. Retrieved October 26, 2026, from https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/teams

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

# Proof: task 1.21 The code convention on shared answer types (#1563)

Merge request !116, branch `docs/1559-review-rules`. Issue: the code conventions with a reason for every rule (#1549). File: `docs/team/codeConventions.md`, frontend rule 15.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk.** The same shape, the messages of a wrong form, was declared three times: `ValidationProblem` in `accounts.ts` and `ValidationProblemBody` in `memberProfiles.ts` and `memberProfileTags.ts`. A change to the answer of the server, such as the keys of backend rule 19, would have to be made in three places.
2. **The rule.** A shape that more than one file of src/api reads is declared once in `client.ts` and imported with `import type` (rule 12 of General and Git, one way everywhere).
3. **Measured.** `ValidationProblem` is declared once in `client.ts`, and the three files import it. Lint, Prettier, the build and every Vitest test pass on each branch.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rule comes from.** Nick Hoebe's review of !70 on 24 October 2026: "`ValidationProblem` exists in account.ts. Perhaps this and other such types could be shared?"
2. **In his file, only that line.** `accounts.ts` imports the shared type instead of declaring its own, and his tests pass without an edit.
3. **Why client.ts.** It is the one file every api file already imports, and the shape is part of what the server sends, like `RequestError`.

## TODO

- [ ] A teammate who did not write the rule: read frontend rule 15 in the review and write what was unclear. The answer stays in the review, and a change it leads to comes before anyone approves.
- [ ] Wesley: after the merge, tick the boxes of #1563 with the evidence in a comment. No change to this branch for it.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

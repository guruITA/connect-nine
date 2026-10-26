# Proof: task 1.19 The code convention on a hook per page (#1561)

Merge request !116, branch `docs/1559-review-rules`. Issue: the code conventions with a reason for every rule (#1549). File: `docs/team/codeConventions.md`, frontend rule 13.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk.** Create your profile, My profile and the page of another member held their state and their calls to the server in the page itself, while Create account keeps them in `useCreateAccount`. A page that mixes showing and loading is harder to read and to test, and a new member sees two ways.
2. **The rule.** The state of a form or a page and its calls to src/api live in a hook `use<What>` in `hooks/`. React: "The code of your components expresses your intent, not the implementation." (Meta Platforms, n.d.).
3. **Measured.** `useCreateProfile` (!69), `useMyProfile` (!70, with the tags of !72) and `useMemberProfile` (!71) took the state over. The page tests mock the api files as before and pass without an edit.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rule comes from.** Nick Hoebe's review of !70 on 24 October 2026: "Consider putting these in a custom hook, keeps your component more focused & readable", and on My profile: "Also here consider using a custom hook. Like use createAccount.ts".
2. **It follows his code.** The hooks have the shape of his `useCreateAccount`: the hook holds the state and the calls to the server, and the page gets values and functions back.
3. **Why the page tests stay unchanged.** They test what the page shows for each answer of the api file, and that did not change, so they prove the move kept the behaviour.

## TODO

- [ ] A teammate who did not write the rule: read frontend rule 13 in the review and write what was unclear. The answer stays in the review, and a change it leads to comes before anyone approves.
- [ ] Wesley: after the merge, tick the boxes of #1561 with the evidence in a comment. No change to this branch for it.

## References

Meta Platforms. (n.d.). *Reusing logic with custom Hooks*. React. Retrieved October 26, 2026, from https://react.dev/learn/reusing-logic-with-custom-hooks

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

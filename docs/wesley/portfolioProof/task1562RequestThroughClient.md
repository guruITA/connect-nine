# Proof: task 1.20 The code convention on request() of client.ts (#1562)

Merge request !116, branch `docs/1559-review-rules`. Issue: the code conventions with a reason for every rule (#1549). File: `docs/team/codeConventions.md`, frontend rule 14.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk.** My profile code still had a temporary helper `http.ts` from before Nick's `client.ts` existed, and three api files read the status of fetch in ways of their own. MDN on fetch: "A fetch() promise does not reject if the server responds with HTTP status codes that indicate errors" (MDN contributors, n.d.), so every file that calls fetch itself has to check the status again.
2. **The rule.** Every function in src/api calls `request()` of `client.ts`, so a lost connection is always a `NetworkError` and an error status always a `RequestError` with its status and body. The header of the temporary account until log in (#15) is added in one small helper, `temporaryAccount.ts`.
3. **Measured.** `http.ts` is gone and `memberProfiles.ts`, `tags.ts` and `memberProfileTags.ts` call `request()`. Three tests of `tags.ts` and `memberProfileTags.ts` failed at first, because their stand-in for fetch gave no `ok`, which `request()` reads. With `ok` they pass, so the rule says that a test that replaces fetch gives `ok` too.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rule comes from.** Nick Hoebe's review of !70 on 24 October 2026: "This is temp, #264 has been added with !82. Should use client.ts".
2. **It follows his code.** `accounts.ts` already turns a `RequestError` into one outcome per status, and the profile and tag files now do the same.
3. **Older code.** No file on dev calls fetch outside `client.ts`, so nothing older has to wait for this rule.

## TODO

- [ ] A teammate who did not write the rule: read frontend rule 14 in the review and write what was unclear. The answer stays in the review, and a change it leads to comes before anyone approves.
- [ ] Wesley: after the merge, tick the boxes of #1562 with the evidence in a comment. No change to this branch for it.

## References

MDN contributors. (n.d.). *Window: fetch() method*. MDN Web Docs. Retrieved October 26, 2026, from https://developer.mozilla.org/en-US/docs/Web/API/Window/fetch

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

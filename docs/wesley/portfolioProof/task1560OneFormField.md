# Proof: task 1.18 The code convention on one form field (#1560)

Merge request !116, branch `docs/1559-review-rules`. Issue: the code conventions with a reason for every rule (#1549). File: `docs/team/codeConventions.md`, frontend rule 12.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk.** Create your profile had field markup of its own: a label, a hint, an input, a select or a textarea and a message per field, with about 50 lines of CSS for them, beside Nick's `FormField` of Create account. Two forms of one product looked different, and a fix for a screen reader in one would not reach the other.
2. **One field, grown where needed.** `FormField` now also draws a hint, a select and a textarea, and takes a `ref` for the focus on the first wrong field. Every part is tied with aria-describedby and aria-invalid. MDN: "The aria-describedby attribute lists the ids of the elements that describe the object." (MDN contributors, n.d.).
3. **Measured.** Create account renders the same as before, and its tests pass without an edit. On Create your profile, in Chrome against the server of !68 at 360 and 1280 pixels, axe found 0 problems in every state, and the focus went to First name with the four messages of the server.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rule comes from.** Nick Hoebe's review of !70 on 24 October 2026: "For create account I created FormField.tsx, perhaps reusable? Otherwise a custom create profile field is also fine. Or something dynamic." I took the dynamic way, so there is one field and not two.
2. **In his files, only what his review asked.** `FormField` gets new optional properties, and an input without them renders as before. The fields of Create your profile now have the corners and the red of Create account.
3. **A finding for the stand up.** The element selector `button` in `CreateAccount.module.css` also reaches other pages, because a CSS Module only scopes class names. The Save button of Create your profile sets its own width for now, and the point is on the list for the stand up.

## TODO

- [ ] A teammate who did not write the rule: read frontend rule 12 in the review and write what was unclear. The answer stays in the review, and a change it leads to comes before anyone approves.
- [ ] Wesley: after the merge, tick the boxes of #1560 with the evidence in a comment. No change to this branch for it.

## References

MDN contributors. (n.d.). *ARIA: aria-describedby attribute*. MDN Web Docs. Retrieved October 26, 2026, from https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-describedby

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

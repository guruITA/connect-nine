# Proof: task 1.22 The code convention on one header (#1564)

Merge request !116, branch `docs/1559-review-rules`. Issue: the code conventions with a reason for every rule (#1549). File: `docs/team/codeConventions.md`, frontend rule 16.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk.** Create your profile, My profile and the page of another member each put their title band in a `<header>`, below the `<header>` of `TopBar`. A new page copies what it sees.
2. **What the source says.** MDN: a header outside sectioning content and main "has an identical meaning to the site-wide banner landmark role", and inside them "it loses its landmark status" (MDN contributors, n.d.). So the second header was not wrong HTML. The rule is about one way: `TopBar` holds the one header of the site, and a page has a `div` or a `section` with its h1, as Create account does.
3. **Measured.** The three title bands are a `div` with the same class, so the pages look the same. Their tests and axe pass without a change.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rule comes from.** Nick Hoebe's review of !70 on 24 October 2026: "Is this a second header? TopBar.tsx already has a header. This is just semantics".
2. **It follows his code.** His Create account page has its title in a `section` with an h1 and no header.
3. **Why the rule does not claim broken HTML.** MDN says a header inside main is allowed, so the reason in the rule is one way everywhere, which a reviewer can check at a glance.

## TODO

- [ ] A teammate who did not write the rule: read frontend rule 16 in the review and write what was unclear. The answer stays in the review, and a change it leads to comes before anyone approves.
- [ ] Wesley: after the merge, tick the boxes of #1564 with the evidence in a comment. No change to this branch for it.

## References

MDN contributors. (n.d.). *<header>: The header element*. MDN Web Docs. Retrieved October 26, 2026, from https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/header

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

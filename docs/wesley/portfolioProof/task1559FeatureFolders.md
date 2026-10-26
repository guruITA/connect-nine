# Proof: task 1.17 The code convention on the folder of a piece of screen (#1559)

Merge request !116, branch `docs/1559-review-rules`. Issue: the code conventions with a reason for every rule (#1549). File: `docs/team/codeConventions.md`, frontend rule 7.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk.** Rule 7 said that `components/` holds pieces of screen used on more than one page and `hooks/` hooks used in more than one place. Dev already worked another way: on dev 8a4db18b, `components/` holds four pieces every feature uses (ButtonLink, FormField, SiteFooter and TopBar), `components/account/` holds `CreateAccountForm` and `components/home/` the seven parts of the home page, and `hooks/useCreateAccount.ts` is used by one page only. A new member who reads the rule puts files where nobody else does.
2. **The rule now says what dev does.** Pieces every feature uses in `components/`, the pieces of one feature in `components/<feature>/` with their test and their CSS Module next to them, and every hook in `hooks/`. React's page on file structure names this way: "One common way to structure projects is to locate CSS, JS, and tests together inside folders grouped by feature or route." (Meta Platforms, n.d.).
3. **Kept transferable.** The rule names the real folders and files, so the place of the next file can be read off without asking.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rule comes from.** Nick Hoebe's review of !70 on 24 October 2026: "Please move this component to components/profile", for `ProfileDetails`, its test and its styles.
2. **My own code follows it.** `ProfileDetails` moved with its test and its CSS to `components/profile/` in !70, and `ParentConsentBanner` of !76 sits there too. `TagInput`, `TagList` and `LoggedOutMessage` stay in `components/`, because organisation profiles, opportunities and every page behind log in will use them. The tests of every branch pass without an edit: 84 to 160 Vitest tests from !70 to !76.
3. **Why the rule follows dev.** Nick's `components/account/` and Bayon's and Gurpreet's `components/home/` were already one folder per feature. Changing the rule costs one line, moving their files would cost their merge requests a rebase for nothing.

## TODO

- [ ] A teammate who did not write the rule: read frontend rule 7 in the review and write what was unclear. The answer stays in the review, and a change it leads to comes before anyone approves.
- [ ] Wesley: after the merge, tick the boxes of #1559 with the evidence in a comment. No change to this branch for it.

## References

Meta Platforms. (n.d.). *File structure*. React. Retrieved October 26, 2026, from https://legacy.reactjs.org/docs/faq-structure.html

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

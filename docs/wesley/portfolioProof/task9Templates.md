# Proof: task 1.8 The templates (#9)

The templates, the Definition of Done and the team rules came in with my commits c3c79d4 (27 September 2026), f4361a2 and 05ace60. This merge request, branch `docs/9-merge-request-template`, adds the parts TODO and AI use to the merge request template, rule 10 of the code conventions and the template parts in docs/team/gitlabSetup.md. Issue: 01 The repository, the pipeline and the team rules (#1).
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating, MS1 and MS4. Link the merge request and this file.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk the templates remove.** Five people with five ideas of done, and proof for the portfolio that is forgotten until the end of the semester. Every issue type (slice, setup, research, decision, client) has its own template in `.gitlab/issue_templates/` with the Definition of Done of that type, and every merge request starts from `.gitlab/merge_request_templates/Default.md`. The new part TODO removes a third risk: work that waits on someone else and that nobody sees. Each open point now stands in the merge request with a name and a checkbox, so a teammate who opens it reads what is still needed without asking.

2. **Why the template asks for the failing test and the version to go back to.** They serve two lines of the Definition of Done for a slice (`docs/team/definitionOfDone.md`): "The merge request names one test and shows that it fails when its rule is removed" and "The merge request names the version to go back to if something breaks". A test that never failed proves nothing, and a change without a way back cannot be undone quickly on the test site. The parts Proof for the portfolio and AI use serve the line "The BoKSA proof is collected, and AI use is declared" of the other four types.

3. **How a change of the Definition of Done reaches every place it lives.** The source is section 3 of `docs/team/issueRules.md` (rule C10). It is copied word for word to `docs/team/definitionOfDone.md` and to the five issue templates. A change goes into all three in one merge request, and the reviewer compares them. Issues that already exist keep their copy; the team updates an open issue only when the change matters for it.

## MS4 Making your work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **How the templates follow the rulebook.** The headings of each issue template follow rule C1 of `docs/team/issueRules.md`, and the Definition of Done at the bottom is section 3, copied in full with a checkbox on every line (rule C10). The merge request template follows section 9.1 of the collaboration agreement for AI use and rule 10 of the code conventions for TODO.

2. **The first issue a teammate made with a template, and what they changed or missed.** Open, see TODO.

3. **What I changed in a template because of it.** Open, see TODO.

## TODO

What is still needed before this proof is complete. Tick a line when it is done.

- [ ] Reviewer of this merge request: open a new merge request on any branch and check that the description starts with the template, including TODO and AI use. Write that in the review.
- [ ] Any teammate: make the first new issue from a template. Then I write in MS4.2 which issue it was and what they changed or missed.
- [ ] Me: write in MS4.3 what I changed in a template because of that issue, or that nothing needed to change.
- [ ] Me: after merge request !13 is merged, add the row for this file to `docs/wesley/README.md` (both merge requests would otherwise change the same lines).

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

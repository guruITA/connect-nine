# Proof: task 1.2 Dev and main protected (#3)

Merge request !2, branch `chore/1-main-only-from-dev`, merged 2 October 2026. Closed test merge requests: !1, branch `test/1-approval-check` into dev, and !3, the same branch `chore/1-main-only-from-dev` into main, both opened and closed on 27 September 2026. Issue: 01 The repository, the pipeline and the team rules (#1), task 1.2 (#3).
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating, MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **What was asked.** Issue #3 asks for a shared branch dev and a protected main, so no change reaches Connect without a merge request that another team member approved. It names the risk directly: after task 1.1 the project had only main, and anyone with access could push to it, which breaks the rule of the collaboration agreement that every change is reviewed.
2. **What was done, and when.** `docs/team/gitlabSetup.md`, part Branches and protection, records the settings made on 27 September 2026: dev made from main and set as the default branch, both dev and main protected with push and merge set to No one, merge set to Maintainers, force push off. One approval is required on every merge request, the author may not approve their own merge request, and people who added commits may not approve it either. Those four settings close the risk before a single feature branch existed.
3. **The risk that main still skips dev.** The settings above protect main from a direct push, but not from a merge request that targets main from a branch other than dev. Merge request !2, merged 2 October 2026, adds the pipeline job branch-flow to `.gitlab-ci.yml` for that gap: it fails a merge request into main when the source branch is not dev.
4. **The test that proved it.** Merge request !3 opened branch `chore/1-main-only-from-dev` into main on 27 September 2026. Pipeline 580111, job branch-flow, failed after printing "Merge request from chore/1-main-only-from-dev into main" and "main only takes merge requests from dev. Merge your branch into dev first." The same branch opened into dev instead, as merge request !2, gave pipeline 580110, job branch-flow green, with only the branch names printed and no refusal. !3 was closed without merging, right after the check.
5. **The self-approval test.** Merge request !1, branch `test/1-approval-check` into dev, was opened and closed six seconds later on 27 September 2026. Its title names what it checked, the approval rule, but it carries no note and no pipeline recording what the test showed.
   - TODO Wesley: write here, or in `docs/team/gitlabSetup.md`, what merge request !1 showed about a self-approval being refused.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Who approved.** Nick Hoebe (hoebenj) approved merge request !2 on 29 September 2026. Wesley Oerlemans merged it on 2 October 2026, after that approval, with a follow-up commit "run branch-flow on the HvA runners (#1)".
2. **What this unlocks for the team.** Issue #3 says that once dev exists and is protected, tasks 1.4, 1.6 and 1.8 can open their own merge requests into dev. Merge request !24, task 1.4's pipeline work (#5), opened into dev on 8 October 2026 and merged the next day, is proof that it did.
3. **Still open.** Issue #3's Acceptance criteria and Reviewer checks are unticked in its description, though its GitLab status was set to Done on 7 October 2026. The issue also asks for a second teammate who tried both refused pushes and a self-approval on their own laptop, with the three refusals pasted as screenshots, and for the first merge request a teammate opened into dev after this task, with what went wrong the first time and what changed in the docs because of it. Neither is written anywhere I can find.
   - TODO Wesley: tick the Acceptance criteria and Reviewer checks of #3, or write why a box stays open.
   - TODO a second teammate: try the refused pushes and a self-approval on your own laptop, and paste the three refusals as screenshots in a comment on #3.
   - TODO Wesley: name the first merge request a teammate opened into dev after this task, what went wrong the first time, and what changed in the docs because of it.

## TODO

- [ ] Wesley: write here, or in `docs/team/gitlabSetup.md`, what merge request !1 showed about a self-approval being refused.
- [ ] Wesley: tick the Acceptance criteria and Reviewer checks of #3, or write why a box stays open.
- [ ] A second teammate: try the refused pushes and a self-approval on your own laptop, and paste the three refusals as screenshots in a comment on #3.
- [ ] Wesley: name the first merge request a teammate opened into dev after this task, what went wrong the first time, and what changed in the docs because of it.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

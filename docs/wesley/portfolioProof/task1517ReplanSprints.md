# Task 1517: sprints 2 to 5 planned again after sprint 1

Relates to #1516, task 180.1 (#1517).

## What changed

Sprint 1 ended on 9 October 2026 with 19 of its issues open, 18 of them must, and none of its 12 slices finished. The plan of 1 October also held 23 should and could issues in sprint 2 and 44 in sprint 3, while 34 musts waited until sprint 3 and 4. On 9 October I planned sprints 2 to 5 again with one set of rules, written down as [decision 004](../../decisions/004-replanAfterSprint1.md): what is done or waits in an open merge request stays, school goals come first, then every must, then should and could, at about 60 weight per sprint of three weeks and 20 in sprint 5. In GitLab 163 issues and 905 open tasks changed sprint, and a second pass the same day moved 29 issues and 134 tasks so the school goals are spread over sprints 2 to 4. docs/project/sprintGoals.md, docs/project/deliveryPlan.md and the milestones of sprints 2 to 5 now tell the same plan.

## Level 3, full-stack

**MS1 Seeing risks early and keeping the work transferable**
- The risk: after a sprint without a finished slice, the board still planned should and could work in sprint 2 and 3 while musts and graded school work waited until sprint 4. A team that follows the board builds the less important work first and runs out of time for what the client and the school need.
- What I made: a plan with fixed rules, so a teammate can run it again. An issue that a stronger issue waits for gets the class of that issue. A school goal never lands after the sprint of its workshop, every research issue lands by sprint 4, and the school goals are spread so no sprint holds all of them. A must is never taken out of the plan, and a should or could that nothing waits for stays open without a sprint.
- Alternatives I did not choose, worked out over all 225 open issues with their weight and links. 80 weight per sprint is a pace the team has not shown. 45 weight per sprint left 13 graded school goals out, among them the DevOps recording and the live demo. 65 weight per sprint leaves no room for a slower sprint. I chose 60 with school goals first, because sprint 1 held the preparation that the later sprints no longer need, and because every student is assessed on the school goals.
- TODO Wesley: after the review of sprint 2 on 5 November 2026, the weight the team finished and what that changed in the plan of sprints 3 to 5.

**MS4 Making your work fit with the work of the team**
- What I made: GitLab, the sprint goals, the delivery plan and the five milestones tell the same plan. Sprint 2 holds the first slice of sprint 1, the security analysis of Kelvin van Voorst and six school goals. Sprints 2, 3 and 4 hold 6, 13 and 12 school goals, so no sprint carries all school work: production and its monitoring in sprint 3, the DevOps demo, the architecture and the research in sprint 4.
- How I checked it: after the change every open issue was read back from GitLab and compared with the plan, with its milestone and its sprint label. A check over all Is blocked by links finds one issue in a sprint before an issue it waits for: issue 106 (#38), which stays in Verify because its research proposals are done, while issues 85 and 10 moved to sprint 2. It finds no should or could in a sprint before the last must unless a must or a school goal waits for it.
- Checked on 10 October 2026 for my own datapoint issues: #187, my custom datapoint of sprint 2, moved back from sprint 4 to sprint 2, because its experiment runs on a small program of its own and does not need the events of #129. That link is removed, and decision 004, the sprint goals and the delivery plan say so. #69 and #72 are back in sprint 2 too, where my personal development plan has them for the DevOps and Testing datapoint. School work follows the school planning and runs ahead of it: the research issues #188 to #190 and the peer review #192 moved from sprint 4 to sprint 2, before the review of the R and D report in sprint 3. A check over every Is blocked by link afterwards finds no issue before an issue it waits for, apart from #38.
- TODO teammates: Gurpreet Singh checks whether the datapoints of #65 and #67 still fit their new sprint, and every team member writes the date in decision 004.

## TODO

- [ ] Wesley: the weight finished in sprint 2 and what it changed in the plan of sprints 3 to 5 (MS1).
- [x] Wesley: check the datapoints of #69, #72 and #187 against their new sprint (MS4, 10 October 2026).
- [ ] Gurpreet Singh: check the datapoints of #65 and #67 against their new sprint (MS4).
- [x] Every team member: decision 004 agreed at the sprint 1 review on 9 October 2026, all five dates in Agreed by.
- [x] After the merge: make the Figma page Issue pyramid again from GitLab (#1498). Done on 9 October 2026: the pages Issue pyramid and Epics and features were drawn again from GitLab after the new plan, 250 cards, no text over the edge of a card.
- [x] After !35 and !37: add the row for this file to docs/wesley/README.md, which those two merge requests also change. Done on 9 October 2026.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve
the formulation of sentences, the structure of wording, readability, and clarity of text that
we had written. The content itself, including our experiences, reflections, learning goals,
decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes
and decided as a team which improvements to include.

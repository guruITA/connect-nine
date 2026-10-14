# 004 Sprints 2 to 5 planned again after sprint 1

**Status.** Proposed on 9 October 2026, waiting for the team. It becomes Accepted when all five names at the bottom have a date.
**Issue.** 180 Sprints 2 to 5 planned again after sprint 1 (#1516), task 180.1 (#1517).
**Who decides.** The whole team. Proposed by Wesley Oerlemans, Scrum Master of sprint 1.

## Context

- Sprint 1 ended on 9 October 2026 with 19 of its issues open: 18 must and 1 should. None of its 12 slices was finished. Issue 03 Create an account (#14) was started, with 3 of its 9 tasks done or in a merge request.
- Sprint 1 did build the preparation that every later slice needs: the repository and the pipeline, the server of the foundation, the architecture and the data model, the clickable prototype of sprint 1 and 2, and the plans. That work does not come back.
- The plan of 1 October put should and could issues early while musts waited. On 9 October, before this decision, GitLab held in sprint 2 37 must, 18 should and 5 could, in sprint 3 10 must, 34 should and 10 could, in sprint 4 24 must, 12 should and 13 could, and in sprint 5 43 could.
- The school goals are what every student is assessed on: the DevOps deliverables, the research reports, and the architecture and security work. The research issues 107 to 112 measure features that are really built, so the events and the showcases must exist before them.

## Options

Each option was worked out over all 225 open issues on 9 October 2026, with their weight and every Is blocked by link in GitLab.

| Option | Weight per three-week sprint, and in sprint 5 | What it gives |
|---|---|---|
| A | 80, and 27 | Every must fits by sprint 4, and 35 should and 10 could follow in sprint 4 and 5. Sprint 1 planned 50 and finished no slice, so 80 is not a pace the team has shown. |
| B | 45, and 15 | 13 school goals do not fit, among them issue 86 the recording, issue 87 the live demo and every research issue, unless the research measures on a small test setup instead of the real feature. |
| C | 60, and 20, school goals first | Every school goal fits in or before the sprint of its workshop and every must fits, with at most 4 weight above the pace in a sprint. 34 should and 69 could stay without a sprint. |
| D | 65, and 22 | Every must fits and part of the should, with no room for a sprint that goes slower. |

## Decision

Option C, with these rules:

1. **What stays.** Every closed issue, every issue whose work waits in an open merge request (01, 09, 11, 178 and 179 in sprint 1, and 16 in sprint 2), issue 106 in Verify, and research that already started (issue 177, #1475).
2. **The order.** School goals first, then every other must, then should, then could. An issue that a stronger issue waits for gets the class of that issue, so a should that a must waits for is planned as a must.
3. **Within a class.** The build order of the board (the rank). A school goal never lands after the sprint its workshop asks for: its own sprint, and every research issue no later than sprint 4, so that issue 112, the peer review of every report, closes the research in sprint 4.
4. **The pace.** About 60 weight per sprint of three weeks and 20 in the one-week sprint 5. A must or school goal that does not fit goes to the sprint with the most room left. It is never taken out of the plan.
5. **Nothing low in front.** No should or could stands in a sprint before the last must, unless a must or a school goal waits for it.
6. **Placeholders.** Should and could issues that nothing waits for stay open without a sprint and keep their priority. The team takes them when it goes faster, or a next team finishes them after the semester. The webshop is among them.
7. **Spread.** School goals are spread over sprints 2 to 4, so no sprint holds all of them. About 25 weight of sprint 3 is kept for school goals before other work fills it, which puts the code quality checks, production and its monitoring in sprint 3 and the DevOps demo, the architecture and the research in sprint 4.
8. **Tasks.** Open tasks move with their issue. Done tasks and tasks in an open merge request stay in sprint 1.

Why C. Sprint 1 held the preparation: the planning, the foundation and the design are done, so the pace goes up from sprint 2. The school goals weigh more than the must haves of the project, because every student is assessed on them. Less important work such as the webshop may stay a placeholder.

Why the others lost. A plans a pace the team has not reached and still puts should work in sprint 4. B leaves graded school work out of the plan, or needs every research issue and its links written again with the teacher's agreement. D fits every must but leaves no room when a sprint goes slower than planned.

## Result

What GitLab holds after the change on 9 October 2026. The weight counts the open issues.

| Sprint | Issues | Must | Should | Could | Open weight |
|---|---|---|---|---|---|
| Sprint 1 | 25 | 23 | 2 | 0 | 12 |
| Sprint 2 | 30 | 25 | 5 | 0 | 63 |
| Sprint 3 | 36 | 29 | 7 | 0 | 60 |
| Sprint 4 | 36 | 15 | 19 | 2 | 63 |
| Sprint 5 | 20 | 20 | 0 | 0 | 23 |
| Without a sprint | 103 | 0 | 34 | 69 | 135 |

163 issues and 905 open tasks changed sprint. A second pass on the same day spread the school goals: 29 issues and 134 open tasks moved between sprints 3, 4 and 5, so sprints 2, 3 and 4 hold 6, 13 and 12 school goals. The should and could issues in sprint 2 to 4 are there because a must or a school goal waits for them, for example the events for research issue 107 and the showcases for research issues 108 and 109. No issue stands in a sprint before an issue it waits for, with one exception: issue 106 (#38) stays in Verify in sprint 1 because the research proposals are done, while issues 85 and 10 that it formally waits for moved to sprint 2.

## Consequences

- **For the client.** Every must he can read on the board is planned. 103 should and could issues are not planned this semester and stay visible as placeholders. The client answered question 1 in issue 10 (#11) at the sprint 1 review on 9 October 2026: he is happy with the order in which the team takes the features in, so the order on the board stands.
- **For the datapoints of the team.** Issues that a student planned for a datapoint moved. Issue 107 (#187) went from sprint 2 to 4, because it waits for the events. Issue 34 (#65) and issue 36 (#69) went from sprint 2 to 3, and issue 34b (#67) and issue 36c (#72) from sprint 2 to 5. Each owner checks whether the datapoint still fits. Checked by Wesley Oerlemans on 10 October 2026: issue 107 (#187) is back in sprint 2, because its experiment runs on a small program of its own with invented data and does not need the events of Connect, so the link to issue 68b (#129) is removed. Issues 36 (#69) and 36c (#72) are back in sprint 2 as well, because his personal development plan has them there for the DevOps and Testing datapoint.
- **School work runs ahead of the school planning.** On 10 October 2026, at the choice of Wesley Oerlemans, the research issues 108 to 110 (#188 to #190) and the peer review of issue 112 (#192) moved from sprint 4 to sprint 2, so every report is written before the review of the R and D report in sprint 3 (complete draft by 22 November, review by 29 November 2026, `docs/school/datapointPlan.md`). Their experiments run on programs of their own with invented data, so the links to the showcase issues 63 (#177), 61b (#180) and 64 (#181) are removed. Issues 68b (#129), 63 (#177) and 64 (#181) stay in sprint 4 although no must or school goal waits for them any more, because the MVP of sprint 4 promises them. That is an exception to rule 6, chosen by Wesley Oerlemans for issue 68b.
- **Sprint 5 is a repair and improvement week.** Decided by Wesley Oerlemans on 10 October 2026: the week of 4 to 8 January 2027 comes before the final evaluation of week 17 and is first for repairing what the sprint 4 review and the teachers ask. School work is finished in sprint 4. The 19 musts stay in sprint 5 and take the time that is left. The knowledge tests got their own issues, 181 (#1521) in sprint 2 and 182 (#1522) in sprint 4.
- **School work on its own boards.** Decided by Wesley Oerlemans on 10 October 2026, after planning every workshop at its first datapoint and undoing it the same evening: the project keeps the plan above. School work that adds nothing to Connect has the label school-only and stands on the School board of its sprint. Overview issues 183 to 188 (#1523 to #1528, one per workshop and the sprint 4 check) and 189 to 195 (#1529 to #1535, one per datapoint) list what the school asks and which issues belong to it, without deadlines of their own and linked with relates to. School research is done locally, outside Connect.
- **Checked again.** After the review of sprint 2 on 5 November 2026 the team measures the weight it finished and plans sprints 3 to 5 again with the same rules.
- **The Figma page Issue pyramid** is made again from GitLab after this change, as group H of `docs/team/issueRules.md` asks (#1498).

## Agreed by

| Name | Date agreed |
|---|---|
| Bayon Eerhart | |
| Nick Hoebe | |
| Wesley Oerlemans | 14 October 2026 |
| Gurpreet Singh | |
| Kelvin van Voorst | |

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

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
7. **Tasks.** Open tasks move with their issue. Done tasks and tasks in an open merge request stay in sprint 1.

Why C. Sprint 1 held the preparation: the planning, the foundation and the design are done, so the pace goes up from sprint 2. The school goals weigh more than the must haves of the project, because every student is assessed on them. Less important work such as the webshop may stay a placeholder.

Why the others lost. A plans a pace the team has not reached and still puts should work in sprint 4. B leaves graded school work out of the plan, or needs every research issue and its links written again with the teacher's agreement. D fits every must but leaves no room when a sprint goes slower than planned.

## Result

What GitLab holds after the change on 9 October 2026. The weight counts the open issues.

| Sprint | Issues | Must | Should | Could | Open weight |
|---|---|---|---|---|---|
| Sprint 1 | 25 | 23 | 2 | 0 | 12 |
| Sprint 2 | 30 | 25 | 5 | 0 | 63 |
| Sprint 3 | 39 | 20 | 18 | 1 | 61 |
| Sprint 4 | 34 | 25 | 8 | 1 | 63 |
| Sprint 5 | 19 | 19 | 0 | 0 | 22 |
| Without a sprint | 103 | 0 | 34 | 69 | 135 |

163 issues and 905 open tasks changed sprint. The should and could issues in sprint 3 and 4 are there because a must or a school goal waits for them, for example the events for research issue 107 and the showcases for research issues 108 and 109. No issue stands in a sprint before an issue it waits for, with one exception: issue 106 (#38) stays in Verify in sprint 1 because the research proposals are done, while issues 85 and 10 that it formally waits for moved to sprint 2.

## Consequences

- **For the client.** Every must he can read on the board is planned. 103 should and could issues are not planned this semester and stay visible as placeholders. The order stays the proposal of the team until the client answers question 1 in issue 10 (#11, now in sprint 2).
- **For the datapoints of the team.** Issues that a student planned for a datapoint moved. Issue 107 (#187) went from sprint 2 to 4, because it waits for the events. Issue 34 (#65) and issue 36 (#69) went from sprint 2 to 3, and issue 34b (#67) and issue 36c (#72) from sprint 2 to 5. Each owner checks whether the datapoint still fits.
- **Checked again.** After the review of sprint 2 on 5 November 2026 the team measures the weight it finished and plans sprints 3 to 5 again with the same rules.
- **The Figma page Issue pyramid** is made again from GitLab after this change, as group H of `docs/team/issueRules.md` asks (#1498).

## Agreed by

| Name | Date agreed |
|---|---|
| Bayon Eerhart | |
| Nick Hoebe | |
| Wesley Oerlemans | |
| Gurpreet Singh | |
| Kelvin van Voorst | |

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

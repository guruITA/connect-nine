# Task 9: work logged afterwards as issues

Relates to #9.

## What changed

On 9 October 2026 the teacher asked the team to put the work it had already done without an issue on the board. Sixteen issues, #1500 to #1515, now stand closed in Sprint 1. Each is assigned to the person who did the work. The work of the whole team (the architecture, the design, the page documents, the brief with the problem analysis and the vision, the requirements, the collaboration agreement and the Definition of Done) is assigned to all five students. Rule A8 in docs/team/issueRules.md and a line in docs/team/gitlabSetup.md say how this is done from now on, so the next issue logged afterwards looks the same.

The same merge request makes the documents name the lists of the sprint boards as they are in GitLab: Open, In progress, Verify, Won't do and Closed. The three lists in the middle follow the status of an issue, and the old labels Doing and Verify are no longer used for them. This is in docs/team/gitlabSetup.md, docs/project/deliveryPlan.md, docs/school/issueMapping.md and docs/team/onboarding.md.

| Who | Issues |
| --- | --- |
| The whole team | #1500 architecture and tech stack, #1501 design and clickable prototype, #1502 page documents, #1503 brief, problem analysis and product vision, #1504 requirements, #1506 collaboration agreement, #1507 Definition of Done |
| Wesley Oerlemans | #1505 delivery plan and sprint goals, #1508 testing strategy, #1509 the board and the import into GitLab, #1510 school documents, #1511 risks and open items |
| Nick Hoebe | #1512 configuration and the example settings file, #1513 the pipeline step that refuses a secrets file, #1514 the server arranged through the client |
| Gurpreet Singh | #1515 the contact with the client arranged in sprint 1 |

## Level 3, full-stack

**MS1 Seeing risks early and keeping the work transferable**
- What I made: the risk was that work outside the board stays invisible, both to a teacher who assesses per person and to a teammate who takes over and looks for the documents. Every new issue links to the files or the merge request where the work is, so the board now leads to all of it.
- Alternative I did not choose: one issue "Sprint 1 documentation" with everything in it. It is less work, but it hides who did what, and the teacher assesses each student on their own work.

**MS4 Making your work fit with the work of the team**
- What I made: the assignments follow who did the work. For the documents that is the Git history per file. For work outside Git it is what the team knows: Gurpreet Singh arranged the contact with the client and Nick Hoebe arranged the server. Work that needed the whole team is on all five names, also where one person wrote it out.
- TODO teammates: each one checks their own issues among #1500 to #1515 and says in this merge request if one is wrong or if work of theirs is missing.

## TODO

- [ ] Bayon Eerhart, Gurpreet Singh, Nick Hoebe and Kelvin van Voorst: check your own issues among #1500 to #1515 and say in this merge request if one is wrong or if work of yours is missing (MS4).
- [ ] Whoever merges this merge request after !35 or !37: both add a row to docs/wesley/README.md, so keep every row.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve
the formulation of sentences, the structure of wording, readability, and clarity of text that
we had written. The content itself, including our experiences, reflections, learning goals,
decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes
and decided as a team which improvements to include.

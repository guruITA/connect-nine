# Task 1499: the team documents follow the new Figma board

Relates to #1498, task 179.1 (#1499).

## What changed

On 9 October 2026 the Figma board went from 82 to 13 pages. The 21 pages with full issue cards, the 20 archive pages, the page Issue creation and the copies of the sprint 1 screens are gone. Two new pages show every issue as a picture made from GitLab: Issue pyramid (a column per epic, a band per sprint, lines for what waits for what) and Epics and features (every epic with its issues in the colour of their priority). Group H of docs/team/issueRules.md now describes these two pages, and the rulebook, the Definition of done, the code conventions, the GitLab setup and the document guide no longer send the reader to a page that is gone.

## Level 3, full-stack

**MS1 Seeing risks early and keeping the work transferable**
- The risk: after the compact form of 8 October the issue cards on the Figma board were longer and older than the issues in GitLab, and four team documents named the page Issue creation as their source. A teammate who followed a document could build from an old card or look for a page that was about to go.
- What I made: the rulebook and the team documents now describe the board that exists. Group H says that GitLab is the source of every issue, how the two pages are read, and that they are made again from GitLab when issues, links, sprints or priorities change, never by hand.
- Alternative I did not choose: keeping the old pages as archive pages in the same file. A reader then has to know which of 82 pages counts, and Figma keeps every earlier version of the file in its version history anyway.
- TODO Wesley: how the team used the page Issue pyramid in the planning of sprint 2, and what changed after that.

**MS4 Making your work fit with the work of the team**
- What I made: the rulebook, GitLab and the two pages describe the same 232 issues, the same 645 Is blocked by links and the same priorities (95 must, 66 should, 71 could). The GitLab texts that named a removed page (#3, #9 and #1467 to #1472) now point to the page Issue pyramid or to docs/team/issueRules.md.
- How I checked it: a search of the whole repository for Issue creation, feature page, section 3b and section 8 of the page Issue creation finds nothing that points to the board any more, and the same search over all 1493 issues and tasks in GitLab finds nothing either. The page descriptions in docs/pages point to the page Design directions, which stays.
- TODO teammates: each team member opens the pages Issue pyramid and Epics and features and confirms in #1498 that the issues they own stand in the right epic and sprint.

## TODO

- [ ] Wesley: how the team used the page Issue pyramid in the planning of sprint 2 (MS1).
- [ ] Every team member: confirm in #1498 that the issues they own stand in the right epic and sprint on both pages (MS4).
- [ ] Reviewer: read group H next to the page Issue pyramid and check that every rule matches what the page shows.
- [ ] After !35: add the rows for board numbers 175, 176 and 179 to docs/team/boardNumbers.md.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve
the formulation of sentences, the structure of wording, readability, and clarity of text that
we had written. The content itself, including our experiences, reflections, learning goals,
decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes
and decided as a team which improvements to include.

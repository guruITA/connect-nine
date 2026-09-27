# Team 9, Connect Nine, Collaboration Agreement

| | |
| --- | --- |
| Team number | 9 |
| Team name | Connect Nine |
| Product | Young Amstelland Connect |
| Program | HBO-ICT Full Time |
| Organization | Amsterdam University of Applied Sciences |
| Studio | Social Impact Apps, semester 1, 2026-2027 |
| Client | Rachid Kherrazi, WailSalutem Foundation |
| Team coach | Karthik Srinivasan |
| Version | 1.11, 23 September 2026 |
| Semester start | 31 August 2026 |
| Sprint 1 start | 21 September 2026 |
| Project completion | week of 4 January 2027 |
| Classification | Open. The client wants the project visible, to motivate young people and to show the foundation works with the HvA |

## 1. Introduction

This collaboration agreement defines the working arrangements for the Connect Nine project. It
sets expectations for communication, documentation, responsibilities, and team conduct so the
collaboration stays clear and productive.

The product is called Young Amstelland Connect. It is the project assignment for the Social
Impact Apps studio, and the client is the WailSalutem Foundation.

All team members are responsible for following the rules in this agreement and for addressing
team members who do not follow them.

## 2. Team members and contact details

Email addresses and phone numbers stay outside Git, in the contact list the team keeps on Teams.

_Students_

| Name | Profile | Level |
| --- | --- | --- |
| Bayon Eerhart | Full Stack Software Developer | 2 |
| Nick Hoebe | Full Stack Software Developer | 3 |
| Wesley Oerlemans | Full Stack Software Developer | 3 |
| Gurpreet Singh | Full Stack Software Developer | 3 |
| Kelvin van Voorst | Security Specialist | 2 |

_Coach_

- Karthik Srinivasan

_Client_

- Rachid Kherrazi, WailSalutem Foundation
- www.wailsalutem-foundation.com
- Zanderij 29, 1185 ZM Amstelveen

## 3. Communication

Microsoft Teams is the primary channel and carries everything that needs to be findable later:
decisions, documents, and anything the coach or the client may need to read.

The WhatsApp group **Team 9 | Connect Nine** is for short and urgent messages, and for anything
outside contact hours.

A message is answered within 24 hours. There is no fixed window inside that, and nobody is
expected to be reachable at a particular moment.

## 4. Working hours, attendance and the semester calendar

Each team member spends **24 hours a week** on the project, across **three project days**.

On campus, the project contact hours are **Thursday 9:00 to 12:00** and **Friday 12:00 to
14:30**. All other contact is remote. Attendance at all scheduled parts of the day is a
condition of passing the semester, and the studio asks for attendance close to 95 percent.

If a team member cannot attend, they inform the team in advance. If that is not possible, they
say so as soon as possible afterwards. An absent team member catches up on what was missed
themselves.

### Dates that affect the plan

| Date | What |
| --- | --- |
| 21 Sep, 28 Sep, 5 Oct | sprint 1 |
| week of 28 Sep | no lecturer guidance |
| week of 5 Oct | progress evaluation 1 |
| 12 to 17 Oct | autumn break |
| 19 Oct, 26 Oct, 2 Nov | sprint 2 |
| 26 Oct | knowledge test 1 |
| 9 Nov, 16 Nov, 23 Nov | sprint 3 |
| 8 to 13 Nov | Bayon Eerhart is at the Stuttgart BIP |
| week of 16 Nov | progress evaluation 2 |
| week of 30 Nov | no lecturer guidance |
| 7 Dec, 14 Dec | sprint 4 |
| week of 14 Dec, day to be confirmed | knowledge test 2 |
| 21 Dec to 2 Jan | Christmas break |
| week of 4 Jan | project completion and progress evaluation 3 |

Each sprint ends with a sprint review at the client and a retrospective.

## 5. Project roles

Scrum Master and Product Owner rotate every sprint. Both are roles and neither is a full-time
job.

The Scrum Master organizes the daily standup, maintains the impediment log and removes
impediments. The Product Owner maintains communication with the client and other stakeholders,
and maintains quality within the project.

| Sprint | Scrum Master | Product Owner |
| --- | --- | --- |
| 0 | Nick Hoebe | Gurpreet Singh |
| 1 | Wesley Oerlemans | Nick Hoebe |
| 2 | Gurpreet Singh | Bayon Eerhart |
| 3 | Kelvin van Voorst | Wesley Oerlemans |
| 4 | Bayon Eerhart | Kelvin van Voorst |

Every team member holds each role exactly once across the five sprints.

**The Stuttgart BIP needs no cover.** Bayon Eerhart is abroad from 8 to 13 November, the first
week of sprint 3, and holds no role in that sprint: he is Product Owner in sprint 2 and Scrum
Master in sprint 4, both of which he attends in full. He catches up on the missed week himself,
per section 4.

### Standup

When the team is physically together, the standup is held in person.

When a team member works from home, the minimum is a written message covering three things:
what they did, what they are going to do, and any blockers.

If this proves not to work, the team tightens it into a fixed schedule with a plan per week for
when the standup happens and how long it takes.

### Retrospective

The retrospective is held on the last physical lesson day of the sprint.

## 6. Documentation

### Commit Messages

Use one of the following labels for each commit:

1. `#issue number feat:` Add a new feature.
2. `#issue number fix:` Fix bugs or resolve defects.
3. `#issue number refactor:` Change existing code without changing behavior.
4. `#issue number remove:` Remove code.
5. `#issue number chore:` Make small maintenance changes.
6. `#issue number docs:` Add or update documentation.
7. `#issue number test:` Add or update tests.

### Code Conventions

The stack is React for the frontend, .NET for the backend, and Microsoft SQL Server for the
database, a team decision relayed by Wesley Oerlemans on 2026-09-22 and recorded with rationale
in `techStack.md`. The conventions are the established best practices of that stack, together
with SOLID, DRY as far as is practical, and clean code as far as is practical.

The stack-specific conventions land in `codeConventions.md`, whose stack-specific section the
team fills at sprint 1 planning.

### Branches and Merge Requests

1. `main` is protected. No direct commits and no pushes. It changes only through a merge
   request.
2. `dev` is the only branch that merges into `main`.
3. Every work branch merges into `dev`: feature, fix, hotfix, refactor, remove and chore alike.
4. Branch names follow the format `label/issue-number`, with a prefix for every commit label:
   `feature/`, `fix/`, `refactor/`, `remove/`, `chore/`.
5. Hotfix branches are `hotfix/` and are exempt from the issue number rules: no issue number in
   the branch name, the commit label or the merge request name. An issue is created after the
   fact, so the paper trail catches up.
6. Merge request names must include an issue number, hotfixes excepted per rule 5.
7. At least one reviewer must be a non-owner.

There is deliberately no hotfix path straight into `main`. A bypass would weaken the protection
that rule 1 exists to provide.

Repository: to be added when the team repository exists.

## 7. Quality assurance

### Definition of Done

Work is done when all of the following are true:

1. The code follows the team's code conventions.
2. The linter passes.
3. There are no compile errors.
4. Any remaining warnings are explained, in the documentation, in a doc comment or in a summary.
5. Unit tests are written and pass.
6. The documentation is updated, for both the code and the architecture.
7. Significant choices are documented.

### Documentation location

Project documentation lives in the `docs` folder in the repository.

## 8. Responsibilities

Each team member is expected to complete assigned work on time, communicate blockers early, and
contribute to shared project goals. If someone cannot meet a deadline, they should inform the
team as soon as possible so the work can be redistributed.

Team members often take on tasks that fit their profile, and also work outside it.

Minor problems are solved within the team first. Larger problems are discussed with the team,
and if they cannot be resolved quickly the person in question raises them with the team coach.

## 9. AI use

Using AI is allowed. Not mentioning it is not, and undisclosed AI-generated content counts as
fraud, which a lecturer is obliged to report to the Board of Examiners.

### 9.1 Declaration in written work

Each team member includes a declaration in the written work they hand in. The wording used in
this team is:

**AI was used as a writing support tool while working on this document. We used AI to improve
the formulation of sentences, the structure of wording, readability, and clarity of text that
we had written. The content itself, including our experiences, reflections, learning goals,
decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes
and decided as a team which improvements to include.**

### 9.2 Limitations set by the client

The client sets no limitations on AI use, confirmed in September 2026. The team's own rules
in this section and the school's rules in the study manual apply unchanged.

### 9.3 AI in project code

Code produced with AI follows the repository style and the same conventions as code written by
hand. The team member who commits it has read what the AI produced at least once.

The AI models used in the project are named here, in this section, so one place answers which
tools the team works with. The team as a whole uses the providers listed below, and the exact
model versions are pinned when a hand-in disclosure is written.

Models in use:

- Anthropic, Claude models
- OpenAI, GPT models

## 10. Consequences

A strike is issued by a majority vote of the team, when a team member does not follow this
collaboration agreement.

1. First strike: internal reminder and discussion within the student team.
2. Second strike: a formal internal warning and follow-up with the coach.
3. Third strike: escalation to the teacher, with the issue documented and discussed further.
   The team member leaves the team.

After each strike, the team will agree on a clear improvement point and a deadline for review.
If the behavior improves, no further action is needed.

Strikes do not reset and do not expire. They stand for the semester.

## 11. Signed by

This collaboration agreement was signed by:

| Name | Signed | Date |
| --- | --- | --- |
| Bayon Eerhart | | |
| Nick Hoebe | | |
| Wesley Oerlemans | | |
| Gurpreet Singh | | |
| Kelvin van Voorst | | |

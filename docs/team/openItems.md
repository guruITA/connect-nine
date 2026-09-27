# Open items register, Young Amstelland Connect

This document serves the HvA level 3 criterion Management & Collaboration (MS).

This is the team's single register of what is still open across the delivered document set,
so the team knows what needs deciding, what needs asking, and what needs planning without
checking every document separately. It gathers items already recorded as owned, dated TBDs
across the team's documents. It raises nothing new beyond what those documents already carry,
except where noted.

## Needs deciding

Decisions the team, or the team with the client, still has to make. Unless stated otherwise,
the forum is sprint 1 planning, the week of September 21, 2026.

| Decision | Owner | Forum and date | Source |
| --- | --- | --- | --- |
| Confirm the three open collaboration agreement notes and sign the agreement: the commit label order, the section 9.1 declaration wording, the section 9.3 AI provider list and model version pinning, and the five signatures | Whole team | The team moment, Thursday September 24, 2026 | The collaboration agreement working copy and the delivery plan |
| Hosting option: a single VPS, Azure for Students credit, a hybrid split, or the nonprofit Azure grant through the client's own foundation | Whole team, with the client | Sprint 1 planning, week of September 21, 2026 | The infrastructure document |
| Database provider: Microsoft SQL Server, the team's coursework stack, or PostgreSQL, which eases Docker hosting on the chosen budget | Whole team, with the client | Sprint 1 planning, week of September 21, 2026 | The tech stack document and the infrastructure document |
| Data access pattern: the proposed Repository pattern, or Command Query Responsibility Segregation as a real alternative, given Nick Hoebe's prior CQRS experience | Whole team | Sprint 1 planning, week of September 21, 2026 | The architecture document |
| Confirm or redirect the proposed layered architecture with dependency inversion on the persistence layer, the component diagram, and the third design pattern if Strategy is not the team's pick | Whole team | Sprint 1 planning, week of September 21, 2026 | The architecture document |
| Linter and formatter picks for each layer (ESLint with Prettier or Biome for the frontend, the built-in .NET analyzers or StyleCop or Roslynator for the backend, SQLFluff for the database), the naming conventions and project layout still open, and whether pre-commit hooks are worth the setup time | Whole team | Sprint 1 planning, week of September 21, 2026 | The code conventions document |
| Testing coverage ambition, the module-by-module test plan, React Testing Library or an alternative for frontend integration tests, and how the end-to-end suite fits the pipeline's timing | Whole team | Sprint 1 planning, week of September 21, 2026 | The testing strategy document |
| Container orchestration: confirm or decline the optional study-points path, and if confirmed, whether the team commits to Docker Compose only or Compose plus single-node k3s | Whole team | Sprint 1 planning, week of September 21, 2026 | The infrastructure document |
| Sprint 1 backlog and the proposed MoSCoW prioritization of the functional requirements | Whole team, with the client | Sprint 1 planning, week of September 21, 2026 | The requirements document |
| Numeric performance and availability targets | Whole team, with the client | Sprint 1 planning, week of September 21, 2026 | The requirements document and the product vision document |
| Success measures for Connect, for the client and for the team | Whole team, with the client | Sprint 1 planning, week of September 21, 2026 | The product vision document and the problem analysis document |
| The pipeline's dependency scan gate severity threshold, and the exact rollback procedure once a hosting option is chosen | Whole team | Sprint 1 planning, week of September 21, 2026 | The infrastructure document |
| Operational needs: the backup retention period, the secrets handling approach, the domain name choice, and the EU data location requirement, the last of which also waits on the client's real-versus-test-data answer | Whole team | Sprint 1 planning, week of September 21, 2026 | The infrastructure document |
| Clarify one unreadable entry in the skills inventory, recorded as "lid low" | Bayon Eerhart | Sprint 1 planning, week of September 21, 2026 | The tech stack document |

## Needs asking

### The client

Six questions are owed by the client, Rachid Kherrazi at the WailSalutem Foundation. The
client questions document is the source of truth for the full wording and reasoning behind
each one. Owner for all six: Nick Hoebe, as the team's client contact. Due by the sprint 1
review, week of October 5, 2026.

| # | Question, in short |
| --- | --- |
| 1 | Who owns the code and the intellectual property once the project ends |
| 2 | Whether development, and later production, uses real personal data or test data only |
| 3 | Whether the team may show the work in a portfolio, in Portflow, and in an R&D report |
| 4 | Whether the webshop module is in scope for this semester, and if so, its priority |
| 5 | What happens to the platform's hosting and the platform itself after the project ends |
| 6 | Whether the WailSalutem Foundation is a formally registered nonprofit in the Netherlands |

### The coach

Four questions are owed by team coach Karthik Srinivasan. Owner for all four: Wesley
Oerlemans. Ask by Thursday September 24, 2026.

- The exact Portflow due dates for the research proposal and the R&D report, neither of which
  appears in the course material the team holds.
- What the sprint 3 and sprint 4 Custom Datapoints should each review, since the team's
  elective work plans one sprint's worth of evidence while the level 3 sheet names three
  Custom Datapoints across the semester.
- The exact days for knowledge test 1, in the week of October 26, 2026, and knowledge test 2,
  in the week of December 14, 2026. Both weeks are confirmed. Neither day is.
- The date of the Decision Committee and the Portflow portfolio submission, which carries no
  due date in the course's own assignment list.

## Needs planning

Dated items the team has to schedule.

- **GitLab repository creation.** The team has no repository yet. Owner: whole team. Date:
  sprint 1 planning, week of September 21, 2026, the first step toward a working code slice
  before the sprint 1 review. The seven labels from the issue mapping document are created in
  the new repository before the first issues are made, so every issue carries its outcome,
  level, and direction label from the start rather than being retagged later.
- **Kelvin van Voorst's Security requirements and risk framing datapoint.** Identifying the
  security requirements, key assets, and main risks for the project, feeding the
  authentication and authorization requirement now. Owner: Kelvin van Voorst. Running now,
  weeks of September 21 and 28, 2026.
- **Wesley Oerlemans's research draft to the learning group teacher.** Owner: Wesley
  Oerlemans. Date: by October 5, 2026.
- **Sprint 1 review minimum set.** The collaboration agreement signed, the sprint 1 planning
  decisions above confirmed rather than proposed, a GitLab repository with a project skeleton
  and a small working slice of code, the sprint board showing the backlog split into sprint 1
  work, and each team member's own evidence collection gathered toward their individual
  datapoints. Owner: whole team. Date: Friday October 9, 2026.

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to draft
the formulation of sentences, the structure of wording, readability, and clarity from
content that we had provided. The content itself, including our experiences, reflections,
learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the
drafts and decided as a team which text to include.

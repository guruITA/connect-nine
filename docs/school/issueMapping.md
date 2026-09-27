# Issue to learning outcome mapping, Young Amstelland Connect

This document serves the HvA level 3 criteria Analysis & Advice (AA), Design & Realisation
(OR), and Management & Collaboration (MS), depending on which label an issue carries. It is
grounded in BoKSA's own studio roadmap and its Full-Stack Developer and Security Specialist
profile pages, and in `requirements.md` and `workshopCoverage.md`, so the labels below match how
this team already tracks datapoints and owns modules.

## Why issues carry learning outcome labels

Portfolio evidence in Portflow is harvested from real project work, not written from memory
months later. `workshopCoverage.md` already builds the first half of that chain: a sprint 0
workshop feeds a named datapoint, and the datapoint's content column quotes a BoKSA article
title. This document extends the same chain one step further, from the datapoint down to the
individual GitLab issue that produced the evidence. An issue tagged with its outcome and level
at creation is findable at datapoint time by filtering the board, instead of the team having to
reconstruct, at the sprint review, which merge request actually proved which outcome.

## Label scheme

GitLab supports scoped labels, a key and a value joined by a double colon, where only one value
per key applies to a given issue at once. That fits a five-person team well: three small label
groups, combined per issue, rather than one label per outcome-level-direction combination.

**Outcome**, using BoKSA's own outcome names, one label per name that this project's issues
actually evidence:

- `outcome::analysing-and-advising`
- `outcome::designing-and-realising`
- `outcome::managing-and-collaborating`

Personal Leadership is BoKSA's fourth outcome, but it is individual reflection on one's own
development, not project work a GitLab issue produces, so it carries no label here and stays
out of this scheme.

**Level**, one label per HvA level this team actually works at:

- `level::2`
- `level::3`

**Direction**, added only where BoKSA's own wording or knowledge base actually differs by
study direction, which is Kelvin van Voorst's track against the other four members':

- `direction::full-stack`
- `direction::security-specialist`

Seven labels total. A Full-Stack Developer Level 3 piece of design work (Nick Hoebe, Wesley
Oerlemans, or Gurpreet Singh) carries `outcome::designing-and-realising` and `level::3`, with
no direction label needed since full stack is the default reading of an unmarked issue. Bayon
Eerhart's own work at Full-Stack Level 2 carries the same outcome label with `level::2`. Kelvin
van Voorst's Security Specialist work carries `direction::security-specialist` alongside the
matching outcome and `level::2`, since BoKSA gives his profile its own Level 2 wording and its
own CYBOK knowledge layer under those same four outcome names.

## Mapping table

Issue kinds this project will actually generate, drawn from `requirements.md`'s modules and
non-functional requirements and from the decided stack, mapped to the outcome and level each
serves and the BoKSA article or CYBOK topic it leans on.

| Issue kind | Labels | BoKSA article or topic |
| --- | --- | --- |
| Client questions and requirements clarification (`clientQuestions.md`, the open TBDs in `requirements.md`) | `outcome::analysing-and-advising`, `level::3` | Technical Requirements Analysis |
| MoSCoW prioritization and non-functional requirement targets (FR1 to FR10, NFR3 to NFR6) | `outcome::analysing-and-advising`, `level::3` | Non-Functional Requirements & Quality Attribute Analysis |
| Stack, hosting, and third-party option research (`infrastructure.md`'s hosting options, `stackResearch.md`) | `outcome::analysing-and-advising`, `level::3` | Third-Party Options Analysis |
| Architecture style and design pattern decisions (the layered shape, Repository, dependency injection, Strategy for FR2) | `outcome::designing-and-realising`, `level::3` | Architecture and Design Patterns |
| Authorization design and implementation (NFR1) | Kelvin: `outcome::designing-and-realising`, `level::2`, `direction::security-specialist`. Full stack realization of the same design: `outcome::designing-and-realising`, `level::3` | CYBOK 14, Authentication, Authorisation & Accountability (AAA), for Kelvin. Authentication & Authorization Basics, for the full-stack realization |
| Database schema and EF Core integration (the Infrastructure layer, repository implementations against Microsoft SQL Server) | `outcome::designing-and-realising`, `level::3` (`level::2` for Bayon Eerhart's own work) | Database Integration |
| Frontend component and Minimal API endpoint implementation (a functional requirement's UI or endpoint) | `outcome::designing-and-realising`, `level::3` (`level::2` for Bayon Eerhart's own work) | Component Architecture & Framework Basics |
| Testing setup and module test coverage (NUnit, FluentValidation, Vitest, and Playwright work per `testingStrategy.md`) | `outcome::designing-and-realising`, `level::3` | Advanced Testing: Strategy & Professional Test Environment |
| CI/CD pipeline, branching, and merge request discipline (`infrastructure.md`'s GitLab CI proposal, `codeConventions.md`'s branch flow) | `outcome::managing-and-collaborating`, `level::3` (`level::2` for Bayon Eerhart's own work) | CI/CD: Complete Pipeline Design & Implementation, alongside Version Control & Git Workflow |
| Security requirements and risk framing, threat modeling (Kelvin van Voorst's own datapoints) | `outcome::analysing-and-advising`, `level::2`, `direction::security-specialist` | CYBOK 02, Risk Management & Governance |
| Data protection and regulatory compliance review (NFR2, Kelvin's own regulatory reading of it) | `outcome::analysing-and-advising`, `level::2`, `direction::security-specialist` | CYBOK 05, Privacy & Online Rights |

Eleven issue kinds. Authorization is the clearest example of a single piece of work
demonstrating two different things: Kelvin's security design and the full-stack team's own
realization of it each get their own label combination, on their own issues, rather than one
issue trying to carry both.

## The level rule

BoKSA gives Level 2 and Level 3 the same four outcome names and the same article list. What
changes is the wording under each outcome's own "Level 2" and "Level 3" heading, and the shift
is consistent across all four: Level 2 asks the student to apply a method competently inside
one clear frame, questioning found sources and taking a plan someone else largely set, while
Level 3 asks the student to choose and justify the approach itself, weigh more than one
perspective or stakeholder, and own the consequences of that choice, including proactively
supporting others in theirs. An issue that stays inside one given method, one stakeholder view,
and one solution check is Level 2 shaped, however well it is executed, and it does not on its
own demonstrate Level 3. So the team tags each issue at the level the work itself actually
reaches, not the level the person doing it is assessed at overall, and does not default
everything to Level 3 because that is the highest bar in the room.

## The habit

1. **Tag at creation.** Every new GitLab issue gets its outcome, level, and direction labels
   (if any) applied when it is opened, not retrofitted later.
2. **Name the outcome in the merge request.** When the work closes, the merge request
   description states which outcome and level the change evidences, for example "outcome:
   Designing & Realising, level 3", so the link between code and outcome survives past the
   issue itself.
3. **Harvest links at datapoint time.** At each sprint review, filter the board by the
   datapoint's own outcome label and pull the resulting issue and merge request links straight
   into Portflow, the same join `workshopCoverage.md` already builds from workshop to datapoint
   to article.

This scheme is a proposal for the whole team to confirm before it is applied to the backlog.

## References

HBO-ICT. (n.d.-a). *Advanced testing: Strategy & professional test environment*. BoKSA.
Retrieved September 24, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/design-and-realise/advanced-testing-strategy-and-test-environment/

HBO-ICT. (n.d.-b). *Analysing & advising*. BoKSA. Retrieved September 24, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/analyse-and-advice/

HBO-ICT. (n.d.-c). *Analysing & advising*. BoKSA. Retrieved September 24, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/security-specialist/analyse-and-advice/

HBO-ICT. (n.d.-d). *Architecture and design patterns*. BoKSA. Retrieved September 24, 2026,
from https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/design-and-realise/architecture-and-design-patterns/

HBO-ICT. (n.d.-e). *Authentication & authorization basics*. BoKSA. Retrieved September 22,
2026, from https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/design-and-realise/authentication-and-authorization-basics/

HBO-ICT. (n.d.-f). *CI/CD: Complete pipeline design & implementation*. BoKSA. Retrieved
September 24, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/manage-and-collaborating/ci-cd-complete-pipeline-design/

HBO-ICT. (n.d.-g). *Component architecture & framework basics*. BoKSA. Retrieved September 22,
2026, from https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/design-and-realise/component-architecture-and-frameworks/

HBO-ICT. (n.d.-h). *CYBOK 02 risk management & governance*. BoKSA. Retrieved September 25,
2026, from
https://boksa.hbo-ict-hva.nl/profiles/security-specialist/analyse-and-advice/mdfiles/CYBOK%2002%20Risk%20Management%20&%20Governance/

HBO-ICT. (n.d.-i). *CYBOK 05 privacy & online rights*. BoKSA. Retrieved September 24, 2026,
from
https://boksa.hbo-ict-hva.nl/profiles/security-specialist/analyse-and-advice/mdfiles/CYBOK%2005%20Privacy%20&%20Online%20Rights/

HBO-ICT. (n.d.-j). *CYBOK 14 authentication, authorisation & accountability (AAA)*. BoKSA.
Retrieved September 24, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/security-specialist/analyse-and-advice/mdfiles/CYBOK%2014%20Authentication,%20Authorisation%20&%20Accountability/

HBO-ICT. (n.d.-k). *Database integration*. BoKSA. Retrieved September 22, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/design-and-realise/database-integration/

HBO-ICT. (n.d.-l). *Designing & realising*. BoKSA. Retrieved September 24, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/design-and-realise/

HBO-ICT. (n.d.-m). *Designing & realising*. BoKSA. Retrieved September 24, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/security-specialist/design-and-realise/

HBO-ICT. (n.d.-n). *Foundation*. BoKSA. Retrieved September 24, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/security-specialist/Foundation/

HBO-ICT. (n.d.-o). *Managing & collaborating*. BoKSA. Retrieved September 24, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/manage-and-collaborating/

HBO-ICT. (n.d.-p). *Managing & collaborating*. BoKSA. Retrieved September 24, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/security-specialist/manage-and-collaborating/

HBO-ICT. (n.d.-q). *Non-functional requirements & quality attribute analysis*. BoKSA. Retrieved
September 24, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/analyse-and-advice/non-functional-requirements-and-quality-attributes/

HBO-ICT. (n.d.-r). *Technical requirements analysis*. BoKSA. Retrieved September 24, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/analyse-and-advice/technical-requirements-analysis/

HBO-ICT. (n.d.-s). *Third-party options analysis*. BoKSA. Retrieved September 24, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/analyse-and-advice/third-party-options-analysis/

HBO-ICT. (n.d.-t). *Version control & Git workflow*. BoKSA. Retrieved September 24, 2026, from
https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/manage-and-collaborating/version-control-with-git/

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to draft
the formulation of sentences, the structure of wording, readability, and clarity from
content that we had provided. The content itself, including our experiences, reflections,
learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the
drafts and decided as a team which text to include.

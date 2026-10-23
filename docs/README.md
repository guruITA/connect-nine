# Documentation of Connect

Connect is the online platform of the WailSalutem Foundation. On it, young people from Amstelland, the organisations around them and the foundation find each other all year. Team 9 (Connect Nine) builds it for the client Rachid Kherrazi, as a project of the HvA studio Social Impact Apps.

This page is the way in. Start with the part that fits your question.

## New in the team

1. Read [team/collaborationAgreement.md](team/collaborationAgreement.md): how the team works together.
2. Read [team/codeConventions.md](team/codeConventions.md): branches, commits, merge requests and the rules per layer.
3. Read [team/definitionOfDone.md](team/definitionOfDone.md): when an issue is done, with How to check per line.
4. Open the board of your sprint through the milestone of that sprint in GitLab (Plan, Milestones).

## The folders

| Folder | What is in it | Documents |
|---|---|---|
| [project](project/) | Why Connect exists and what it must do | [productVision.md](project/productVision.md), [problemAnalysis.md](project/problemAnalysis.md), [requirements.md](project/requirements.md), [deliveryPlan.md](project/deliveryPlan.md), [sprintGoals.md](project/sprintGoals.md) |
| [client](client/) | The client, his brief and every open question for him | [clientBrief.md](client/clientBrief.md), [clientBriefConcept.jpeg](client/clientBriefConcept.jpeg), [clientQuestions.md](client/clientQuestions.md), [serverHandover.md](client/serverHandover.md) (how the foundation gets into its server after the handover) |
| [architecture](architecture/) | How Connect is built and where it runs | [architecture.md](architecture/architecture.md), [techStack.md](architecture/techStack.md), [stackResearch.md](architecture/stackResearch.md), [infrastructure.md](architecture/infrastructure.md), [configuration.md](architecture/configuration.md), [api.md](architecture/api.md) |
| [team](team/) | How the team works and the rules for every change | [collaborationAgreement.md](team/collaborationAgreement.md), [codeConventions.md](team/codeConventions.md), [definitionOfDone.md](team/definitionOfDone.md), [gitlabSetup.md](team/gitlabSetup.md), [issueRules.md](team/issueRules.md), [boardNumbers.md](team/boardNumbers.md), [openItems.md](team/openItems.md), [actionOverview.md](team/actionOverview.md), [onboarding.md](team/onboarding.md) |
| [decisions](decisions/) | One file per choice, `<number>-<shortName>.md`, with the date, who decided, the options and why | [001-codeConventions.md](decisions/001-codeConventions.md) |
| [future-improvements](future-improvements/) | Improvements that could be made in the future | [futureImprovements.md](future-improvements/README.md) |
| [testing](testing/) | How Connect is tested | [testingStrategy.md](testing/testingStrategy.md) |
| [pages](pages/) | Every page of the website, one document per page with every element | [pages/README.md](pages/README.md), the overview with a link to each of the 66 page documents |
| [school](school/) | What the HvA asks and how the issues prove it | [workshopCoverage.md](school/workshopCoverage.md), [issueMapping.md](school/issueMapping.md), [documentGuide.md](school/documentGuide.md), [datapointPlan.md](school/datapointPlan.md) |
| [sprintReviews](sprintReviews/) | The deck of each sprint review, the review form the client filled in, the texts the team puts in Portflow, and the template for the next reviews | [sprint1ReviewDeck.pptx](sprintReviews/sprint1ReviewDeck.pptx), [sprint1ReviewForm.pdf](sprintReviews/sprint1ReviewForm.pdf), [sprint1PortflowGuide.md](sprintReviews/sprint1PortflowGuide.md), [portflowSprintReviewTemplate.md](sprintReviews/portflowSprintReviewTemplate.md) |
| [presentations](presentations/) | The decks the team shows the client between the sprint reviews, with speaker notes | [sprint2Week1ClientUpdate.html](presentations/sprint2Week1ClientUpdate.html) |

[school/documentGuide.md](school/documentGuide.md) describes every document with what it is for and which school criterion it serves.

## Made later by the issues

These folders and files do not exist yet. The issue that makes them says so on its card.

| Place | What |
|---|---|
| docs/security | The security analysis of Kelvin: threat models, the role table, GDPR notes, the privacy statement |
| docs/research | The research reports of the experiments (issues 106 to 112) |
| docs/school | Also the school evidence the workshop issues ask for |
| docs/architecture | Also designPatterns.md, socialLogin.md and concurrencyRegistration.md |
| docs/testing | Also testData.md (the seed accounts and seed data of the test site) |
| docs/client | Also clientContact.md (who asked what and when) |
| docs/project | Also houseRules.md (the house rules of Connect) |

## Where the rest lives

- The issues, the sprints and the boards: GitLab, [Connect Nine](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine). Every card names other cards by its board number. [team/boardNumbers.md](team/boardNumbers.md) gives the GitLab number of each.
- The templates for a new issue and a new merge request: .gitlab/issue_templates and .gitlab/merge_request_templates.
- Decisions and messages: Microsoft Teams, as the collaboration agreement says.

## Keeping this up to date

A change that moves, adds or removes a document also changes this page, in the same merge request.

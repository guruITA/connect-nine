# What we need to talk about

One list of everything the team still has to talk over, so that no open point waits unseen in a merge request, a card or a chat. It is the agenda of the stand up and of the Friday at the client.

How we use it:

- Add a point with who it is for and where it comes from: an issue, a merge request, a decision file or a rule.
- When a point is settled, write the outcome where its source says (a decision file, a card, the code conventions), and move the point to Settled with the date and that place.
- The open decisions per document stay in [openItems.md](../team/openItems.md). A point from there comes here when it is due at a meeting.
- The open questions for the client stay in [clientQuestions.md](../client/clientQuestions.md). The ones to ask on the next Friday come here.

Started on 24 October 2026 by Wesley Oerlemans (#1556).

## At the next stand up

| Point | Who | Where it comes from |
|---|---|---|
| Approve !68, !69, !79, !88 and !100. Their pipelines are green. !68 and !69 need an approval again after their rebase onto dev. | Whoever reviews | The merge requests |
| Confirm rule 12 of General and Git, one way everywhere. Wesley brings the two names of `accounts.ts` to one way (below). | Whole team | !79, #1548 |
| Decision 008, the maintained fork of kaniko for the images: a line under Agreed by. Since !89 the container registry holds the images of every merge into dev (Deploy, Container registry). | Bayon, Nick, Gurpreet, Kelvin | [008-kanikoForImages.md](../decisions/008-kanikoForImages.md), #354 |
| Decision 009, a table session beside ASP.NET Core Identity: a line under Agreed by. Gurpreet's line is in !100. | Bayon, Kelvin | [009-sessionTableBesideIdentity.md](../decisions/009-sessionTableBesideIdentity.md), #1552 |
| The part Log in and sessions of the code conventions: read it before log in is built. Task 4.2 (#269) checks the password through `IAppDbContext` and the hasher of #262, where decision 007 and rule 1 of that part put the check behind a small interface of Application. | Nick | #1554, #15 |
| Rule 19, the keys of a validation answer: who brings `ToCamelCaseErrors()` to dev, and in which task. | Nick | !79, #1546 |
| Parent permission: a parent is asked for every young person under 18, and the address is kept at most one month. #40, #41, #42 and their tasks carry a dated note. | Gurpreet | Client meeting of 23 October 2026, #40 |
| The starter tags: who researches which tags fit Amstelland, young people and the Young Amstelland Awards, and by when. The current starter list stays until then. | Whole team | Question 9, #69 |
| Checks by a teammate who did not build it: the profile pages on a phone (#322, #27, #30, #69, #72, #351, #399), part 13 of the onboarding guide on a clean computer (#357), part 13 on a Mac with Apple silicon (!100), and both images built from a clean clone (#352, #354). | Volunteers | #32 |
| The test site starts the images of the registry instead of publishing the server in deploy-test. This changes deploy-test and needs a deploy token that only reads the registry. | Gurpreet, Wesley | #355 |
| The server check with the five of us is still to be held. Part 10 of the onboarding guide names the address of the server, while step 6 of the plan checks that it names none: we pick one of the two. | Whole team | #241, [serverCheckMeeting.md](../team/serverCheckMeeting.md) |
| The site shell story: the teacher asked on 22 October 2026 that we align on it. | Bayon, Wesley | #19 |
| k3s runs on the foundation's server since 25 October 2026. Who does task 93.3, the same install on a spare computer by someone who built no task of #157? | Bayon, Nick, Gurpreet, Kelvin | #157, !103 |
| How a change reaches production: a merge request that changes `infra/` gets a Terraform plan, and every merge of dev into main goes live by itself through `deploy-production`. Read part 14 Changing production of the getting started guide before you review such a merge request. The chain to review and merge in this order: !103, !105, !107, !108, !109, !110. | Whole team | !109, !110, #158, #162 |
| The first merge of dev into main. It is the first release: everything on dev goes live at https://live.31-70-66-186.sslip.io. When do we do it, and who checks the live address while it happens (task 96.3, #1060)? | Whole team | #162 |
| Decision 010, k3s with Terraform for the live version and docker compose for the test site, which we agreed to with the architecture documents in !4: one of us who did not write it answers from it alone in #156 why not docker compose in production, where the state lives and how a visit reaches production. Bayon adds his line under Agreed by. | Bayon, Nick, Gurpreet, Kelvin | [010-productionOnK3s.md](../decisions/010-productionOnK3s.md), !102, #156 |

## Where the code differs from our rules

Checked against the code on dev on 24 October 2026. Rule 12 of General and Git asks us to choose one way per point, or to write the exception into its rule with the reason. Older code follows a rule once its file changes, so none of this blocks a merge. Rules 17 to 20 and backend rule 12 are in !79, which is not merged yet.

| What | Whose code | Rule | Where it comes from |
|---|---|---|---|
| `web/src/api/accounts.ts` reads each field of a wrong form under two names, `errors.Email ?? errors.email` and `errors.Password ?? errors.password` (lines 24 and 25). The server answers with `email` and `password` today. Wesley brings it to one name, with Nick's yes, since it is his file. | Nick | Rule 12 of General and Git | !79, #1548 |
| The keys of a validation answer come from the global `PropertyNameResolver` in `api/Connect.Presentation/DependencyInjection.cs` and `validation.ToDictionary()` in `AccountEndpoints`, where rule 19 asks for `ToCamelCaseErrors()` and no resolver. | Nick | Backend rule 19 | !79, #1546 |
| The setup of a database test is written out per class: `CreateDbContext` and `FindPostgresException` in `AccountDatabaseTests`, and `CreateDbContext` in `HealthDatabaseTests`, beside the shared helper `TestDatabase` that `ProfileDatabaseTests` uses since !67. | Nick, Gurpreet | Backend rule 20 | !79, #1547 |
| One `!` after a value is left: `warnings[0][2]!` in `AccountRegistrationServiceTests` line 163. | Nick | Backend rule 17 | !79, #1544 |
| A parameter of a primary constructor is used without a `private readonly` field: `dbContext` in `DatabaseHealthCheck`, and `next` and `logger` in `ExceptionHandlingMiddleware`. | Gurpreet, Nick | Backend rule 18 | !79, #1545 |
| `HealthAnswerWriter` answers with `Results.Problem`, where the rest of the server uses `TypedResults`. The rule names it and lets it stay until the file changes for another reason. | Gurpreet | Backend rule 12 | !79, Gurpreet's review of it |

## With the client on Friday

| Point | Who | Where it comes from |
|---|---|---|
| Which towns and areas are on the fixed list of places for Region. | Whole team | Question 136, #1553 |
| A date for the meeting with the director about the live version. The part Production of `clientQuestions.md` goes to the client two working days before it. | Wesley | Questions 111 to 116, #1041 |
| Show the live version at https://live.31-70-66-186.sslip.io. It runs on the foundation's server since 25 October 2026, with no real data until the director answers questions 111 and 114. | Wesley | #159, questions 111 and 114 |
| The domain www.youngamstellandconnect.nl: the client still has to buy it, and Bayon asks him by email. | Bayon | Question 113 |

## With the coach

| Point | Who | Where it comes from |
|---|---|---|
| The four questions for the coach: the Portflow dates for the research proposal and the R&D report, what the custom datapoints of sprints 3 and 4 review, the days of knowledge tests 1 and 2, and the date of the Decision Committee. No answer is recorded yet. | Wesley | [openItems.md](../team/openItems.md), part The coach |

## Settled

| Point | Settled on | Where the outcome is written |
|---|---|---|
| The log tests read from substitutes of NSubstitute instead of a logger written by hand, so rule 4 of Tests and mocking holds there. | 22 October 2026 | #312, Gurpreet's change of the log tests |
| Of the seven `!` after a value counted on 21 October 2026, six are gone from dev. | 24 October 2026 | The table above keeps the last one |
| Every claim of the Structure table of decision 002 names its source or is marked as an opinion. | 24 October 2026 | !84, #253 |
| The current starter tags stay, and the researched tags are added later as extra tags. | 24 October 2026 | Question 9 in `clientQuestions.md`, !97 |
| An organisation may see the profile of a member under 18 and contact that member only inside Connect, once a parent has given permission. | 24 October 2026 | Question 31 in `clientQuestions.md`, !97 |

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

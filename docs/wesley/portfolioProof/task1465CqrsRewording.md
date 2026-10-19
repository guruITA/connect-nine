# Proof: task 11.4 The tasks of every next sprint reworded to the CQRS shape (#1465)

Merge request !62, branch `docs/1465-cqrs-rewording`. Issue: 11 The team agrees on the layered architecture (#12). Where it happened: the tasks on the board, with the planning notes on #1465.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS4.

A point that needs someone else is marked TODO with who and what.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Why.** The team chose CQRS by feature on 1 October 2026, written down in `docs/decisions/002-layeredArchitecture.md`, and the code follows it. Many tasks on the board still described repository classes, use cases written as a service and the old folders. A teammate who picked one up would build a shape the team no longer uses.
2. **The fixed mappings.** #1465 holds the mappings, so a teammate can reword a task with only that task open. A repository or a use case service becomes one handler per use case with its command or query beside it. A test with a fake repository becomes a handler test on an in-memory SQLite ConnectDbContext. A rule of the real database gets a database test.
3. **What I reworded.** On 9 October 2026 the tasks of sprint 2 that had the old shape. On 14 October 2026 the 351 open cards of sprints 2 to 5, to the folders of the repository and the class name ConnectDbContext. On 19 October 2026 the 80 cards of sprints 2 to 5 that still had the old shape. The planning notes on #1465 list every changed card with what changed in at most fifteen words.
4. **One card before and after.** #743 had one line for the contract of the site pages: "ISitePageRepository.cs: ListPageAsync, FindBySlugAsync (never returns who saved it, reused by #102), FindBySlugForUpdateAsync and SaveNewVersionAsync". It now says that IAppDbContext gets the two DbSets. ListSitePagesQueryHandler and FindSitePageBySlugQueryHandler read them and UpdateSitePageCommandHandler writes them, each with its query or command beside it.
5. **Where a fixed mapping did not fit.** The mapping turns one repository into the handler of one use case. The site page repository of #743 held three use cases: list the pages, find one page and save a new version. So the card now asks for three handlers. That still matches decision 002. The decision is one handler per use case, with a query for a read and a command for a write. A second case is #1143. Once the answer "not yours" goes away, the own project of #1100 and the project page of #1142 are one use case. So FindProjectPageQueryHandler of #1142 takes over from FindOwnProjectQueryHandler.
6. **The cards follow a teammate's code.** Later on 19 October 2026 I chose to follow the account code Nick Hoebe wrote for #262 (!47), so its folders and names became the shape: `api/Connect.Application/Accounts/CreateAccount/` with `CreateAccountCommandHandler`, the contracts in `Common/Abstractions` and `IAppDbContext` with `IQueryable<T>`. I tried the new shape on #343 and #88 first. Then 190 cards of sprints 2 to 5 followed, listed in the second planning notes on #1465, and !58 brings decision 002 and the code conventions in line.
7. **How I checked it.** Every card was compared before and after: the same headings, the same number of checklist lines and the same weights, and no old phrase left. Every card read back from GitLab equal to what I wrote. I also searched the open tasks of sprints 2 to 5 for Repository, IRepository, Repositories and the only layer that talks to the database. The search now only finds the code repository in the git sense and a backup repository.

## TODO

- [ ] A teammate: reword one new task with only #1465 open at the planning of sprint 3, and write it in the planning notes on #1465.
- [ ] Wesley: at the planning of sprints 3 to 5, check the tasks added since 19 October 2026 for the old shape.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

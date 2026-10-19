# Proof: task 1.7 The conventions with examples and the getting started guide (#8)

Branch `docs/8-conventions-examples-and-onboarding`. Issue: 01 The repository, the pipeline and the team rules (#1). Files: `docs/team/codeConventions.md` and `docs/team/onboarding.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating, MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The handover risk.** A newcomer who needs the team to get started. The guide now says after each command in parts 1 and 2 what you should see, has the real clone address instead of a placeholder, and in part 7 says what to do with a red pipeline.
2. **Tool or reviewer.** Every rule of the conventions now ends in (tool: name) or (reviewer). A team that changes keeps the tool checks without anyone remembering them, and the (reviewer) marks show what a new reviewer must look at by hand.
3. **The walkthrough.**
   - Planned on 10 October 2026 as task #1520 (Task 1.9 under #1), assigned to Gurpreet Singh, who did not write the guide.
   - Done on 12 October 2026. Gurpreet Singh followed the guide alone and wrote the result in a comment on #1: 39 minutes from `git clone` to his first merge request, !46, about 21 of those minutes without a problem of his own setup, and the rest to a problem on his own laptop and a pause before opening the merge request. His questions are the numbered list in that comment. !46 merged into dev on 14 October 2026.
   - My decision of 14 October 2026: the team has no person who is new to the project. A walkthrough by a team member who did not write the guide is the closest the team can do, and it counts for the non functional criterion that a newcomer gets started within one hour.
   - Fixed in !42, each point with the question number from Gurpreet Singh's comment on #1:
     1. **dotnet-ef (question 1).** Added `.config/dotnet-tools.json`, pinning `dotnet-ef` at 10.0.12, the version of the Entity Framework Core packages in `api/Connect.Infrastructure/Connect.Infrastructure.csproj`. Part 1 now lists the tool, part 4 now says to run `dotnet tool restore` once before the first `dotnet ef` command. Checked in the worktree: `dotnet tool restore` restored `dotnet-ef` 10.0.12, and `dotnet ef --version` answered `10.0.12`.
     2. **Node (question 2).** Part 1's check sentence now says `node --version` starts with v22, the version in `web/package.json`, `web/.nvmrc` and the pipeline image, instead of naming the current LTS.
     3. **Two clones in folders with the same name (question 3).** Part 2 now says Compose names the containers and the volume after the folder, so a second clone needs another folder name or `docker compose -p <name>`. The two comments in `docker-compose.yml` that said every checkout gets its own database now say that holds only when the folder names differ.
     4. **Windows reserved ports (question 4).** Part 3 now adds the message `bind: An attempt was made to access a socket in a way forbidden by its access permissions`, the check `netsh interface ipv4 show excludedportrange protocol=tcp`, and that Windows can reserve a range such as 5358 to 5457. The other port to use, wherever the guide offered 5433, is now 15432.
     5. **pgAdmin (question 5).** Part 3 now says pgAdmin needs about half a minute after `docker compose ps` says healthy before http://localhost:5050 answers.
     6. **One connection string name (question 8).** My decision of 14 October 2026: the one name is `DefaultConnection`, the name the code and the tests already read. Changed in the production secret example in part 4 and the database password line in part 10 of `docs/team/onboarding.md`, in `docs/architecture/configuration.md` and in `deploy/.env.example`. A grep of the whole worktree for `ConnectionStrings` after the change leaves two other names: `ConnectionStrings:Default` on one `user-secrets set` line and one sentence of part 4, left untouched on purpose because !46 already rewrites that block, and `ConnectionStrings:Authentication` and `ConnectionStrings__Connect` named in `docs/wesley/portfolioProof/task9ClientServer.md`, a file this merge request does not touch.
     7. **The migrations path (question 9).** The comment in `docker-compose.yml` named `api/Connect.Infrastructure/Migrations`. It now names the real folder, `api/Connect.Infrastructure/Persistence/Migrations`.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Which tool enforces which rule.** TypeScript, ESLint and Prettier from #263, dotnet build and dotnet format from #259, the pipeline jobs of #266, env-file-check and branch-flow of #5. Until a tool exists, the reviewer checks the rule.
2. **Left out on purpose, to avoid clashing with open merge requests.** Rule 2 of For all code got its mark in !23, backend rules 1 and 8 in !27. Audit rule 14 got its mark after !21 of a teammate was merged on 9 October 2026. After !21 and !22 were merged, parts 5 and 6 got the commands that work today (`api/Connect.slnx`, the health address) and what you should see, and the npm steps say they start with #263.
3. **The decision of #7.** The conventions say what `docs/decisions/001-codeConventions.md` proposes. Once the team agrees, any difference is changed here.
   - TODO teammate who did not write the guide: write in the review what you think of the examples.

## TODO

- [x] Wesley: plan the walkthrough with a teammate (task #1520, Gurpreet Singh, 10 October 2026).
- [x] Wesley: the date of the walkthrough in #1. Done, 12 October 2026.
- [x] Gurpreet Singh (#1520): do the walkthrough and write the result in #1. Done, 12 October 2026.
- [x] Wesley: fix every question in the guide and write here what changed. Done in !42, questions 1 to 5, 8 and 9 above.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

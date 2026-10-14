# Proof: task 3.8 Build and test in the pipeline (#266)

Branch `chore/266-build-and-test-pipeline`. Issue: 03 Create an account (#14). Files: `.gitlab-ci.yml`, `docs/team/gitlabSetup.md` part Pipeline, `docs/team/onboarding.md` part 6.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk this removes.** Until now the pipeline only checked for a committed `.env` and for the branch flow. Code that did not build, or broke a test, could reach dev after one approval. The onboarding walkthrough of #1520 found exactly that gap: part 6 of the guide said the pipeline runs the same checks as the laptop, and it did not.
2. **The jobs follow the stages of #1.** `backend-build` and `frontend-build` go into stage build. `backend-test`, `frontend-lint` and `frontend-test` go into stage test, next to `env-file-check` and `branch-flow`. Every one of the five carries the same rule as `env-file-check`, so it runs on every merge request and once more on dev, never on the weekly schedule.
3. **Why every check is its own job.** A red run then shows what broke without opening a log:
   - `backend-build`: the server compiles, and `dotnet format` finds no layout change.
   - `frontend-build`: TypeScript type checks, and Vite builds the screen.
   - `backend-test`: every `dotnet test` of `api/Connect.slnx`, against a real PostgreSQL 16 service under the alias `database`, the same major version as `docker-compose.yml`.
   - `frontend-lint`: ESLint and Prettier find nothing.
   - `frontend-test`: every Vitest test.
4. **No password or connection string in the file.** `backend-test` builds `ConnectionStrings__DefaultConnection`, the one name the team uses since 14 October 2026, from three CI/CD variables of the project, `TEST_POSTGRES_USER`, `TEST_POSTGRES_PASSWORD` and `TEST_POSTGRES_DB`. Their values are throwaway test values, the password is masked, and none of them appears in `.gitlab-ci.yml`. No job in stage build or stage test has `allow_failure`.
5. **Checked on a laptop before the push.** `dotnet build api/Connect.slnx` and `dotnet format api/Connect.slnx --verify-no-changes` both passed with no change asked. `dotnet test` passed on the three test projects that need no database (`Connect.Tests`, `Connect.Application.Tests`, `Connect.Presentation.Tests`, four tests). `Connect.Infrastructure.Tests` needs a real PostgreSQL and only runs in the pipeline. In `web`: `npm run build`, `npm run lint`, `npx prettier --check .` and `npm test` (four files, 20 tests) all passed with no change asked. The file passed the CI lint of the project before the first push.
6. **The red run next to the green run.**
   - TODO Wesley: the pipeline id and duration of the first green run of this merge request.
   - TODO Wesley: the pipeline where one failing unit test turns only `backend-test` red, and the pipeline where one lint error turns only `frontend-lint` red.
7. **Someone else can keep it working.** Part 6 of `docs/team/onboarding.md` now ends with "Reading a red pipeline", a table of the five jobs with what each checks and the exact command that shows the same error on a laptop. The part Pipeline of `docs/team/gitlabSetup.md` names what each job runs, next to the existing lines for `env-file-check` and `branch-flow`, so the file and the team documents say the same.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **How it fits the merge request flow of #1.** Pipelines must succeed is on since 8 October 2026 (#5), so a red job in build or test now blocks the merge button the same way a committed `.env` already did. The reviewer rule stays: a reviewer never approves a red run.
2. **A teammate reading a red run with only the guide.**
   - TODO Wesley: a teammate who did not build this reads one of the two red runs with only `docs/team/onboarding.md` open, and writes in the review of this merge request what they could not figure out.
3. **What I add because of it.**
   - TODO Wesley: every point the teammate could not figure out, fixed in the guide in this merge request.

## TODO

- [ ] Wesley: link the pipeline id and duration of the first green run.
- [ ] Wesley: link the pipeline where only `backend-test` is red (a failing unit test) and the pipeline where only `frontend-lint` is red (a lint error).
- [ ] A teammate who did not build this: read one red run with only the guide, and write what was unclear in the review.
- [ ] Reviewer of the merge request: confirm in the review that the part Pipeline of `gitlabSetup.md` matches the five jobs in `.gitlab-ci.yml`.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

# Proof: task 1.1 The starting structure (#2)

Merge request: !13, branch `chore/2-repository-skeleton`, commit 74b8f81. Issue: 01 The repository, the pipeline and the team rules (#1).
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating, MS1 and MS4. Link the merge request and this file.

Every point the task asks for is below. A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk the .gitignore removes before anyone writes code.** A committed `.env` holds a password that stays in the Git history for ever, also after the file is deleted. The only fix then is to change the password everywhere. Proof, run on 7 October 2026 on this branch with a harmless test value in `web/.env`:

   ```
   $ git status --short --ignored web
   !! web/.env
   $ git status --short
   (no output)
   ```

   `!!` means Git ignores the file, and the normal `git status` does not offer it for a commit. `web/.env.example` stays in Git, so every setting still has an example value (rule 3 of the code conventions).

2. **Where my rules come from, and why a short file.** I added five lines, each with its source: `test-results/` and `playwright-report/` are the folders Playwright writes by default for task 3.7 (#265), and `api/**/wwwroot/` is the folder task 3.9 (#267) fills with the built screen, so only the pipeline makes it. The existing blocks for .NET, editors, Node.js and Docker I left as they were. A short file that every team member can explain line by line fits a team of five that is mostly new to .NET and React better than a full template of hundreds of lines nobody reads.

3. **What the reviewer could not follow in the README on the first try, and what I changed.**
   - TODO reviewer of !13: write in the merge request what was unclear in the README on the first read. Then I write here what I changed because of it.

## MS4 Making your work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **How the .editorconfig follows rule 2, and what issue 03 adds.** Rule 2 of `docs/team/codeConventions.md` says UTF-8, LF, 2 spaces for TypeScript, JSON, YAML and Markdown, 4 spaces for C#. The file sets exactly that, plus a final newline and no trailing spaces for every file. One extra block marks `**/Migrations/*.cs` as generated code: EF Core writes those files with a byte order mark, and without the block `dotnet format --verify-no-changes` in task 3.1 would fail on code nobody writes by hand. The tasks of issue 03 Create an account (#14) add no new rules to `.editorconfig`; task 3.1 only checks the layout with `dotnet format`.

2. **The folders for the screen and the server.** The task says to leave them to issue 03. They exist already: Nick Hoebe renamed them to `web` and `api` in merge request !6 and put the server solution in `api` in merge request !11. For the teammate who picks up issue 03: `api/Connect.slnx` is the server with four layers and a first create user endpoint, and `web` holds only `.env.example`, so task 3.5 starts the React app in `web`.

3. **The missing CSS line, fixed in a second merge request.** The checklist of task 1.1 asks for 2 spaces in CSS files as well, and the first `.editorconfig` and rule 2 both left CSS out. Branch `chore/2-editorconfig-css` adds `css` to the block for 2 spaces and adds JavaScript and CSS to rule 2, so the file and the rule say the same thing before the React app of task 3.5 (#263) adds its first `.module.css`.

4. **Lines that exist for later tasks.** `api/**/wwwroot/` is for task 3.9 (#267), where the pipeline copies the built screen into the server; the task names `backend/Presentation/wwwroot`, the folder is now under `api`. `test-results/` and `playwright-report/` are for task 3.7 (#265), the Playwright test that makes an account in a real browser.

5. **Confirmed after the merge.** Merge request !23, branch `chore/2-editorconfig-css`, merged 9 October 2026. Gurpreet Singh checked it on 9 October 2026, and I noted the check in the merge request: `.editorconfig` on dev lists `css` in the 2-space block together with `ts`, `tsx`, `js`, `jsx`, `json`, `yml`, `yaml` and `md`, and 4 spaces for `cs`, matching rule 2 of `docs/team/codeConventions.md`. The `.editorconfig` line of the checklist of #2 is ticked.

## TODO

What is still needed before this proof is complete. Tick a line when it is done.

- [ ] Reviewer of !13: in your own clone, make a `.env`, run `git status`, and write in the review that it is not listed (Done means of the task).
- [ ] Reviewer of !13: write in the review what was unclear in the README on the first read.
- [ ] Me: after the review, fill in MS1.3 with what I changed because of it.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

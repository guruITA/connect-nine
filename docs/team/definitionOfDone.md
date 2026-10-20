# Definition of Done, one list per type

Every issue carries the list of its type, in full, with a checkbox on every line. Never on a task. The same list for everyone: no line depends on frontend or backend. The issue templates in .gitlab/issue_templates copy only the bold sentence of each line. The "How to check" under it lives only here. A line that is not true for every issue of its type does not belong here: it becomes a non functional acceptance criterion on the issues where it applies.

This file is the source. When it changes, the templates in .gitlab/issue_templates change in the same merge request.

## Type slice, the list for every vertical slice (37 lines)

### Code

- [ ] **The work was done on its own branch and reached dev only through a merge request that a teammate approved.**
  How to check: the branch is named `<type>/<issue number>-<short name>` and started from dev, and the merge request into dev shows an approval. Nobody pushed directly to dev or main.
- [ ] **Every commit message follows the team format: type, scope, what changed and the issue number, for example feat(backend): add account endpoint (#3).**
  How to check: every commit in the merge request reads `<type>(<scope>): <what changed> (#<issue number>)`, as in docs/team/codeConventions.md.
- [ ] **The code follows the team's code conventions in docs/team/codeConventions.md.**
  How to check: the reviewer compares names, folders and structure with docs/team/codeConventions.md for every layer the change touches.
- [ ] **The linter reports no errors and no warnings, and the build passes.**
  How to check: the lint and build steps of the pipeline are green. On your own machine: `npm run lint` and `npx prettier --check .` in frontend, `dotnet build` and `dotnet format --verify-no-changes` in backend.
- [ ] **No password, key or other secret is in the code, in a commit or in any shared text.**
  How to check: `git grep -i -E "password|secret|token|connectionstring"` shows only placeholders. Real values live only in .env and in the settings of the server.
- [ ] **No leftover code: nothing commented out, no debug output, and every TODO names an issue.**
  How to check: search the change for commented out code, console.log, Console.WriteLine and TODO. Every TODO names an issue number.
- [ ] **The server keeps its layers apart: a handler reaches the data only through a contract of Application, such as IAppDbContext.**
  How to check: no file in api/Connect.Presentation uses the database context or SQL. A handler in api/Connect.Application uses a contract of Application, IAppDbContext or for the account IAccountRegistrationService, never ConnectDbContext, UserManager, Npgsql or SQL text. The compiler already refuses Domain code that uses another project. See docs/decisions/002-layeredArchitecture.md.

### Tests

- [ ] **Every rule in the code has a unit test that proves it.**
  How to check: every rule in Application or Domain has an NUnit test, and every frontend function without a screen has a Vitest test.
- [ ] **Every server address has an integration test for a correct request, a wrong input and a person who is not allowed.**
  How to check: a test with WebApplicationFactory covers one correct request, one wrong input and one person without permission (401 or 403). For an address that is open to everyone, the third case shows that it works without logging in.
- [ ] **Every new or changed screen has a test for loading, empty, filled and error.**
  How to check: React Testing Library tests with a fake server answer cover loading, empty (where a list can be empty), filled and error.
- [ ] **The story of this issue is tested once from start to finish in a real browser.**
  How to check: the Playwright test of this issue runs against the test site after the merge and passes.
- [ ] **The merge request names one test and shows that it fails when its rule is removed.**
  How to check: the merge request names the test and says in one sentence that it failed when the rule it checks was removed.
- [ ] **All tests pass in the pipeline, and none is skipped or switched off.**
  How to check: `dotnet test` and `npm test` are green in the pipeline, and no test is marked skip or ignore or is commented out.

### Peer test

- [ ] **A team member who did not build this issue ticked every acceptance criterion on the test site and wrote their name and the date in the issue.**
  How to check: every acceptance criterion is ticked, the name and date are in the issue, and that name is not the assignee of any task of this issue.

### Security

- [ ] **Every server address says who may use it, and anyone else is refused.**
  How to check: docs/architecture/api.md names the role for every address (visitor, member, organisation or admin), and the integration test shows that a wrong role gets 401 or 403.
- [ ] **Error messages never show technical details, such as database errors or internal names.**
  How to check: stop the database or send wrong input and read the answer: no stack trace, no database text, no internal names.
- [ ] **The logs never contain passwords, tokens, full email addresses or message texts.**
  How to check: read the log lines the change writes. None holds a password, a token, a full email address or a message text, and every error line has the request id.

### Screen

- [ ] **Every screen works on a phone (360 pixels wide) and on a laptop (1280 pixels wide) without scrolling sideways.**
  How to check: in the browser developer tools at 360 and at 1280 pixels wide nothing scrolls sideways. The merge request has one screenshot of each.
- [ ] **Everything can be used with only the keyboard, and you can always see where you are.**
  How to check: put the mouse away and press Tab. Every button, link and field is reached in a logical order, has a visible focus ring, and works with Enter or Space.
- [ ] **Every field has a visible label, every error message sits next to its field, and every meaningful image has an alt text.**
  How to check: look at every field, error and image of the changed screens.
- [ ] **The accessibility checker axe finds no problems, and the text contrast is at least 4.5 to 1.**
  How to check: run axe in the browser on every changed screen: zero problems.
- [ ] **The screen uses the Young Amstelland colours, fonts and spacing, and follows its design.**
  How to check: colours and fonts come from web/src/styles/tokens.css, and the screen matches its drawing on the Figma page Design directions. Every difference is explained in the merge request.
- [ ] **Every text on the screen is plain language, without technical words or codes.**
  How to check: read every text: no technical words, no codes, no developer names, and the same thing has the same name everywhere.
- [ ] **Every button and link on a phone is at least 44 by 44 pixels.**
  How to check: measure in the developer tools at phone size. (A proposal, to be confirmed with the client.)

### Review

- [ ] **A teammate who did not write the code ran it on their own laptop and approved the merge request.**
  How to check: the approval in the merge request comes from a teammate who wrote that they ran it on their own laptop.
- [ ] **Every review comment is fixed or answered.**
  How to check: every comment in the merge request is resolved with a fix or an answer, none is only closed.
- [ ] **The board is correct: the tasks are Done, and "is blocked by" and "blocks" are linked both ways.**
  How to check: every task is Done, and Linked items shows the same link on both issues.
- [ ] **The team heard about the change at the stand up, and the issues that waited on this one know they can start.**
  How to check: it was mentioned at the daily stand up, and every issue this one blocks has a comment that it can start.

### Deploy

- [ ] **The pipeline put the change on the test site, database changes included, without manual steps.**
  How to check: the deploy stage is green and applied the database migration. Nobody changed the test site by hand.
- [ ] **The merge request names the version to go back to if something breaks.**
  How to check: the merge request names the previous version of the app and the migration to go back to, as in issue 17 (rollback).
- [ ] **The health check of the test site answers OK after the deploy.**
  How to check: the health address of the test site answers OK after the deploy.
- [ ] **Every new setting is in .env.example with an example value.**
  How to check: every new setting is read from the environment and is listed in .env.example with an example value.

### Docs

- [ ] **The description of the architecture and the list of server addresses in docs are up to date.**
  How to check: docs/architecture/architecture.md and its diagrams are updated when this issue added a table, an address or a layer, and docs/architecture/api.md describes every new or changed address with who may call it, what goes in and every answer.
- [ ] **Every important choice is its own file in docs/decisions, with the date, who decided, the options and why.**
  How to check: every choice of a library, a structure or a rejected alternative is a file `docs/decisions/<number>-<shortName>.md` with the date, who decided, the options, the choice and why the others lost.
- [ ] **The BoKSA proof that each task asks for is in the merge request, ready for Portflow.**
  How to check: every task's proof block is filled in its merge request description.
- [ ] **Where AI helped, it is declared as the collaboration agreement asks.**
  How to check: the team declaration from section 9 of the collaboration agreement is in the merge request.
- [ ] **Anything the team learned while building is added to the issue.**
  How to check: a surprise or a changed plan is written in the issue in GitLab.

## Type setup

For example the repository, access to the server, a tool for the whole team.

- [ ] What was set up works for every team member, and each of them confirmed it in the issue.
- [ ] How to set it up again is written step by step in docs.
- [ ] No secret is in Git or in the issue. Secrets live only in .env or in the settings of the server.
- [ ] A teammate checked it and wrote their name and the date in the issue.
- [ ] The BoKSA proof is collected, and AI use is declared.

## Type research

A question the team answers with sources.

- [ ] The question is answered in one document in docs, with at least two sources for every claim.
- [ ] The document ends with a conclusion and what the team does with it.
- [ ] A teammate read it and wrote their name and the date in the issue.
- [ ] The issues that need the answer are linked and told.
- [ ] The BoKSA proof is collected, and AI use is declared.

## Type decision

A choice between options.

- [ ] The choice is one file in docs/decisions, named with a number and a short title, for example docs/decisions/003-minimalApis.md, with the date, who decided, the options, the choice and why the others lost.
- [ ] The team agreed at a stand up or a meeting, and that is written in the issue.
- [ ] The documents and issues that change because of the choice are updated.
- [ ] The BoKSA proof is collected, and AI use is declared.

## Type client

A question to the client.

- [ ] The question was asked to the client, and the answer is written in the issue with the date.
- [ ] The answer is in the document it belongs in, such as docs/project/requirements.md or a file in docs/decisions.
- [ ] The issues that waited on the answer are updated and told they can start.
- [ ] The BoKSA proof is collected, and AI use is declared.

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to draft
the formulation of sentences, the structure of wording, readability, and clarity from
content that we had provided, and to generate the diagrams. The content itself, including
our experiences, reflections, learning goals, decisions, self-assessments, and
conclusions, is our own. We reviewed the drafts and decided as a team which text to
include.

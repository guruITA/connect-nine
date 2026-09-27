<!-- Title of the issue: the story, "As a <real role>, I want <what>, so that <what it gives me>." -->

Board number <number> · rank <number> · <epic> · weight <1, 2, 3 or 5>

## DESCRIPTION

Connect is the online platform of the WailSalutem Foundation. On it, young people from Amstelland, the organisations around them and the foundation find each other all year, not only at the Young Amstelland Awards gala.

This issue is for <who, with an example person>.

When it is done, <what that person can do and see>. <What they see when something goes wrong.>

<When it applies: why this issue is special, for example its weight.>

## OUT OF SCOPE

- <what a reader might expect here>: issue <number> <title>

## LINKED ITEMS (GitLab)

Is blocked by
- <number> <title> · epic <name>

Blocks
- <number> <title> · epic <name>

Relates to (docs/requirements.md)
- <FR code> <name> · epic <name> · <first issue of that epic, or why it relates>

Quality rules this issue follows (docs/requirements.md)
- <NFR code> <name> · epic <name> · <issue that holds the rule, if there is one>

## HOW IT WORKS (for developers)

Words on this card
- <word>: <one plain sentence>

The road the data travels
1. The screen. <what the person does and what the frontend sends where>
2. The server, through its four layers, like a restaurant:
   - Presentation (the waiter) <takes the request, brings back the answer>
   - Application (the chef) <the rule of this issue>
   - Domain (the menu) <what it describes for this issue>
   - Infrastructure (the storeroom) <what it reads or writes>
3. The database. <which table, what changes>

The answer goes back the same way, and the screen shows <what>. All words and setup steps: docs/onboarding.md. The rules for names and layout: docs/codeConventions.md.

## TESTING, WHAT PROVES THIS ISSUE

- [ ] <what it proves in plain words>: <unit, integration, screen, end to end or manual>, task <n.m>

## ACCEPTANCE CRITERIA

Checked on the test site by a team member who did not build this issue.

Functional
- [ ] AC1 As a <role>, I <see or do what, and where on the screen>.
  How to check: <page, button, what to type, what you see>.

Non functional
- [ ] AC<n> As a <role>, I <privacy, security or quality rule special to this issue>.
  How to check: <page, button, what to type, what you see>.

## DEFINITION OF DONE (type slice)

The same list for every slice and for everyone. docs/definitionOfDone.md explains how to check every line.

**CODE**
- [ ] The work was done on its own branch and reached dev only through a merge request that a teammate approved.
- [ ] Every commit message follows the team format: type, scope, what changed and the issue number, for example feat(backend): add account endpoint (#3).
- [ ] The code follows the team's code conventions in docs/codeConventions.md.
- [ ] The linter reports no errors and no warnings, and the build passes.
- [ ] No password, key or other secret is in the code, in a commit or in any shared text.
- [ ] No leftover code: nothing commented out, no debug output, and every TODO names an issue.
- [ ] The server keeps its four layers apart: only the Infrastructure layer talks to the database.

**TESTS**
- [ ] Every rule in the code has a unit test that proves it.
- [ ] Every server address has an integration test for a correct request, a wrong input and a person who is not allowed.
- [ ] Every new or changed screen has a test for loading, empty, filled and error.
- [ ] The story of this issue is tested once from start to finish in a real browser.
- [ ] The merge request names one test and shows that it fails when its rule is removed.
- [ ] All tests pass in the pipeline, and none is skipped or switched off.

**PEER TEST**
- [ ] A team member who did not build this issue ticked every acceptance criterion on the test site and wrote their name and the date in the issue.

**SECURITY**
- [ ] Every server address says who may use it, and anyone else is refused.
- [ ] Error messages never show technical details, such as database errors or internal names.
- [ ] The logs never contain passwords, tokens, full email addresses or message texts.

**SCREEN**
- [ ] Every screen works on a phone (360 pixels wide) and on a laptop (1280 pixels wide) without scrolling sideways.
- [ ] Everything can be used with only the keyboard, and you can always see where you are.
- [ ] Every field has a visible label, every error message sits next to its field, and every meaningful image has an alt text.
- [ ] The accessibility checker axe finds no problems, and the text contrast is at least 4.5 to 1.
- [ ] The screen uses the Young Amstelland colours, fonts and spacing, and follows its design.
- [ ] Every text on the screen is plain language, without technical words or codes.
- [ ] Every button and link on a phone is at least 44 by 44 pixels.

**REVIEW**
- [ ] A teammate who did not write the code ran it on their own laptop and approved the merge request.
- [ ] Every review comment is fixed or answered.
- [ ] The board is correct: the tasks are Done, and "is blocked by" and "blocks" are linked both ways.
- [ ] The team heard about the change at the stand up, and the issues that waited on this one know they can start.

**DEPLOY**
- [ ] The pipeline put the change on the test site, database changes included, without manual steps.
- [ ] The merge request names the version to go back to if something breaks.
- [ ] The health check of the test site answers OK after the deploy.
- [ ] Every new setting is in .env.example with an example value.

**DOCS**
- [ ] The description of the architecture and the list of server addresses in docs are up to date.
- [ ] Every important choice is its own file in docs/decisions, with the date, who decided, the options and why.
- [ ] The BoKSA proof that each task asks for is in the merge request, ready for Portflow.
- [ ] Where AI helped, it is declared as the collaboration agreement asks.
- [ ] Anything the team learned while building is added to the issue.

<!-- The tasks are child items of this issue (Child items, Add, New task), one per layer. Labels on the issue: type, epic, prio, sprint, and blocked when it applies. -->

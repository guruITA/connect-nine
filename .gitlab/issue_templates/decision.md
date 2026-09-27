<!-- Title of the issue: the story, "As a <real role>, I want <what>, so that <what it gives me>." -->

Board number <number> · rank <number> · <epic> · weight <1, 2, 3 or 5> · decision, not a slice

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

Relates to (docs/project/requirements.md)
- <FR code> <name> · epic <name> · <first issue of that epic, or why it relates>

Quality rules this issue follows (docs/project/requirements.md)
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

The answer goes back the same way, and the screen shows <what>. All words and setup steps: docs/team/onboarding.md. The rules for names and layout: docs/team/codeConventions.md.

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

## DEFINITION OF DONE (type decision)

- [ ] The choice is one file in docs/decisions, named with a number and a short title, for example docs/decisions/003-minimalApis.md, with the date, who decided, the options, the choice and why the others lost.
- [ ] The team agreed at a stand up or a meeting, and that is written in the issue.
- [ ] The documents and issues that change because of the choice are updated.
- [ ] The BoKSA proof is collected, and AI use is declared.

<!-- The tasks are child items of this issue (Child items, Add, New task), one per layer. Labels on the issue: type, epic, prio, sprint, and blocked when it applies. -->

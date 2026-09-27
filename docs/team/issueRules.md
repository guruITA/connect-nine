# Rulebook: how to write an issue and its tasks

The source is section 1 of the page Issue creation on the Figma board. When the board changes, this file changes in the same merge request.

The complete rule set, set by Wesley on 25 and 26 September 2026. A person or an AI that follows these rules, and nothing else, writes issues and tasks exactly like the example in section 2. Nothing here depends on another page: section 2 shows the result, section 3 holds the Definition of Done that rule C10 copies, section 3b the code conventions that rule F6 points to. Group H says how the issues stand on a feature page, as a pyramid. When anything else on this board disagrees with this rulebook, the rulebook wins.

## What an issue is

### A1

Every issue is written for someone outside the team. The reader is a second year student who joins in week 10, has never used React, .NET, EF Core, PostgreSQL, Docker or the test tools, and must be able to do the work without asking anyone. The client and a teacher must understand the description and the acceptance criteria.

### A2

An issue is a vertical slice: one path from the screen through the server to the database that does one thing or shows one thing to a person using Connect. Two paths are two issues, however small the second one is. A form that creates something and the page that shows the result are two issues. An action and the list where you see it afterwards are two issues. Log in and log out are two issues.

### A3

Every slice has all three layers as tasks: at least one database task (a table, a column, an index, a query or seed data), one backend task and one frontend task, plus the tests. A separate frontend, backend or database issue does not exist. Those layers are tasks in the first slice that needs them.

### A4

Make every slice as small as possible while a person using Connect still notices it. When a slice holds several business rules, split by rule: first the basic version, then one issue per rule that improves or extends it. This is the agile way: first build the basic version, then improve or extend it. Example: 03 Create an account saves an account and refuses an email address that is taken. The password rules (at least fifteen characters, typing it twice the same) are a slice of their own, 03b A clear message when my password is too weak, because each rule with its message can be built and tested on its own.

### A5

Build only what the client needs. A slice exists because of a requirement in docs/requirements.md, or because another required slice cannot start without it. Never invent a feature only to prove the technology works. The first technical setup (the server project, the database, the pipeline, the first deploy) goes into the first real slice. Example: the old card 02 (the home page shows how many members Connect has) was removed on 26 September 2026 because no requirement asks for a member count. It only existed to connect the screen, the server and the database for the first time, so that setup moved into 03 Create an account. Exception, decided by Wesley on 27 September 2026: an issue that an HvA workshop requires may exist without a client requirement. It carries the label school::required and the part SCHOOL REQUIREMENT (rule C1), which says it is there because of school and not because the client asked for it, names the workshop and says why school asks it. It is still as small as possible, the client does not pay for it and it never delays a slice the client needs.

### A6

Work that is not a slice is still an issue, so the team records what it did and will do. It gets one type label: type::setup (tools, access, environments), type::research (a question answered with sources), type::decision (a choice between options), type::client (a question to the client). Keep these as few as possible. A normal slice has type::slice.

### A7

Weight is size and uncertainty compared with a known example, never a number of hours: 1, 2, 3 or 5, as in the weight scale in section 2. A 5 is a signal to split again. The only accepted 5 is a slice that cannot be split without making horizontal cards, and its description says why.

## The story

### B1

The title of an issue is a user story: "As a &lt;role>, I want &lt;what>, so that &lt;what it gives me>." It is specific, and a person can check afterwards whether it is true.

### B2

The role is always a real person who uses Connect: a visitor, a person new to Connect, a young person, a member, an organisation, or a foundation employee named by function (admin of the foundation, platform owner, director, data protection contact). The role of an issue is NEVER a developer or the team, because we never build for ourselves. Only on an issue with school::required, when it serves school alone: a teacher of the HvA or the coach of the team. Rachid Kherrazi is the client: the director of the foundation and the contact for how the project must be. There is no separate data protection contact, the team writes the privacy rules and the client approves them.

### B3

The title of a task uses the same form with the developer as the role, and its "so that" names what the task makes possible for the role of the issue. Example: "As a developer, I want an address on the server that saves a new account, so that a person new to Connect can create an account."

## The parts of an issue, in this order

### C1

An issue has exactly these parts, in this order, each with its heading in capitals, and a blank line between parts and between paragraphs: the RANK line, STORY, DESCRIPTION, OUT OF SCOPE, LINKED ITEMS (GitLab), HOW IT WORKS (for developers), TASKS, ONE LAYER EACH, TESTING, WHAT PROVES THIS ISSUE, ACCEPTANCE CRITERIA, DEFINITION OF DONE, LABELS. The acceptance criteria always sit directly above the Definition of Done. An issue with the label school::required has one more part, SCHOOL REQUIREMENT, directly under STORY.

### C2

The RANK line: "RANK &lt;number> · &lt;EPIC> · weight &lt;n>". The rank is the build order on the board.

### C3

DESCRIPTION: plain language without technical words, for someone outside the team. Short paragraphs with a blank line between them. One paragraph on what Connect is: the online platform of the WailSalutem Foundation where young people from Amstelland, the organisations around them and the foundation find each other all year, not only at the Young Amstelland Awards gala. One paragraph on who this issue is for. One paragraph on what that person can do when it is done, including what they see when something goes wrong. When it applies, one paragraph on why this issue is special, for example the first slice or its weight. Length follows the content, and every sentence carries information.

### C4

OUT OF SCOPE: a list of what a reader might expect here but is not part of this issue, each with the number and title of the issue where it is done.

### C5

LINKED ITEMS (GitLab): every link in one place, with the GitLab names. "Is blocked by": every issue that must be done first. "Blocks": every issue that cannot start before this one. "Relates to (docs/requirements.md)": the requirements this issue serves or prepares, each as code, name, epic and, when useful, the first issue of that epic. "Quality rules this issue follows (docs/requirements.md)": the NFRs, each as code, name, epic and, when there is one, the issue that holds the rule. Every issue is named as number, title and epic. Every link is set both ways in GitLab: when A blocks B, B says it is blocked by A.

### C6

HOW IT WORKS (for developers): first "Words on this card", a list that explains every technical word the card uses in one plain sentence, for a team that is new to React and .NET. Then "The road the data travels": the screen, the server with its four layers, and the database, in the order the data travels, with what happens in each for this issue. The four layers are always explained with the same picture: Presentation is the waiter, Application is the chef, Domain is the menu, Infrastructure is the storeroom. End with where to read more: docs/onboarding.md for words and setup, docs/codeConventions.md for names and layout.

### C7

TASKS, ONE LAYER EACH: one line per task with its number, its layer in capitals (BACKEND, DATABASE, FRONTEND, TEST, DEPLOY, DOCS) and a short name. Tasks are numbered &lt;issue number>.&lt;n> in build order.

### C8

TESTING, WHAT PROVES THIS ISSUE: a checklist with one line per test: what it proves in plain words, the kind of test, and the task that writes it.

### C9

ACCEPTANCE CRITERIA: first the line "Checked on the test site by a team member who did not build this issue." Then two parts, each with its own heading: Functional and Non functional. Every criterion starts with "[ ] AC&lt;n> As a &lt;role>, I ..." with the role of the story, or a second real role when the issue touches two. It describes what that person sees or can do on the screen, in plain words, never a status code, a table, a name from the code or an NFR code. When a text appears, it says where: on the page in place of what, next to which field, as a popup or not. It is followed by "How to check:" with the page, the button, what to type and what you see, so a person without technical knowledge can do it on the test site. Together the criteria cover the normal path, every error the person can meet (wrong input, a value that is taken, the server not reachable) and every privacy or security rule that is special to this issue. There are as many as the issue needs. A rule that holds for every slice is not repeated here, it is in the Definition of Done. No criterion needs a developer to change the database by hand: when a check needs known data, the pipeline puts it there (seed data) and docs/testData.md lists it.

### C10

DEFINITION OF DONE: the list for the type of the issue, copied in full from section 3, with a checkbox on every line, the category names between two asterisks. Never on a task.

### C11

LABELS: type, epic, priority, sprint, and blocked with its reason when it applies. The portfolio labels (outcome, level, direction) are never on the issue. An issue that exists because of a school workshop also gets school::required.

## The parts of a task, in this order

### D1

A task is one thing on one layer: a table, an endpoint, a screen, seed data, a test in a real browser, a pipeline step, a deploy or a document. Never "the form and the page".

### D2

The unit tests and integration tests belong to the task that writes the code they test. Only the end to end test in a real browser and the manual team check are tasks of their own.

### D3

A task has exactly these parts, in this order, each with its heading and a blank line between parts: the TASK line, STORY, LABELS, DESCRIPTION, BEFORE YOU START, a context block when the task works inside a structure (for a backend task: THE FOUR FOLDERS YOU WORK IN), FILES, BUILD THEM IN THIS ORDER, WATCH OUT, CHECK IT ON YOUR OWN MACHINE, DONE MEANS, ACCEPTANCE CRITERIA, CHECKED BY THE REVIEWER, and PROOF FOR THE PORTFOLIO (BoKSA).

### D4

The TASK line: "TASK &lt;n.m> · &lt;LAYER> · own assignee, own status".

### D5

LABELS: one outcome label for every learning outcome the proof block covers (outcome::analysing-and-advising, outcome::designing-and-realising, outcome::managing-and-collaborating), one level label for every level that can do the task (level::2, level::3), and one direction (direction::full-stack, or direction::security-specialist for a task of the security specialist).

### D6

DESCRIPTION has two parts. "In short": one complete paragraph that says what you build, why, where it sits on the road of the data, what the result is when it works and when it fails, which tests and docs you write, and which task can start after it. Someone who reads only this paragraph knows the task without reading the issue. "In detail": a list with the exact behaviour: the address and method, what comes in, every answer with its code and an example, the rules, and who may use it. Every code gets its meaning in words, for example 409 ("conflict").

### D7

BEFORE YOU START has three headings of its own. "Needs first": the task or issue that must be done, with what it made. "Issue": number, title, epic and page. "New to .NET, React, the four layers or the tests?": read docs/onboarding.md first, which explains every word and how to get the code, make the branch &lt;type>/&lt;issue number>-&lt;short name> and start the database.

### D8

The context block explains the structure the task works in, for a beginner: what each folder is for, that the names are standard names and where the team chose them, with the same picture of the waiter, the chef, the menu and the storeroom.

### D9

FILES, BUILD THEM IN THIS ORDER: a numbered list of every file with its full path from the root of the repository, in the order to build them. Under each file, bullets in plain words: what it is, what it does, and for a test file every case it proves.

### D10

WATCH OUT: every rule that is easy to break, each followed by a line "Why:" with the reason in plain words, so a beginner understands the rule instead of only obeying it.

### D11

CHECK IT ON YOUR OWN MACHINE: numbered steps. Each step says what to do, the exact command on its own line, and "You see:" with what appears when it works. The steps start what is needed, run the tests, walk the normal path and every error path, check that nothing secret is stored or shown, and end by putting the machine back in its normal state.

### D12

DONE MEANS: a short list: the merge request is open, the pipeline is green, what the reviewer ran and saw, which docs are updated, and that the proof is in the merge request.

### D13

ACCEPTANCE CRITERIA, CHECKED BY THE REVIEWER: a checklist of what the reviewer checks for this task. Technical words are allowed here, because the reader is a developer.

### D14

A task never carries the Definition of Done. That belongs to the issue.

## The proof block of a task (boksa)

### E1

For the grade the proof block counts more than the code. Never shorten it. It is complete enough that a student knows exactly what to make, how and why, without opening another document.

### E2

It starts with three lines. "Why this matters": the block counts more than the code, and a high score comes from showing your thinking, not from showing that it works. "Where it goes": everything goes in the description of the merge request, which is linked in Portflow under the code named. "What makes it level 3": at level 2 you show your plan and that it works, at level 3 you also name the alternative you rejected and why, name the standard you followed, and say what you changed after testing.

### E3

Then one heading per level: "LEVEL 3, FULL-STACK DEVELOPER" always, and "LEVEL 2, FULL-STACK DEVELOPER" when a level 2 student can do the task. Level 2 is left out when the core of the task is choosing and justifying (decide, choose, compare, weigh). A task of the security specialist uses the heading "SECURITY SPECIALIST" and the wording of that profile.

### E4

Under each level, per BoKSA code, a heading "&lt;code> &lt;what it means in plain words>" and these lines. "What school asks:" the success criterion of that level, quoted word for word from the BoKSA site. "Why this task proves it:" one or two sentences. "What you make:" a numbered list of the concrete things to write or paste. At level 3 with OR1 also "After testing, add:" what to add after the tests. "Hand in:" merge request description, Portflow under &lt;outcome> &lt;code>.

### E5

Which codes fit which task: a table or seed data OR1 and OR4. An endpoint OR1, OR2, OR4 and OR5. A screen OR1, OR3, OR4 and OR5. A test task OR4. Research AA1, AA2 and AA4. A decision AA4 and AA5. Process and pipeline work MS1 and MS4. PL never comes from a task, it comes from reflection at the sprint review.

### E6

The standards named under OR2 are real and specific, for example RFC 9110 for status codes, RFC 9457 for ProblemDetails, the OWASP Password Storage Cheat Sheet, WCAG 2.2 for accessibility, and the BoKSA article that fits.

### E7

OR5 at level 3 always asks for the student’s own judgment on a real trade-off in this task, named concretely.

## Writing style

### F1

Everything on the board and in GitLab is in English.

### F2

No em dashes and no semicolons in written text.

### F3

Plain words first. A technical word is explained where the reader meets it: in "Words on this card" on the issue, in the context block of a task, or in brackets.

### F4

Every part has a heading, and a blank line separates parts and paragraphs, so a reader who scrolls finds every part at once.

### F5

A reference to another card always names its number, its title and its epic, and where needed its page on this board.

### F6

One set of names for the whole project, the same on every card: the folders frontend and backend, the backend projects Presentation, Application, Domain, Infrastructure and Connect.Tests, docs/requirements.md, docs/architecture.md, docs/api.md, docs/onboarding.md, docs/codeConventions.md, docs/definitionOfDone.md, docs/testData.md, the folder docs/decisions, the docker compose service database, and server addresses under /api. Names in code follow section 3b.

### F7

Everything a reader needs is in the issue and the task themselves. Documents in docs may explain more, but never replace what the card must say.

## How the issues stand on a feature page (the pyramid)

### H1

Every feature page (Issues 01 to Issues 21) shows its issues as a pyramid, read from top to bottom, so everyone sees which issue can start now and which issue waits for another one.

### H2

A feature page has two parts. At the top the pyramid map: one small box per issue. Below it the full issue cards, in the same layers, layer 1 first, and inside a layer from left to right in the same order as the map. Every layer of cards has the same label as in the map. Every card stands exactly under its box in the map, in the same slot, so the cards form the same pyramid as the map. Every layer of cards starts with a thick navy line and the large title LAYER &lt;n> in bold, readable when you zoom out, with the line that says when it can start under it.

### H3

Layers. An issue that waits for nothing, or only for issues on other pages, is in layer 1. Every other issue is one layer below the lowest issue on this page that it waits for. Layers are numbered from 1 at the top. A layer holds as many issues as is true, also 20 or more.

### H4

A box in the map holds: the number and title, "Is blocked by:" with the numbers on this page (or the number on another page for layer 1), "Blocks:" with the numbers on this page or "nothing on this page", "Blocks on other pages:" with the numbers and their epic when there are any, and the weight.

### H5

Lines. A line with an arrow runs from an issue to every issue on this page that waits for it. An issue that waits for two issues gets two lines. A line never crosses a box: it runs down from the bottom of a box, sideways halfway between two layers, and down into the top of the next box.

### H6

Order inside a layer. An issue stands under the issue that blocks it, so every branch forms its own block, and a parent is centred above its children. An issue that waits for more than one issue stands under the first of them in rank order.

### H7

Issues on other pages. An issue on another page that blocks an issue here is drawn above the layer where it is needed as a grey box with a dashed border, with its number, title, epic and page, and a dashed line to the issue that waits for it. What an issue blocks on other pages is written inside its own box.

### H8

Every layer has a label on the left: "LAYER &lt;n>" and one line that says when it can start, for example "22 waits for 21, 24 and 25 wait for 23". The bands of the layers alternate two light greys so they are easy to tell apart. Above the map one line explains how to read it: from top to bottom, a line runs from an issue to every issue that waits for it, everything in one layer can be built at the same time once the issues above it that it is connected to are done.

### H9

Issues that are not slices (setup, research, decision, client) stand in the pyramid too, in their layer, with their type in the box.

### H10

One truth for every link. The box, the Linked items on the card and the links in GitLab use the same words (Is blocked by, Blocks) and say exactly the same. When a link changes, all three change together.

### H11

The rank number is the build order over the whole board and follows the layers: an issue never has a lower rank than an issue that blocks it.

## Before a card is finished, check

### G1

- [ ] The role in the story is a real person, not a developer.
- [ ] The issue is one path, and a person using Connect notices it.
- [ ] It exists because of a requirement, or because a required slice needs it first.
- [ ] Every part is there, in the order of C1 for an issue and D3 for a task, with its heading.
- [ ] The description has no technical word.
- [ ] Out of scope names the issues where the rest is done.
- [ ] Every link under Linked items is also set on the other issue.
- [ ] Every acceptance criterion starts with "As a &lt;role>", says where things appear on the screen, and has How to check.
- [ ] No acceptance criterion needs hand work in the database.
- [ ] Every slice has a database, a backend and a frontend task, and the unit and integration tests are inside the task that writes the code.
- [ ] Every task description has In short and In detail, every file has its bullets, every Watch out has its Why, every check step has You see.
- [ ] The proof block has Level 3, Level 2 when a level 2 student can do it, and every code has What school asks, Why this task proves it, What you make and Hand in.
- [ ] The Definition of Done of the right type is on the issue, and on no task.
- [ ] The issue stands in the right layer of its feature page, and its box in the pyramid map, its Linked items and GitLab say the same.
- [ ] No em dashes and no semicolons.

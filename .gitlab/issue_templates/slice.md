<!-- Title: "As a <real role>, I want <what>, so that <what it gives me>." Never the developer or the team as role. -->

Board number <number> · rank <number> · <epic> · weight <1, 2, 3 or 5>

## Description
<!-- 2 to 4 plain sentences: who, what they do, why. No technical words. Refer to other issues by their GitLab number. -->

## Paths (for developers)
Each unhappy path has the number of the happy path step where it goes another way.

Happy path
1. <what the person does on the screen>
2. <what the screen sends to which server address>
3. <what the server checks and decides>
4. <what is read or written in which table>
5. <what the person sees>

Unhappy paths
- 1a. <what goes wrong at step 1>: <what happens, what the person sees>
- 3a. <what goes wrong at step 3>: <what happens, what the person sees>

## Acceptance criteria
<!-- Plain words for a tester without technical knowledge: what the role notices, no files, addresses, tables or codes. -->
Functional
- [ ] As a <role>, I <can do or see what>.

Non functional
- [ ] As a <role>, I <notice what about privacy, safety, speed or access>.

## Out of scope
- <what a reader might expect here> (#<issue>)

## Definition of done
Details per line: docs/team/definitionOfDone.md
- [ ] Code: own branch and approved MR, commit format, code conventions, linter and build green, no secrets, no leftover code, handlers reach the data only through IAppDbContext
- [ ] Tests: a unit test per rule, integration tests per server address (correct, wrong input, not allowed), screen states (loading, empty, filled, error), one browser test of the story, one test shown to fail without its rule, all green and none skipped
- [ ] Peer test: a teammate who did not build it ticked every AC on the test site, with name and date
- [ ] Security: every server address says who may use it, no technical details in errors, no secrets or personal data in logs
- [ ] Screen: phone (360) and laptop (1280), keyboard only, labels and alt texts, axe clean and contrast 4.5:1, Young Amstelland design, plain language, buttons 44 by 44
- [ ] Review: a teammate ran it and approved, every comment answered, board and links correct, team told at the stand up
- [ ] Deploy: pipeline deploys without manual steps, MR names the version to go back to, health check OK, new settings in .env.example
- [ ] Docs: architecture and address list up to date, choices in docs/decisions, BoKSA proof in the MR, AI use declared, lessons added to the issue

<!-- Tasks are child items (template task), one per layer. Is blocked by and Blocks are Linked items, not text. Labels on the issue: type, epic, prio, sprint, and blocked when it applies. -->

# Proof: task 1.6 The team confirms the code conventions (#7)

Branch `docs/7-code-conventions-decision`. Issue: 01 The repository, the pipeline and the team rules (#1). File: `docs/decisions/001-codeConventions.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Analysing and Advising, AA4 and AA5.

A point that needs someone else is marked TODO with who and what.

## AA4 Giving advice that weighs the alternatives

School asks: "You give well-founded advice in which you weigh alternatives, identify risks and make a substantiated recommendation."

1. **The proposal per choice.** Four choices, each with two options in a table and a source: frontend folders by kind or per feature (React, n.d.), snake_case or PascalCase in the database (PostgreSQL 16 documentation on identifiers), plural nouns or verbs in server addresses (Microsoft, Web API design best practices), Conventional Commits or free text (Conventional Commits 1.0.0). Every source was read on 8 October 2026, and every quote it is based on was checked on the page itself.
2. **The risk of each recommendation.** Folders by kind can get crowded past sprint 3, written in Consequences with the way out (a new decision file). snake_case costs one package. The other two have no real cost for this team.
3. **What the team chose and where it differed from the advice.**
   - TODO Wesley: after the team moment, write what the team chose per point and where it differed, with the reason.

## AA5 Adapting the message to who reads it

School asks: "You adapt your communication to different target groups in form and language."

1. **For the client.** The Context of the decision file opens with why this matters to him: someone outside the team keeps Connect after January 2027. No GitLab words in that paragraph.
2. **For the team.** The options are tables with one row per thing a developer notices (where a new file goes, typing SQL by hand), and the exact rule numbers of `docs/team/codeConventions.md`.
3. **Left out on purpose.** The split of the server into layers, because that is #12 and a second place for the same choice would drift apart.
4. **The question that showed a first explanation did not land.**
   - TODO Wesley: after the team moment, write the question and what changed in the file.

## TODO

- [ ] Whole team: walk the four choices and point 5 at a stand up, then each fills in their date in Agreed by.
- [ ] Whole team: each confirms in a comment on #1 that they can follow the rules.
- [ ] Wesley: fill in AA4 point 3 and AA5 point 4 after the team moment.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

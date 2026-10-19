# Proof: task 1.6 The team confirms the code conventions (#7)

Branch `docs/7-code-conventions-decision`. Issue: 01 The repository, the pipeline and the team rules (#1). File: `docs/decisions/001-codeConventions.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Analysing and Advising, AA4 and AA5, and for !59 Managing and Collaborating, MS4.

A point that needs someone else is marked TODO with who and what.

## AA4 Giving advice that weighs the alternatives

School asks: "You give well-founded advice in which you weigh alternatives, identify risks and make a substantiated recommendation."

1. **The proposal per choice.** Four choices, each with two options in a table and a source: frontend folders by kind or per feature (React, n.d.), snake_case or PascalCase in the database (PostgreSQL 16 documentation on identifiers), plural nouns or verbs in server addresses (Microsoft, Web API design best practices), Conventional Commits or free text (Conventional Commits 1.0.0). Every source was read on 8 October 2026, and every quote it is based on was checked on the page itself.
2. **The risk of each recommendation.** Folders by kind can get crowded past sprint 3, written in Consequences with the way out (a new decision file). snake_case costs one package. The other two have no real cost for this team.
3. **Agreed by.** Merge request !42, branch `docs/1-sprint1-checklist-fixes`, records that the whole team agreed `001-codeConventions.md` at the sprint 1 review on 9 October 2026, with no objection to point 5. All five rows of Agreed by carry that date, and the Decision now says why each option that lost, lost.
4. **What the team chose and where it differed from the advice.**
   - TODO Wesley: after the team moment, write what the team chose per point and where it differed, with the reason.

## AA5 Adapting the message to who reads it

School asks: "You adapt your communication to different target groups in form and language."

1. **For the client.** The Context of the decision file opens with why this matters to him: someone outside the team keeps Connect after January 2027. No GitLab words in that paragraph.
2. **For the team.** The options are tables with one row per thing a developer notices (where a new file goes, typing SQL by hand), and the exact rule numbers of `docs/team/codeConventions.md`.
3. **Left out on purpose.** The split of the server into layers, because that is #12 and a second place for the same choice would drift apart.
4. **The question that showed a first explanation did not land.**
   - TODO Wesley: after the team moment, write the question and what changed in the file.

## Merge request !59: the tables of ASP.NET Identity keep their own names

Branch `docs/7-identity-table-names`. Files: `docs/decisions/001-codeConventions.md`, `docs/team/codeConventions.md` and `docs/architecture/architecture.md`.

### MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **The code and the documents differed.** The code of !36 (#261) creates the tables of ASP.NET Identity with the names Identity gives them, such as `AspNetUsers` and `AspNetRoles`, with snake_case columns and keys. Decision 001 and the code conventions said that every name in the database is snake_case.
2. **The documents follow the code.** We do it the way Nick Hoebe built it, and fix it later if it turns out to be a problem (my decision of 14 October 2026). On 19 October 2026 I set it for the whole project: Nick's code is leading, and where it differs from a document, the document follows. So the exception is not a new choice that reopens decision 001. Decision 001 names the exception under Consequences, and so do database rule 1 of the code conventions and the short list of database rules in the architecture.
3. **Told where the code was made.** My comment on !36 names the documents I updated.

- TODO Wesley: after the merge, add !59 and MS4 to the row of this file in `docs/wesley/README.md`.

## TODO

- [x] Whole team: agreed the four choices and point 5 at the sprint 1 review on 9 October 2026, all five dates in Agreed by.
- [x] Whole team: each confirms in a comment on #1 that they can follow the rules. On 9 October 2026 every teammate told Wesley that it is good, written down in #7.
- [ ] Wesley: fill in AA4 point 4 and AA5 point 4 after the team moment.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

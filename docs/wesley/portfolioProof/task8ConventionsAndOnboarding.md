# Proof: task 1.7 The conventions with examples and the getting started guide (#8)

Branch `docs/8-conventions-examples-and-onboarding`. Issue: 01 The repository, the pipeline and the team rules (#1). Files: `docs/team/codeConventions.md` and `docs/team/onboarding.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating, MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The handover risk.** A newcomer who needs the team to get started. The guide now says after each command in parts 1 and 2 what you should see, has the real clone address instead of a placeholder, and in part 7 says what to do with a red pipeline.
2. **Tool or reviewer.** Every rule of the conventions now ends in (tool: name) or (reviewer). A team that changes keeps the tool checks without anyone remembering them, and the (reviewer) marks show what a new reviewer must look at by hand.
3. **The walkthrough.**
   - TODO Wesley: plan the timed walkthrough with a teammate who did not write the guide, with a date in #1.
   - TODO teammate: follow the guide alone, and write in #1 your name, the date, the minutes and every question.
   - TODO Wesley: fix every question in the guide and write here what changed.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Which tool enforces which rule.** TypeScript, ESLint and Prettier from #263, dotnet build and dotnet format from #259, the pipeline jobs of #266, env-file-check and branch-flow of #5. Until a tool exists, the reviewer checks the rule.
2. **Left out on purpose, to avoid clashing with open merge requests.** Rule 2 of For all code got its mark in !23, backend rules 1 and 8 in !27. Audit rule 14 is changed by !21 of a teammate and gets its mark after that merge. Parts 3, 5 and 6 of the guide are left to the open merge requests !21 (the database) and !22 (the server).
3. **The decision of #7.** The conventions say what `docs/decisions/001-codeConventions.md` proposes. Once the team agrees, any difference is changed here.
   - TODO teammate who did not write the guide: write in the review what you think of the examples.

## TODO

- [ ] Wesley: plan the walkthrough in #1.
- [ ] Teammate: do the walkthrough and write the result in #1.
- [ ] Wesley: add the mark to audit rule 14 after !21 is merged.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

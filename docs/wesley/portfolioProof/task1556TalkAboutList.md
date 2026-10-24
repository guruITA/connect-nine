# Proof: task 1.16 One list of everything the team still has to talk about (#1556)

Merge request !101, branch `docs/1556-talk-about-list`. Issue: the code conventions give one way for the whole team (#1549). Files: `docs/meetings/talkAbout.md` and `docs/README.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **Points that were waiting in many places.** On 24 October 2026 the points the team still had to talk over stood in the TODO parts of my merge requests, in two decision files with empty Agreed by lines, in comments on cards, in the open items register and in my own notes. A point that only one of us knows about is not raised at the stand up, and then a card waits without anyone seeing why. The list puts them in one place, each with who it is for and where it comes from.
2. **The risk I put first.** Task 4.2 of log in (#269) still checks the password through `IAppDbContext` and the hasher of #262, while decision 007 and the part Log in and sessions put that check behind a small interface. Log in is built in sprint 2, so if nobody raises this before Nick starts it, the code and the rules go two ways and one of them has to be undone. It is the first point for Nick.
3. **Checked, not copied.** Every difference between the code and the rules was checked against the code on dev on 24 October 2026, with the file, the line where it matters and the rule. The check also showed what had been settled since I counted on 21 October 2026: the log tests already use substitutes of NSubstitute, and six of the seven `!` after a value are gone. Those moved to Settled instead of staying on the list.
4. **Transferable.** The list says how to use it: a point is added with who and where, and a settled point moves to Settled with the place its outcome is written. The open items register and the client's questions stay the registers they are, and the list takes from them only what is due at the next meeting, so nothing is kept in two places.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Named without blame.** Each difference names whose code it is in, the rule it meets and the merge request where the rule came from, and that older code follows a rule once its file changes, so none of it blocks a merge. Rule 12 asks the team to choose one way per point, which is a choice for all of us at the stand up, not something I decide in someone else's file.
2. **What I take on myself.** The two names in `accounts.ts` are in Nick's file. I said I would bring them to one name, and the list says that I do it with his yes.
   - TODO the team: use the list at the stand up, and move what is settled to Settled with the date and where the outcome is written.

## TODO

- [ ] The team: use the list at the stand up, and move what is settled to Settled.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

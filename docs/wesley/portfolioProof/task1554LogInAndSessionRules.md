# Proof: task 1.15 The code conventions for log in and sessions (#1554)

Merge request !98, branch `docs/1552-session-table-beside-identity`. Issue: the code conventions give one way for the whole team (#1549). Files: `docs/team/codeConventions.md`, part Log in and sessions, and a dated note in `docs/wesley/securityAndArchitecture.md`, `docs/wesley/securityAndArchitectureEvidence.md` and `docs/wesley/securityFundamentals.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **Rules before the code.** Nothing logs a member in on dev yet. Decision 009, which Nick Hoebe and I picked on 22 October 2026, says how a log in works, and the five rules turn it into what every card that logs a member in or out must do. Written now, they are in place before #15 is built, so no second way has to be taken out again later.
2. **The risks each rule prevents.** A second way to hash or check a password next to Identity (rule 1). A second log in cookie, so one part of Connect trusts a log in another part already ended (rule 2). A token readable in the database, a backup or a log (rule 3). A log out that only deletes the cookie, and a rule for an active session written differently in the guard and in the count of #197 (rule 4). A member who acts as another member by changing an account id in a request (rule 5).
3. **The reason next to every rule.** Each rule says why, with decision 009, decision 007, a task of #15 or the OWASP Session Management Cheat Sheet as its source, so a developer after the team can follow it without asking. Every quotation from OWASP is in the cheat sheet as written.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **The rules follow the tasks Nick Hoebe builds.** Each rule names the task of #15 it comes from: `SessionToken` of task 4.2 (#269), `Session.IsActive(now)` and the cascade of task 4.1 (#268), `SessionAuthenticationHandler` and `User.GetAccountId()` of task 4.3 (#270). Rule 1 applies decision 007, which Nick drafted, to the password check at log in. That is the one place where the rules differ from the text of a task: task 4.2 (#269) says the log in goes through `IAppDbContext` and the password hasher of #262, and rule 1 puts the check of the password behind a small interface of Application, with the session row still through `IAppDbContext`. Decision 009 says the same, and my comment on #15 of 23 October 2026 told Nick.
2. **Its own numbers.** The part is numbered on its own after Tests and mocking, like that part, so it does not clash with backend rules 17 to 20 of !79, which is not merged yet.
3. **My own documents follow the decision.** My sprint 1 advice was cookie authentication through Identity. My architecture document, its summary for the teacher and my security document now each carry a dated note that points to decision 009, and the sprint 1 text stays as it was handed in.
4. **What a teammate's review changed.** Gurpreet Singh checked every quotation against the Microsoft and OWASP pages in his review of !98 on 24 October 2026 and found two places where a rule could be read wrongly. Rule 2 could be read as no cookie at all, while the log in with Google of #46 needs a short cookie for the handshake, so rule 2 now says that cookie exists only for that step and never says who is logged in, with Microsoft's page on the sign-in scheme as its source. Rule 5 could be read as no id in any address, which would forbid the page of another member (#30), so it now says the id of the caller. Both were written in through !100, after !98 was merged on his approval.
   - TODO Nick Hoebe: read the part Log in and sessions and write in !98 what is unclear or different from how you build #15, the password check of task 4.2 first.

## TODO

- [ ] Nick Hoebe: read the part Log in and sessions and write in !98 what is unclear or different, the password check of task 4.2 (#269) first.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

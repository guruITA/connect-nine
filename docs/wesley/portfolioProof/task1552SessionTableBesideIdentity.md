# Proof: task 11.5 The choice for a table session beside ASP.NET Core Identity (#1552)

Merge request !98, branch `docs/1552-session-table-beside-identity`. Issue: 11 One clear structure for the server (#12). Files: `docs/decisions/009-sessionTableBesideIdentity.md` and `docs/architecture/architecture.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Analysing and Advising AA4 and AA5.

A point that needs someone else is marked TODO with who and what.

## AA4 Giving advice that weighs alternatives and risks

School asks: "You give well-founded advice in which you weigh alternatives, identify risks and make a substantiated recommendation."

1. **Two ways, held against the cards.** The cookie of ASP.NET Core Identity alone, and a table session beside Identity. For each, the decision shows what happens at log in and on every next request, and one table holds both against five cards: log out (#18) in Sprint 2, a new password logs out the other devices (#45) in Sprint 3, #196 and #197 if the client says yes to question 8, and #202 if the client says yes to question 14.
2. **Why the cookie alone lost.** Log out (#18), a must in Sprint 2, asks that a copy of the log in stops working at once. Microsoft writes that "the cookie is the single source of identity", and the security stamp ends every cookie of an account at once, never one. OWASP asks that log out ends the session at least on the server. The table ends one log in on the server and keeps the other log ins of the member, and it can count them (#197). Where the cookie alone is enough, ending every log in at once (#45, #202), the decision says so.
3. **The risks of the choice.** One read of the database on every request with a cookie, on the unique index of task 4.1 (#268). A token that must stay out of the database and the logs, so only its hash is stored. A second hash beside the hash of the password, which the decision keeps apart: the first is one log in on one device, the second stays with Identity.
4. **What it means for the code that exists.** Nick Hoebe's `AddIdentityCore<Account>()` switches on no cookie of Identity, so nothing has to be removed. Following decision 007, the check of the password at log in is Identity work behind a small interface of Application, and the session row goes through `IAppDbContext`. The foreign key `account_id` points to `AspNetUsers`.
5. **Picked with the one who builds it.** Nick Hoebe and I picked this way in our talk of 22 October 2026, before he started task 4.1 (#268). He confirmed the written decision under Agreed by on 23 October 2026, in commit 35aa34c7 on this branch.

## AA5 Adapting your communication to each audience

School asks: "You adapt your communication to different target groups in form and language."

1. **For the client.** The part In plain words: one member on a laptop in the school library and on her phone, what happens when she logs out and when she loses the phone. No table, column or card name in it.
2. **For the team.** The two flows step by step, the rows of the table with `account_id`, `token_hash`, `expires_at` and `revoked_at`, the rule for an active session as C#, and the task behind every card.
3. **Left out on purpose.** How the log in and the guard are built. That is in the tasks of #15, which Nick builds, and a second place for the same detail would drift apart.

## TODO

- [x] Nick Hoebe: a line under Agreed by in `docs/decisions/009-sessionTableBesideIdentity.md`, 23 October 2026 (35aa34c7).
- [x] Gurpreet Singh: a line under Agreed by, 24 October 2026, asked for in his review of !98 and written in through !100.
- [ ] Bayon Eerhart and Kelvin van Voorst: a line under Agreed by.
- [ ] Wesley: write in #1552 when and where the rest of the team agreed.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

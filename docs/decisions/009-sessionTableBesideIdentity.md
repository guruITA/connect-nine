# 009 How Connect remembers a log in: a table session beside ASP.NET Core Identity

**Status.** Picked by Wesley Oerlemans and Nick Hoebe in their talk of 22 October 2026, and written down by Wesley Oerlemans on 23 October 2026. Nick Hoebe confirmed it under Agreed by on 23 October 2026. The rest of the team confirms it at the stand up, and each member adds a line under Agreed by.
**Issue.** 04 Log in with email and password (#15), task 4.1 (#268) to task 4.3 (#270), and the cards that end a session: 05 Log out (#18), 22b A new password logs me out on my other devices (#45), 117 Log out on every device at once (#197), 118 Log out after a while of doing nothing (#196) and 124 Logged out everywhere when log ins are locked (#202). Written in task 11.5 (#1552).
**Related decision.** [007 Identity is an exception to the IAppDbContext rule](007-identityAndIAppDbContext.md).
**Who decides.** Wesley Oerlemans and Nick Hoebe, who builds #15. The whole team confirms.

## Context

ASP.NET Core Identity already manages the account of a member: users, passwords, profile data, roles, claims, tokens and email confirmation (Microsoft, n.d.-b). Connect uses it through `AddIdentityCore<Account>()` in `api/Connect.Infrastructure/DependencyInjection.cs`, with the accounts in `AspNetUsers` (decision 007). Nothing logs a member in yet: that is #15.

The question is not who the member is. It is how the server remembers that a member is logged in after the password was checked. There are two ways.

### Without a table session: the cookie of Identity

```text
A member sends email and password
→ Identity checks the password
→ Identity makes a ClaimsPrincipal
→ ASP.NET Core protects the claims and puts them in an authentication cookie
→ the browser sends the cookie with every next request
→ ASP.NET Core checks the cookie and builds HttpContext.User from the claims again
```

The claims, for example the account id and the role, are inside the protected cookie. The database holds the account and the hash of the password, but no row per browser. A member logged in on a laptop and on a phone has two cookies, and the two cookies are the two log ins. Microsoft says it plainly: "Once a cookie is created, the cookie is the single source of identity" (Microsoft, n.d.-c, React to back-end changes section). The hash of the password is only used for the check at log in. It never goes to the browser and is not the cookie.

Logging out deletes the cookie in that one browser. To make cookies that already exist stop working, Identity has the security stamp: `UserManager.UpdateSecurityStampAsync(user)` makes existing cookies invalid "the next time they are checked", and how often that check runs is a setting (Microsoft, n.d.-a, ISecurityStampValidator and SignOut everywhere section). It works on every cookie of the account at once, never on one phone or laptop.

### With a table session beside Identity

Identity still owns the account and the check of the password. Connect adds one row for every log in:

```text
A member sends email and password
→ Identity checks the password
→ Connect makes a random token
→ Connect calculates the SHA-256 hash of the token
→ Connect stores the hash, the account id, the time it was made and when it ends
→ Connect sends the token itself in the cookie connect_session
→ the browser sends the cookie with every next request
→ Connect hashes the token it receives and looks for the row with that hash
→ Connect checks that the session is active
→ Connect makes the ClaimsPrincipal for the account of that row
```

A member logged in on a laptop and on a phone has two rows:

```text
account_id   token_hash   expires_at   revoked_at
123          hash(A)      tomorrow     empty
123          hash(B)      tomorrow     empty
```

The browser holds the token, the database only its hash. The hash alone says nothing about the member. The column `account_id` links the session to the account of Identity. The hash of the token and the hash of the password are two different things: the first tracks one log in on one phone or laptop, the second stays with Identity.

A session is active when:

```csharp
RevokedAt is null && now < ExpiresAt
```

So a session stops being active when it is ended, or at the exact moment of `expires_at` (task 4.1, #268). The claims can still come from Identity. The table only decides whether this one log in still counts.

## What the cards ask

| Card | What it asks | The cookie of Identity alone | A table session beside Identity |
| --- | --- | --- | --- |
| 05 Log out (#18), Sprint 2, must | "a copy of their log in stops working at once" | No. Logging out deletes the cookie in that browser, and a copy of it keeps working until it ends. The security stamp would stop the copy, but it logs out every other device of the member too | Yes. `revoked_at` is filled in for this one log in, and a copy is refused at its next request |
| 22b A new password logs me out on my other devices (#45), Sprint 3 | Every device of the account logged out, the browser used for the change too | Yes, with the security stamp, at the next check of each cookie | Yes. Every active row of the account gets `revoked_at`, in one transaction with the new password |
| 117 Log out on every device at once (#197) | "Connect says on how many devices they are logged in" | No. There is nothing to count | Yes. The active rows of the account are counted |
| 118 Log out after a while of doing nothing (#196) | Logged out after a while without use | Partly. `SlidingExpiration` renews the cookie while it is used, and the fixed end one day after log in (#15) then needs a check of its own | Yes, with a last active time per row (#1212) |
| 124 Logged out everywhere when log ins are locked (#202) | Every device logged out at the lock | Yes, with the security stamp, the same as 22b | Yes. The lock and the end of every row in one transaction (#1243) |

#196 and #197 are only built when the client says yes to question 8 in #11, and #202 only when the client says yes to question 14. #18 is a must in Sprint 2 and #45 is planned in Sprint 3 either way. So the cookie of Identity alone can end every log in of a member at once, but not one log in on its own (#18), and it cannot count them (#197).

## Decision

A table session beside ASP.NET Core Identity, as task 4.1 (#268) to task 4.3 (#270) of #15 already plan. Picked by Wesley Oerlemans and Nick Hoebe on 22 October 2026.

Identity stays the identity system of Connect: the account in `AspNetUsers`, the hash of the password, the check of the password, roles and claims. The table session adds only what the cookie of Identity alone does not have: one row per log in, with the hash of the token, the account it belongs to, when it ends and when it was ended. The cookie `connect_session` holds the token and is the one log in cookie of Connect (task 4.3, #270). The table does not replace Identity.

Why the other way lost:

- **The cookie of Identity alone.** It is less code, and it is enough for a log in, and for ending every log in of a member at once with the security stamp. But log out (#18) asks that a copy of the log in stops working at once, and OWASP asks that log out ends the session "at least on server side" (OWASP Foundation, n.d., Logout Button section). To end one log in on the server and keep the others, the server has to remember each log in, and that is what the table does. Counting the devices of a member (#197) needs the same list.

## In plain words

Alice logs in on a laptop in the school library and on her phone. Connect writes down two log ins, one for each. When she logs out in the library, Connect marks that one log in as ended, so even someone who copied it is out, and her phone stays logged in. When she loses her phone, Log out everywhere ends both. Without the list, logging out would only delete the key on the library laptop, and a copy of it would still work. Connect could only stop all her keys at once.

## Consequences

- **One read of the database on every request with a cookie.** The guard of task 4.3 (#270) looks up the hash of the cookie through the unique index `ux_session_token_hash` of task 4.1 (#268). A request without a cookie, or with an empty or too long one, causes no read.
- **The token stays out of the database and the logs.** Only its hash is stored, a log line never holds it (issue 17 A health check and structured logs), and the cookie has the flags Secure, HttpOnly and SameSite=Lax.
- **Identity keeps the password, Connect keeps the sessions.** Following decision 007, the check of the password at log in (task 4.2, #269) is Identity work behind a small interface of Application. The session row is Connect's own data and goes through `IAppDbContext`.
- **`account_id` points to the account of Identity.** Task 4.1 (#268) says the column points to account. The accounts are in `AspNetUsers`, the table of Identity (database rule 2 of `docs/team/codeConventions.md`), so the foreign key points there.
- **When a card is dropped.** If the client says no to question 8, #196 and #197 are not built, and if the client says no to question 14, #202 is not built. The table stays either way, because #18 needs to end one log in on its own.

## Agreed by

| Name | Date agreed |
|---|---|
| Bayon Eerhart | |
| Nick Hoebe | 23 October 2026 |
| Wesley Oerlemans | 23 October 2026 |
| Gurpreet Singh | |
| Kelvin van Voorst | |

## References

Microsoft. (n.d.-a). *Configure ASP.NET Core Identity*. Microsoft Learn. Retrieved October 23, 2026, from https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-10.0

Microsoft. (n.d.-b). *Introduction to Identity on ASP.NET Core*. Microsoft Learn. Retrieved October 23, 2026, from https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-10.0

Microsoft. (n.d.-c). *Use cookie authentication without ASP.NET Core Identity*. Microsoft Learn. Retrieved October 23, 2026, from https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-10.0

OWASP Foundation. (n.d.). *Session management cheat sheet*. OWASP Cheat Sheet Series. Retrieved October 23, 2026, from https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

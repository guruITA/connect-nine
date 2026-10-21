# Proof: tasks 4b.1 and 4b.2 The test member, added at start outside Production (#276, #277)

Merge request !74, branch `feature/276-test-member`. Issue: 04b A member page asks me to log in first and brings me back (#16). Files: `api/Connect.Infrastructure/Persistence/TestDataSeeder.cs` and `TestDataSeedingExtensions.cs`, the call in `api/Connect/Program.cs`, `appsettings.Development.json`, `deploy/.env.example`, their tests, `docs/testing/testData.md` and the part Test member of `docs/team/onboarding.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Designing and Realising OR1, OR2, OR4 and OR5.

A point that needs someone else is marked TODO with who and what.

## OR1 Designing and building to a plan

School asks: "You design and implement systematically, justify choices in relation to alternatives, and adapt based on new insights."

1. **What the seed adds.** One made up account, `test-member@example.org`, with a confirmed email address and no profile, so a later check can log in as a known member. It is added once and never changed or deleted after that, also when the password setting changes.
2. **The alternatives I weighed.** Seed data in a migration (`HasData`) lost, because the hash of the password would then sit in Git and the live site would get the member too. A SQL script lost, because it cannot make the hash the way log in will check it, and someone has to remember to run it. A class that runs at every start won, behind two guards: the setting `Seeding:TestData` and never in Production.
3. **Following the code of the team.** The account is created through `UserManager<Account>` of ASP.NET Core Identity, which Nick Hoebe's account code already registers. That way the normalised email address and the hash are exactly what log in will check later.
4. **What the test taught me.** Without the lookup of the email address first, a second start did not make a second account, because Identity refuses a user name that is taken. It did log a warning at every start. So the test checks that a second start stays silent, and not only that there is one row.

## OR2 Choosing standards and methods

School asks: "You independently select relevant standards and methods from the field and substantiate why they fit the context."

1. **The password hasher of the framework.** The hash comes from the password hasher of ASP.NET Core Identity (PBKDF2), never from my own code, and the password itself is never stored or logged.
2. **Secrets outside Git.** On a laptop the password is a .NET user secret (Microsoft, Safe storage of app secrets in development), on the test site an environment variable (`Seeding__TestMemberPassword`) that comes from a masked GitLab variable once the deploy of #267 exists.
3. **Environments.** The seed runs only when the environment is not Production. The live site never gets the setting, and even when it is set there, the server logs "Test data is never added in Production" and adds nothing.

## OR4 Validating the solution

School asks: "You systematically validate that the solution does what it should, and draw conclusions for improvement."

1. **The tests.** `TestDataSeederTests` on PostgreSQL: one run gives one account, two runs still one and the second is silent, the stored hash is not the password and the hasher accepts it, an empty setting adds nothing, an existing account is never changed. `TestDataSeedingTests` on the whole server: on gives the account once with a password that verifies, off gives none, Production with the setting on gives none, and with a database that cannot be reached the server starts and `GET /api/health` answers.
2. **Tests broken on purpose.** Without the lookup of the email address, `SeedAsync_TwoRuns_StillOneAccountAndTheSecondRunIsSilent` failed. Without the guard for Production, `Start_Production_AddsNoAccountEvenWithTheSettingOn` failed. Both passed again once the code was back.
3. **Two starts by hand.** The server started twice in Development with a made up password, and the database held one test member:

```
          email          | email_confirmed | has_hash 
-------------------------+-----------------+----------
 test-member@example.org | t               | t
(1 row)
```

4. **Not covered yet.** Logging in as the test member (204) waits for log in (#15). Writing the password to the test site waits for the deploy of #267.

## OR5 Impact on users and society

School asks: "In your design and implementation, you consciously take into account the broader impact on end users and society, while forming your own ethical judgment."

1. **Never a real person and never live.** The test member is made up and has no profile, so no data of a young person is ever made up in its name, and the live site cannot get it.
- TODO Wesley: where the team keeps the password of the test member, so every member can use the same one.

## TODO

- [ ] Wesley: the OR5 line above, once the team agreed where the password is kept.
- [ ] Wesley: after the merge, tick the boxes of #276 and #277 with the evidence in a comment.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

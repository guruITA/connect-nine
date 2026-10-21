# Proof: task 7.1 The profile table (#320)

Merge request !67, branch `feature/320-profile-table`. Issue: 07 Create a profile (#25). Files: `api/Connect.Domain/Users/Profile.cs`, `api/Connect.Domain/Users/AgeBand.cs`, `api/Connect.Infrastructure/Persistence/ConnectDbContext.cs`, the migration `AddMemberDetailsToProfile`, `api/Connect.Infrastructure.Tests/Persistence/ProfileDatabaseTests.cs` and `docs/architecture/architecture.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Designing and Realising OR1 and OR4.

A point that needs someone else is marked TODO with who and what.

## OR1 Designing and building to a plan

School asks: "You design and implement systematically, justify choices in relation to alternatives, and adapt based on new insights."

1. **The columns and why.** `first_name` is how other members see you. `age_band` puts you in one of three bands, 15 to 17, 18 to 21 or 22 to 30, without an exact age. `region` is your town or area. `bio` is an optional line about yourself. `id`, `account_id`, `created_at` and `updated_at` were already in the table.
2. **Left out on purpose.** No birth date, exact age, school, street address, phone number or email address. Many members are 15 to 17, and issue 07 says Connect never asks for these (the privacy rules of #24). The email address is already on the account.
3. **The option I did not choose.** Task 7.1 asked for a new table `member_profile` linked to the account. Nick Hoebe's account code already has a table `profile` with one row per account, and the team agreed on the way Nick builds the .NET code, so I put the details in his table and kept his names and his way of configuring it. A second table would have meant two places for one profile. I also did not add a birth date, because Connect would then know the exact age the issue says it never asks for.
4. **The rules in the database.** The age band is stored as the name of the enum, such as `YoungAdult`, which is how the code conventions store an enum. The check `ck_profile_age_band` refuses any other text, so a wrong value cannot get in through SQL typed by hand either.
5. **What I found after the first migration.** EF Core gives the new text columns a default of an empty text. On a table that already holds profiles the checks would then refuse those rows. No code saves a profile yet, so the table is empty everywhere and the migration applies. The migration stays as the command made it.

## OR4 Validating the solution (#320)

School asks: "You systematically validate that the solution does what it should, and draw conclusions for improvement."

1. **Up, back and up again.** On an empty database the migration applied, went back to `AddAccountTable` and applied again. The output is in the merge request.
2. **A refused insert.** An insert with the age band `From10To14` was refused by `ck_profile_age_band`.
3. **The database tests.** Eight tests in `ProfileDatabaseTests`: a saved profile leaves one row, a second profile for the same account fails on `ix_profile_account_id` (23505), two saves at the same moment leave one row, the age band is stored as the text `YoungAdult`, an unknown age band, an empty first name and an empty region fail on their checks, and a profile without a bio saves. All 16 tests of the backend pass.
4. **The red run.** With `ck_profile_age_band` dropped from the test database, `InsertingUnknownAgeBand_FailsOnAgeBandCheck` failed, so the test really watches the check.
5. **What the table does not check yet.** A street address typed in the region (#26), and the limits of 60, 60 and 280 characters, which the validator of task 7.2 checks. That is fine for now, because no screen or address saves a profile before task 7.2.

- TODO a reviewer: apply the migration on your own machine and write in a comment on !67 what you saw.

## TODO

- [ ] Wesley: after the merge, tick the boxes of #320 with the evidence in a comment.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

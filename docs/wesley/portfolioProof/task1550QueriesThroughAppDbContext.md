# Proof: task 3.13 The code convention on queries and saves through IAppDbContext (#1550)

Merge request !68, branch `feature/321-create-profile-address`. Issue: my profile on the merged account code (#1542). Files: `docs/team/codeConventions.md`, backend rule 7, `api/Connect.Application/Common/Abstractions/IAppDbContext.cs`, `api/Connect.Infrastructure/Persistence/ConnectDbContext.cs`, `api/Connect.Application/Profiles/CreateProfile/CreateProfileCommandHandler.cs` with its tests, and `api/Connect.Infrastructure.Tests/Persistence/AppDbContextDatabaseTests.cs`.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The build that failed.** Since !50, `Connect.Application` references only Domain and the DI abstractions. Built on top of it, my `CreateProfileCommandHandler` failed with `error CS0234: The type or namespace name 'EntityFrameworkCore' does not exist in the namespace 'Microsoft'` and `error CS0246: The type or namespace name 'Npgsql' could not be found`. The handlers of !70 to !76 use the same kind of calls: `SingleOrDefaultAsync`, `SingleAsync`, `AnyAsync`, `CountAsync`, `ToListAsync` and `AsNoTracking`, and `AddProfileTagCommandHandler` catches the PostgreSQL error 23505 twice.
2. **One way for every handler.** Without a rule, each handler would find its own way around the missing EF Core. Rule 7 now says how: a query goes through `IAppDbContext` with a method named after the EF Core one, added when a handler first needs it, and a save that can meet a unique index uses `SaveChangesOrConflictAsync`. This merge request adds the three that `CreateProfileCommandHandler` uses: `SingleOrDefaultAsync`, `AnyAsync` and `SaveChangesOrConflictAsync`.
3. **A conflict is a result, any other error is not.** Decision 005 says: "The database adapter may translate a known database constraint into an application `Error.Conflict`; an unknown database exception remains an exception." So `ConnectDbContext` answers `Error.Conflict` only for a unique violation (23505), and every other refusal of the database still throws.
4. **Measured on PostgreSQL 16.** Seven tests in `AppDbContextDatabaseTests`: a new profile is saved, a second profile for the same account answers `Error.Conflict` and leaves one row, an empty first name refused by `ck_profile_first_name_not_empty` stays a `DbUpdateException`, and `SingleOrDefaultAsync` and `AnyAsync` each find a row and find none.
5. **The tests that turn red.** With the translation switched off, `Post_TwoProfilesAtTheSameMoment_GivesOneCreatedAndOneConflict` failed in 8 runs out of 8, the second save answering 503 instead of 409, and `SaveChangesOrConflictAsync_SecondProfileForSameAccount_ReturnsConflict` failed as well. With the translation back, all 152 tests are green, each test project alone on a database of its own with only `TestsConnection` set.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **The team's decision leads.** Decision 007 keeps EF Core out of Application and names `IAppDbContext` as the contract for the tables that are not Identity's, `Profile` included. So the query goes through `IAppDbContext`, not through a second interface per use case, which the decision says it "does not justify", and EF Core does not go back into Application.
2. **Nick's code stays as it is.** `IAppDbContext` and `ConnectDbContext` are Nick Hoebe's. I only added members, in the shape he uses for `Add` and `Remove`, and changed none of his. His `SaveChangesAsync` and the times it sets run as before, because `SaveChangesOrConflictAsync` calls it. His tests ran without an edit and stayed green, also on dev with this merge request merged in.
3. **The way that was already there.** On dev, Nick's `AccountRegistrationService` in Infrastructure already turns a unique violation into `Error.Conflict`. `SaveChangesOrConflictAsync` does the same for the tables behind `IAppDbContext`, so there is one way and not two.
4. **The ways that lost.** Synchronous LINQ on `IQueryable<T>` needs nothing new, but a second save at the same moment would throw and answer 503 instead of 409, as the red run above showed. A small interface per use case is option 2 of the future improvement 002, which the team keeps for a future team. EF Core back in Application goes against decision 007.

## TODO

- [ ] A teammate who did not write the rule: read rule 7 in the review and write what was unclear.
- [ ] Wesley: after the merge, tick the boxes of #1550 with the evidence in a comment.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

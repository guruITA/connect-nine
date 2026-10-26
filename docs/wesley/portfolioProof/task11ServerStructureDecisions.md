# Proof: tasks 11.1, 11.2 and 11.3 The structure and the version of the server (#253, #254, #255)

Branch `docs/12-server-structure-decisions`. Issue: 11 The team agrees on the layered architecture (#12). Files: `docs/decisions/002-layeredArchitecture.md` and `docs/decisions/003-dotnet10.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Analysing and Advising, AA2, AA3, AA4 and AA5, and for !58 also Managing and Collaborating, MS4.

A point that needs someone else is marked TODO with who and what.

## AA2 Questioning sources and my own assumptions

School asks: "You show an inquisitive and critical attitude by questioning sources, AI results, the problem statement and your own assumptions."

1. **CQRS makes testing easier.** The team assumed this. The EF Core team's own page on choosing a testing strategy advises the real database first, a repository second, and calls SQLite a fake whose results are not guaranteed to match (Microsoft, n.d.-b). The decision file now says that testing is no reason for CQRS, and that rules which depend on PostgreSQL get a database test against a real PostgreSQL.
2. **CQRS fits Connect.** Fowler writes that "for most systems CQRS adds risky complexity" and Microsoft that it "might not be suitable when the domain or the business rules are simple". Checked further: both warnings are mostly about the heavy form with a separate read database and messaging. Microsoft's simplified CQRS (one database, two logical models) and Bogard's vertical slices describe the light form Connect uses. The risk that stays is the one Bogard names: a team that must recognise a handler that does too much. All four sources are in the decision file, per option, with what each option is for.
3. **The structure of rule 8.** Rule 8 of the code conventions said Infrastructure uses only Domain. The code on dev, merge request !22 and task 3.1 (#259) have Infrastructure using Application and a start project Connect, and Microsoft's Clean Architecture says Infrastructure "should have a reference to the Application Core project" (Microsoft, n.d.-a). On 8 October 2026 I decided the code leads, and rule 8, the test projects, the solution file and the migration command now follow the code.
4. **The support dates.** The code conventions said .NET 10 is supported "until November 2028" and .NET 8 stops on 10 November 2026. The Microsoft page, read on 8 October 2026, says 14 November 2028 and 10 November 2026, and adds that .NET 9 stops on the same day as .NET 8. Rule 1 now has the exact date and names .NET 9.

## AA3 Weighing the issue from several sides

School asks: "You analyze issues from multiple perspectives and weigh the societal and ethical consequences."

Consequences in `002-layeredArchitecture.md` are split per party: the team new to .NET, the maintainer after January 2027 and the foundation. The heaviest is a platform that is switched off because nobody can change it, and that is why a structure the compiler checks won over one only a reviewer checks.

## AA4 Giving advice that weighs alternatives and risks

School asks: "You give well-founded advice in which you weigh alternatives, identify risks and make a substantiated recommendation."

1. **Two tables with five rows each.** Files for one new feature, what a beginner must learn, how a rule is unit tested without the database, what catches a mistake and the source. Every claim names a Microsoft page or is marked as an opinion.
2. **The compiler proof.** Run on 8 October 2026 with the .NET SDK 10.0.401 on a throwaway copy: Domain gives CS0234 on `Connect.Infrastructure.Persistence.ConnectDbContext`, the project Connect builds the same line with 0 errors. Pasted in the decision file.
3. **The .NET versions with their risk.** .NET 8 and .NET 9 run out on 10 November 2026, .NET 10 is newer with fewer examples for beginners. Next upgrade planned no later than May 2028.
4. **Agreed by.** Merge request !42, branch `docs/1-sprint1-checklist-fixes`, fills the Agreed by table of `002-layeredArchitecture.md` for all five of us with 1 October 2026, the day the whole team decided the structure and CQRS. The whole team agreed `003-dotnet10.md` at the sprint 1 review on 9 October 2026, and all five rows carry that date. My own line of `dotnet --list-sdks` is in it: `10.0.401`. The lines of Bayon Eerhart, Nick Hoebe, Gurpreet Singh and Kelvin van Voorst are still open.

## AA5 Adapting the communication to each audience

School asks: "You adapt your communication to different target groups in form and language."

1. **For the client.** The Consequences of both files start with the foundation: until when Connect gets security fixes, and why a later developer can change it.
2. **For the team.** The exact project names, the direction of the references, the test line and where `net10.0` is set.
3. **A teammate new to .NET explains the four layers in their own words.**
   - TODO a teammate new to .NET: write a comment in #12 that explains the four layers in your own words.

## Merge request !58: decision 002 follows the code

Branch `docs/12-dbcontext-name-follows-code`. File: `docs/decisions/002-layeredArchitecture.md`, and the name of the class in four more documents.

### MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **The documents follow the name we agreed.** Nick Hoebe named the class `ConnectDbContext` in !11 and !36 (#261), while five documents said `AppDbContext`. We agreed on the way Nick built it, so the documents now say `ConnectDbContext`: `docs/decisions/002-layeredArchitecture.md`, `docs/architecture/architecture.md`, `docs/team/codeConventions.md`, `docs/testing/testingStrategy.md` and `docs/wesley/securityAndArchitecture.md`. The contract keeps its name, `IAppDbContext`, which #262 adds. My comment on !36 names the documents I updated.
2. **Where a Domain rule is tested.** The code of task 3.1 (#259) has no `Connect.Domain.Tests`. Decision 002 and the code conventions now say that a rule in Domain is tested without any database in `Connect.Application.Tests/Domain`.
3. **The folders follow the account code.** The account code of #262 (!47, merged with !50) puts a use case in `api/Connect.Application/Accounts/CreateAccount/` with `CreateAccountCommandHandler`, the contracts in `Common/Abstractions` and `IAppDbContext` with `IQueryable<T>`. Decision 002, the code conventions, the architecture and the testing strategy now say the same, and so do my own architecture document and its summary. My comment on !47 names the team documents I updated.
4. **Again after the merge of !50.** Nick's account code reached dev with !50 on 20 October 2026, after this merge request was approved, and it differs from what the documents said in four places. The handler no longer checks the address and hashes the password itself: it hands both to `IAccountRegistrationService`, which `AccountRegistrationService` in Infrastructure implements with `UserManager<Account>`. The result file is `CreateAccountResult.cs`, not `CreateAccountResponse.cs`. The account handler test uses a stand-in for that contract, not SQLite. And `Account` in Domain is built on `IdentityUser<Guid>`, so Domain uses that package. Decision 002, code conventions rules 7 and 16, the Definition of Done, the architecture and the testing strategy now say what the code does, and so does the paragraph on the account handler in the architecture, which still said it used `UserManager<Account>` in Application. I held every line against the code on dev, and named the documents in a comment on !50.

### AA2 Questioning sources and my own assumptions

School asks: "You show an inquisitive and critical attitude by questioning sources, AI results, the problem statement and your own assumptions."

1. **Quotes checked at their source.** The rows on what catches a mistake and where each option fits now quote Microsoft, with the source named, such as (Microsoft, n.d.-a). The data access row on what catches a mistake is marked as an opinion. I read the two Microsoft pages behind the new quotes again on 14 October 2026 and checked the quotes on them.

### AA4 Giving advice that weighs alternatives and risks

School asks: "You give well-founded advice in which you weigh alternatives, identify risks and make a substantiated recommendation."

1. **The missing row (#253).** The Structure table now has the row on how a rule is unit tested without the database.
2. **Why the options lost (#254).** Consequences say why one project with four folders and the repository pattern lost, each from its own table.
3. **My preference.** The part Preference holds my preference and why, because I drafted the file. The team decided 002 together on 1 October 2026.

- Done: !58 and MS4 are in the row of this file in `docs/wesley/README.md`, added in !83.

## Every claim of the Structure table has its source (#253, 21 October 2026)

### AA2 Questioning sources and my own assumptions

School asks: "You show an inquisitive and critical attitude by questioning sources, AI results, the problem statement and your own assumptions."

1. **Two rows without a source.** After the merge of !58 I held every cell of the two tables of decision 002 against the second box of #253: every claim names a Microsoft page or is marked as an opinion. Two rows of the Structure table did neither: files for one new feature, and what a beginner must learn.
2. **Quoted at the source.** I read Microsoft's page Common web application architectures again (Microsoft, n.d.-a) and quoted it for what it says: its reference application organises "its code into projects", "In a single project scenario, separation of concerns is achieved through the use of folders", and "Infrastructure should have a reference to the Application Core project".
3. **What the page does not say is marked.** The page does not say that project references are more to learn than folders. That part is my judgement, so both cells of that row now say it is an opinion.

## TODO

- [x] Whole team: decision 002, decided together on 1 October 2026, Agreed by filled in.
- [x] Whole team: `003-dotnet10.md` agreed at the sprint 1 review on 9 October 2026, written in #12.
- [ ] Bayon, Nick, Gurpreet and Kelvin: your line of `dotnet --list-sdks` in #12.
- [ ] Whole team: each writes the line of `dotnet --list-sdks` that starts with 10 in `003-dotnet10.md` and in #12.
- [ ] Owner of #259: link both decision files from `docs/architecture/architecture.md`.

## References

Bogard, J. (2018, April 19). *Vertical slice architecture*. Retrieved October 8, 2026, from https://www.jimmybogard.com/vertical-slice-architecture/

Fowler, M. (2011, July 14). *CQRS*. Retrieved October 8, 2026, from https://martinfowler.com/bliki/CQRS.html

Microsoft. (n.d.-a). *Common web application architectures*. Microsoft Learn. Retrieved October 8, 2026, from https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

Microsoft. (n.d.-b). *Choosing a testing strategy*. EF Core, Microsoft Learn. Retrieved October 8, 2026, from https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy

Microsoft. (n.d.-c). *CQRS pattern*. Azure Architecture Center, Microsoft Learn. Retrieved October 8, 2026, from https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

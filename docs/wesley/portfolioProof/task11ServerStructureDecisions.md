# Proof: tasks 11.1, 11.2 and 11.3 The structure and the version of the server (#253, #254, #255)

Branch `docs/12-server-structure-decisions`. Issue: 11 The team agrees on the layered architecture (#12). Files: `docs/decisions/002-layeredArchitecture.md` and `docs/decisions/003-dotnet10.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Analysing and Advising, AA2, AA3, AA4 and AA5.

A point that needs someone else is marked TODO with who and what.

## AA2 Questioning sources and my own assumptions

School asks: "You show an inquisitive and critical attitude by questioning sources, AI results, the problem statement and your own assumptions."

1. **CQRS makes testing easier.** The team assumed this. The EF Core documentation says the repository pattern is its recommended way to test without the production database, and SQLite in memory is the other way (Microsoft, n.d.-b). The decision file says so in the part What the sources changed: both ways test a rule without PostgreSQL, so testing is no reason for or against CQRS.
2. **CQRS fits Connect.** Microsoft writes that CQRS "might not be suitable when the domain or the business rules are simple" (Microsoft, n.d.-c). That row is in the table, next to the reason the team gives.
3. **The structure of rule 8.** Rule 8 of the code conventions says Infrastructure uses only Domain. The code on dev has Infrastructure using Application, and Microsoft's Clean Architecture says Infrastructure "should have a reference to the Application Core project" (Microsoft, n.d.-a). Written as an open point for the team, not decided by me.
4. **The support dates.** The code conventions said .NET 10 is supported "until November 2028" and .NET 8 stops on 10 November 2026. The Microsoft page, read on 8 October 2026, says 14 November 2028 and 10 November 2026, and adds that .NET 9 stops on the same day as .NET 8. Rule 1 now has the exact date and names .NET 9.

## AA3 Weighing the issue from several sides

School asks: "You analyze issues from multiple perspectives and weigh the societal and ethical consequences."

Consequences in `002-layeredArchitecture.md` are split per party: the team new to .NET, the maintainer after January 2027 and the foundation. The heaviest is a platform that is switched off because nobody can change it, and that is why a structure the compiler checks won over one only a reviewer checks.

## AA4 Giving advice that weighs alternatives and risks

School asks: "You give well-founded advice in which you weigh alternatives, identify risks and make a substantiated recommendation."

1. **Two tables with five rows each.** Files for one new feature, what a beginner must learn, how a rule is unit tested without the database, what catches a mistake and the source. Every claim names a Microsoft page or is marked as an opinion.
2. **The compiler proof.** Run on 8 October 2026 with the .NET SDK 10.0.401 on a throwaway copy: Domain gives CS0234 on `Connect.Infrastructure.Persistence.ConnectDbContext`, the project Connect builds the same line with 0 errors. Pasted in the decision file.
3. **The .NET versions with their risk.** .NET 8 and .NET 9 run out on 10 November 2026, .NET 10 is newer with fewer examples for beginners. Next upgrade planned no later than May 2028.

## AA5 Adapting the communication to each audience

School asks: "You adapt your communication to different target groups in form and language."

1. **For the client.** The Consequences of both files start with the foundation: until when Connect gets security fixes, and why a later developer can change it.
2. **For the team.** The exact project names, the direction of the references, the test line and where `net10.0` is set.
3. **A teammate new to .NET explains the four layers in their own words.**
   - TODO a teammate new to .NET: write a comment in #12 that explains the four layers in your own words.

## TODO

- [ ] Whole team: confirm both files at a stand up and fill in Agreed by. Write in #12 the date and who was there.
- [ ] Whole team: each writes the line of `dotnet --list-sdks` that starts with 10 in `003-dotnet10.md` and in #12.
- [ ] Whole team: choose the answer to the open point of rule 8 in `002-layeredArchitecture.md`.
- [ ] Owner of #259: link both decision files from `docs/architecture/architecture.md`.

## References

Microsoft. (n.d.-a). *Common web application architectures*. Microsoft Learn. Retrieved October 8, 2026, from https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

Microsoft. (n.d.-b). *Testing without your production database system*. EF Core, Microsoft Learn. Retrieved October 8, 2026, from https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database

Microsoft. (n.d.-c). *CQRS pattern*. Azure Architecture Center, Microsoft Learn. Retrieved October 8, 2026, from https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

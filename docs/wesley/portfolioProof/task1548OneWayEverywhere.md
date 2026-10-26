# Proof: task 1.14 The code convention on one way everywhere (#1548)

Merge request !79, branch `docs/1544-null-forgiving-operator`, together with tasks 1.10 to 1.13 (#1544 to #1547). Issue: 01 The code of Connect in one safe place (#1). File: `docs/team/codeConventions.md`, rule 12 of the part General and Git.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk.** Connect is one product, built by five students, four of them new to .NET, and handed to a maintainer after January 2027. Every place where the same thing is done in two ways is one more thing such a person has to know before the code makes sense. When the second way is written nowhere, it looks like a mistake, or it is copied as if it were the rule.
2. **Counted on dev, 21 October 2026, at 7aa7ab24.** The conventions already name their exceptions with a reason: GET /api/health answers with its own body because deploy jobs read it (rule 2 of Answers of the server), a record ...Request or ...Response may sit in the endpoint file of its feature (rule 11 of Files and folders), and on !79 backend rule 17 names a test that passes `null` on purpose and the navigation inside an EF Core query. Those stay, because each says why and where. Second ways that no rule names:
   - `CapturingLoggerProvider` in `api/Connect.Tests/Fixtures`, a logger written by hand that `AccountLoggingTests` and `LoggingTests` read the log lines through, while rule 4 of the part Tests and mocking, merged the same evening with !77, says every stand-in in a test is a substitute of NSubstitute
   - `web/src/api/accounts.ts` reads each field of a wrong form under two names, `errors.Email ?? errors.email`
   - the setup of a database test written out in four test classes, which backend rule 20 on !79 brings to one helper
3. **The rule.** Every rule holds for all code, without exceptions wherever that is possible. An exception is made only where the shared way does not work, and then it is written in the rule it departs from, with the reason and where it holds. A way only one part uses is changed to the shared way or proposed as the new shared way, never kept beside it.
4. **The source.** Microsoft's page on the C# code conventions: "Coding conventions are essential for maintaining code readability, consistency, and collaboration within a development team. Code that follows industry practices and established guidelines is easier to understand, maintain, and extend." (Microsoft, 2025).
5. **Kept transferable.** A new member reads the conventions once and can trust that the code does what they say, with every exception named in the rule it belongs to, instead of finding them one by one in the code.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Where the rule comes from.** My decision of 21 October 2026, the evening a second way of testing the log, a capturing logger written by hand, was found on dev next to the rule that every stand-in is a substitute of NSubstitute. Each team member followed a rule that made sense on its own, so the conventions needed one rule about the rules.
2. **No teammate's merge request has to change for it.** Code that is older than a rule follows it once its file changes, as backend rules 17, 18 and 20 already say, and new code never copies the older way.
3. **Which way is shared.** Where two ways exist, the team chooses one for all of it. For .NET the team agreed that Nick's code sets the way (the item "Nick's .NET code sets the way" in `docs/team/openItems.md`), so for .NET code the shared way is the way his code does it. Everywhere else, rule 12 asks for a proposal to the team instead of a second way.
4. **Placed where everyone reads it.** Rule 12 sits in the part General and Git, which holds for every layer, and not in the backend part, because the screen has its own second way as well.

## TODO

- [x] A teammate who did not write the rule: read rule 12 of General and Git and write in the review what was unclear. Gurpreet Singh, 23 October 2026, in !79: it is clear to him.
- [ ] The team: confirm rule 12 at the stand up, and name who brings `CapturingLoggerProvider` and the two names in `accounts.ts` to the shared way.

## References

Microsoft. (2025). *Common C# code conventions*. Microsoft Learn. Retrieved October 21, 2026, from https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

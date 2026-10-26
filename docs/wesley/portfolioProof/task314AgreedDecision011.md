# Proof: my agreement to decision 011, a migration only adds (#314)

Merge request !122, branch `docs/314-agreed-decision-011`, built on !121 of Gurpreet Singh. Issue: 17 A health check and structured logs, task 17.4 The way back to the previous version (#314), which is Gurpreet's task. File: `docs/decisions/011-migrationsOnlyAdd.md`, my line under Agreed by.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS4.

A point that needs someone else is marked TODO with who and what.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **What I checked before I agreed.** Gurpreet's steps to go back to the previous version on the test site (!121), held against the project and not only read: GitLab may run an older deploy again with our setting that stops outdated deploys, a deploy takes 56 to 78 seconds, the migration bundle takes the name of a migration, and Make an account answers "This email address already has an account" for an email that is already there. I approved !121 on 26 October 2026.
2. **Why I agree.** My own way back in production (!112, `rollback-production`) also runs the older version on the newer tables, so it only works while a migration only adds. The decision names one of my migrations as the example of what does not count: `20261021205912_AddMemberDetailsToProfile` adds a check to `profile` that `first_name` is not empty, and a version before it that saves a profile without `first_name` gets the default `''`, which the check refuses. That is right, so from now on I put such a check in a later merge.
3. **Where it met my own work.** !121 and my !120 change the same line of database rule 12, so the second of the two to be merged gets a conflict. I told Gurpreet in my review that I adjust mine if his goes in first.
4. **The check account.** Before the practice of #314 I made the check account that step 6 of the way back needs, on the test site. The same email again answered `409 This email address already has an account`, which is the check of step 6. Its email and password are in no document and no chat.

## TODO

- [ ] Wesley: the practice of #314 with a stopwatch, after Gurpreet puts the broken version on the test site. Gurpreet writes the result in part Practised of `docs/team/onboarding.md`, and I tick it on #314. No change to this branch for it.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

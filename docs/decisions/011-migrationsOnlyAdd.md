# 011 A migration only adds

**Status.** Proposed by Gurpreet Singh on 26 October 2026, for the stand up. The team agrees at the stand up, and each member adds a line under Agreed by. Only after that, rule 13 under Database of `docs/team/codeConventions.md` links this file.
**Issue.** 17 A health check and structured logs, task 17.4 (#314): the way back to the previous version, and the point about this rule in `docs/meetings/talkAbout.md`.
**Related decision.** [001 Code conventions](001-codeConventions.md), rule 7 under Database: tables only change through EF Core migrations.
**Who decides.** The whole team, because every member writes migrations.

## Context

Every merge into dev puts Connect on the test site, and its migrations run first (part The test site of `docs/team/onboarding.md`). When a merge breaks the site, the way back is Rollback on Operate, Environments, test: GitLab runs the deploy-test job of the last healthy version again (part Going back to the previous version). That brings back the older code. It does not bring back the older database.

The older job runs its own migration bundle. A bundle without a name applies the migrations it is missing and stops. It never undoes a migration it does not know, because only the bundle that holds a migration has the code to undo it (its Down). The `--help` of the bundle, checked on 26 October 2026 with EF Core 10.0.12, says: `<MIGRATION>  The target migration. If '0', all migrations will be reverted. Defaults to the last migration.` So after a Rollback, the older version runs on the newer tables.

Whether that works depends on what the newer migration did:

- **It added a table or a column.** The older code does not know it and does not use it. It runs as before. A new column needs a default or must allow empty, or the older code cannot add a row.
- **It removed or renamed a table or a column, or changed its type.** The older code still asks for the old name or type and gets an error from the database. The site stays down after the Rollback, and the only way back is to undo the migration, which also deletes the data it moved.

EF Core itself is careful with the second kind: "The migration scaffolding process warns when an operation might result in data loss (like dropping a column)", and a rename it does not recognise becomes a drop and an add, after which "all your customer names will be lost" (Microsoft, n.d.-b, Column renames section). Microsoft also warns that a migration "may drop a column when the intent was to rename it, or may fail for various reasons when applied to a database" (Microsoft, n.d.-a).

## Options

- **Undo the migration on every way back.** Run the bundle of the broken version with the name of the last good migration before the Rollback. It works for every kind of change, but it needs someone on the server with sudo, it is not practised, it deletes the data of the new tables and columns, and a migration that already deleted data cannot bring it back.
- **A migration only adds.** A migration adds tables, columns and indexes and never removes, renames or changes one in the same merge. Removing or renaming happens in two steps: first a merge that adds the new one and stops using the old one, and only when that version runs well, a later merge that removes the old one. Then every Rollback keeps the database as it is, and anyone in the team can go back in five minutes from the GitLab page alone.

## Decision

A migration only adds. Proposed by Gurpreet Singh on 26 October 2026.

1. A migration adds tables, columns, indexes and seed data. A new column has a default or allows empty. A new check, unique or foreign key rule on a table that the previous version already writes to does not count as only adding: the previous version does not know the rule, so after a Rollback its writes fail while /api/health stays healthy. For example, 20261021205912_AddMemberDetailsToProfile gives first_name the default `''` and also a check that first_name is not empty, so an older version that saves a profile without first_name gets `''` and the check refuses it. Such a rule goes in a later merge, like removing something (rule 2).
2. Removing, renaming or changing the type of a table or a column takes two merges, in two different merge requests: first add and switch, later remove. The second one names the first in its description.
3. A merge request with a migration says in its part The version to go back to whether the migration only adds. If it does not, the reviewer asks why, and the author writes how to go back.

Why the other way lost:

- **Undo the migration on every way back.** It turns a five minute job for anyone into a job for one person with sudo who knows EF Core, and it loses data. That is exactly the risk of one person knowing the way back that task 17.4 (#314) wants to remove. It stays as the last resort for a migration that broke by itself (part The database stays as it is).

## In plain words

Think of the database as a cupboard that two versions of Connect share. The new version may put in a new drawer, and the old version simply does not open it. But when the new version takes a drawer out, or puts a different label on it, the old version reaches for a drawer that is not there and falls over. So a merge only adds drawers. Taking one out happens later, in a merge of its own, once nobody needs it anymore.

## Consequences

- **Going back is the same five minutes every time.** Rollback on the GitLab page, and the database stays as it is.
- **Removing something takes two merges.** That is more work for a rename. In return, the version between the two merges works with both the old and the new tables.
- **Old columns stay a little longer.** A column that is no longer used stays until the second merge.
- **A broken migration is still the weak part.** When a migration breaks the site by itself, the way back is the bundle with the name of the last good migration, run before the Rollback. That way is written down but not practised.

## Agreed by

| Name | Date agreed |
|---|---|
| Bayon Eerhart | |
| Nick Hoebe | |
| Wesley Oerlemans | |
| Gurpreet Singh | 26 October 2026 |
| Kelvin van Voorst | |

## References

Microsoft. (n.d.-a). *Applying migrations*. Microsoft Learn. Retrieved October 26, 2026, from https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying

Microsoft. (n.d.-b). *Managing migrations*. Microsoft Learn. Retrieved October 26, 2026, from https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/managing

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

# Test data

What the server adds to its own database for testing, and where the values that make that
possible live. Read `docs/architecture/configuration.md` first for the general rule on
configuration and secrets.

## Test member

One made up account, never a real person: `test-member@example.org`, email confirmed, with no
profile. `TestDataSeeder` (`api/Connect.Infrastructure/Persistence/TestDataSeeder.cs`) adds it
once through `UserManager<Account>`, the same way an account made through the product will one
day exist, so a later check can log in as a known member. It only ever adds: an account that is
already there is left exactly as it is, even when the password setting changed since, and the
seeder never deletes or changes a row.

The test member exists only on a laptop and on the test site, controlled by the setting
`Seeding:TestData`. It is never added on the live site, because the live site gets no `Seeding`
settings at all (#277).

| Environment | `Seeding:TestData` | The password comes from |
|---|---|---|
| Laptop | `true` in `appsettings.Development.json` | A .NET user secret, `Seeding:TestMemberPassword`, set once with `dotnet user-secrets set` in `api/Connect` |
| Test site | `false` by default in `deploy/.env.example`, `true` on the server | The masked and protected GitLab variable `SEEDING_TEST_MEMBER_PASSWORD`, written into `pipeline.env` on every deploy once the deploy of #267 exists |
| Live site | Not configured | Not applicable, the setting does not exist there |

The password itself is never written in this document, in a log line or in Git. An empty or
missing password adds nothing: the server logs one warning and starts without a test member.

# Test data

The accounts and data that exist only to test Connect, on a laptop and on the test site https://31-70-66-186.sslip.io. Every address here is made up and ends in `@example.org`, a domain that never delivers mail (RFC 2606). No real person, email address or password goes into the test site (`docs/pages/CreateAccount.md`, Privacy).

Each task that adds test accounts or seed data adds its lines here, in the same merge request.

## Accounts made by the end to end tests

| Address | Made by | What for |
|---|---|---|
| `e2e-<time>@example.org`, for example `e2e-1792579200000@example.org` | `web/e2e/createAccount.spec.ts`, once per run | The browser test of making an account (#265). `<time>` is the number of milliseconds of `Date.now()`, so every run makes a new account and never meets an old one. |

- The password is made up during the run with `randomUUID()` plus `Aa1!`, so it meets every password rule of the server, and is not in Git. The trace of a failed run (`test-results/`) does show it, because a trace keeps every typed value. That is accepted: the account is made up and holds no data.
- Once the job e2e-test of #267 runs the test after every deploy, these accounts pile up on the test site, one per deploy of dev. They hold no personal data and are not in the seed data, so a test site that starts over with an empty database no longer has them.

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to draft
the formulation of sentences, the structure of wording, readability, and clarity from
content that we had provided. The content itself, including our experiences, reflections,
learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the
drafts and decided which text to include.

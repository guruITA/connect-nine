# Proof: task 96.2 A read only browser test of production after every deploy (#1059)

Merge request !113, branch `chore/1059-production-smoke`, built on !112 (task 97.1). Issue: 96 A merge into main goes to production (#162). Files: `web/e2e/productionSmoke.spec.ts`, the job `smoke-production` in `.gitlab-ci.yml`, a line in `docs/team/onboarding.md` (part 14, From dev to main to production) and one in `docs/team/gitlabSetup.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Designing and Realising OR4.

A point that needs someone else is marked TODO with who and what.

## OR4 Validating the solution

School asks: "You systematically validate that the solution does what it should, and draw conclusions for improvement."

1. **What the two tests prove that the health loop of deploy-production does not.**
   - The loop of `deploy-production` asks `/api/health` with curl, which says the server and its database run.
   - The first test opens the home page in a real browser and checks that React drew the screen inside `#root`. That fails when the screen does not load in a browser, for example a file of the screen that is missing or a script error, while the health address stays healthy.
   - The second test asks the health address through the request of the same page, so it goes to the same address as the browser, with the padlock of Caddy and through Traefik.
2. **It can fail, shown on purpose.** Against an address where Connect does not answer, both tests failed: the home page with `net::ERR_SSL_PROTOCOL_ERROR` and the health address with an SSL alert.
3. **Run where it must pass.** On 26 October 2026 both tests passed against production, https://live.31-70-66-186.sslip.io, and against Connect on my own computer with `docker compose --profile app`. ESLint and Prettier found nothing.
4. **What it does not catch, and why it stays read only.** It does not log in, send a form or make an account, so a broken log in or a broken save would pass. That is on purpose: production may hold real data of young people, and a test that writes would leave its own rows there. When the foundation allows a test account on production, the improvement I would make is one more test that logs in with that account and opens its own profile, still without changing anything.
5. **Where it fits.** `smoke-production` runs on main right after `deploy-production` (`needs`), with the same Playwright image as `e2e-test`, uses no token or secret, and keeps its report and trace for seven days when it fails. A red run means the release is live but does not work for a visitor, and the guide sends the reader to Going back in production.

## In teammates' files

`web/e2e` holds the browser tests Gurpreet Singh started (#265). `productionSmoke.spec.ts` is a new file beside them in the same shape as `home.spec.ts`, and none of his files changed. `e2e-test` on dev runs every file of `web/e2e`, so it also runs these two read only tests against the test site.

## TODO

- [ ] After the merge, no change to this branch: the first green `smoke-production` after the first merge of dev into main, in #162.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

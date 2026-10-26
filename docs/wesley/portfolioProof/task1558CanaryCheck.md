# Proof: task 96.4 The new version checked beside the old one before visitors move (#1558)

Merge request !111, branch `chore/1558-canary-check`, built on !110 (task 96.1). Issue: 96 A merge into main goes to production (#162). Files: `infra/k3s/canaryCheck.sh`, `infra/k3s/canaryCompare.sh`, `infra/k3s/canaryPaths.txt`, three lines of `deploy-production` in `.gitlab-ci.yml`, and `docs/team/onboarding.md` (part 14, From dev to main to production, The canary check).
Profile: Full Stack Software Developer, level 3. Portflow: Designing and Realising OR4.

A point that needs someone else is marked TODO with who and what.

## OR4 Validating the solution

School asks: "You systematically validate that the solution does what it should, and draw conclusions for improvement."

1. **What the readiness probe does not see.** Visitors already only reach a pod whose `/api/health` says healthy. That says the server and its database run, not that the new version answers the way the old one did: a page that stopped loading, a missing file of the screen, or an address that now answers 500 all pass it. So before visitors move, the new version runs beside the old one as `connect-server-canary`, with no visitors, and a Job inside k3s asks both the same addresses.
2. **The same settings, so a difference is the version.** The canary is a copy of the Deployment `connect-server` with only its name, labels and image changed: the same Secrets, settings, probes, limits and wait before stopping. A difference it finds comes from the new image and not from a second setup that drifted.
3. **What it compares.** Every address in `canaryPaths.txt` must answer the new version with the same status and the same kind of content as the old one. The home page must have its root element and every file it names must load, `/api/health` must say healthy, and the canary must not restart and log no Error or Critical while it is checked. It only sends GET.
4. **Tried on production, passed.** On 26 October 2026 I ran `deploy-production` exactly as the pipeline file gives it, with the version that runs now. The migrations ran alone first, the canary got ready, and all five addresses answered the same: `/` 200 text/html, `/create-account` 200 text/html, `/api/health` 200 application/json, `/api/accounts` 405, and `/api/does-not-exist` 404 application/problem+json. The new version was healthy, its home page had its root element and both of its files loaded, and the canary restarted 0 times with 0 lines of Error or Critical. Then the full apply had nothing to change.
5. **Tried on production, stopped.** A canary with an image that does not exist never got ready ("ErrImagePull"), and after the waiting time the check said "Production stays on" the old version and ended red. My first run stopped too: the curl image runs as `curl_user`, and Kubernetes only accepts `runAsNonRoot` with a number, so the check never started ("CreateContainerConfigError"). The deploy stopped before the switch with "no answer from the check", and I added `runAsUser: 100`. Each time the canary, its Service, the Job and its ConfigMap were gone afterwards. The live version answered all 836 checks of its health address during the three runs, and the test site as well.
6. **A difference found.** A difference between two versions cannot be shown on production today, because every image in the registry holds the same code of the server and the screen. So I ran `canaryCompare.sh` in the same curl image against two small test servers: one with a missing file of the screen, an unhealthy health answer and an address that answered differently. It printed `MISSING`, `NOT HEALTHY` and `DIFFERENT` and ended with exit 1. With equal servers it ended with exit 0.
7. **What it does not catch, and why the migrations come first.** Nothing behind the log in, and nothing the new version writes, because the check only reads. The migrations of the new version run before the canary, because the new version needs its own tables. A migration only adds, so the old version keeps working on those tables when the check stops the deploy.
8. **A finding for later.** The log of the server pod that started on 25 October 2026 has one line of level Error: its first health check said the database could not be reached, 886 milliseconds after the start, and the next one was healthy. That is why the canary only counts errors from the moment the comparison starts, after it is ready.

## TODO

- [ ] Wesley: the first canary check of a real new version, in the first `deploy-production` after the merge of dev into main, pasted in #162.
- [ ] The team: a merge request that changes an answer on purpose also changes `canaryPaths.txt`, named in part 14 of the getting started guide.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

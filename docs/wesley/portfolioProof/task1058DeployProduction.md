# Proof: task 96.1 Every merge into main goes to production by itself (#1058)

Merge request !110, branch `chore/1058-deploy-production`, built on !109 (task 94.2). Issue: 96 A merge into main goes to production (#162). Files: the job `deploy-production` in `.gitlab-ci.yml`, `infra/k3s/waitForHealthy.sh`, a time limit on the rollout in `infra/terraform/connectServer.tf`, `docs/team/onboarding.md` (part 14 Changing production, From dev to main to production), and a line each in `docs/team/gitlabSetup.md`, `docs/architecture/architecture.md` and `docs/architecture/infrastructure.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The gates, by name, and none may fail.** On a push pipeline of main the jobs before `deploy-production` are backend-build, frontend-build and image-push (build), env-file-check, backend-test, frontend-lint, frontend-test and terraform-check (test), and backend-scan and frontend-scan (scan). I read the rules of every job for a push pipeline of main with a small script, and no job before deploy has `allow_failure`. Only `kube-check` has it, in stage deploy and only by hand on a merge request. The code quality check (#153) and the scan of the images (#154) do not exist yet, and the guide names which issue adds each.
2. **A rule of the card I did not follow, on purpose.** The card asks for `needs: image-push`. With `needs` a job starts as soon as the jobs it needs are done, before the tests and the scan. That would put a version live that no test had passed, so the job waits for its stage instead, which already comes after image-push.
3. **When the new version never gets ready.** The apply waits at most five minutes for the rollout and then stops, and `waitForHealthy.sh` asks the health address at most 12 times. Either turns the job red. The old version keeps serving all the while, because the rolling update keeps the old pod until the new one is ready, and the wait of five seconds before a pod stops keeps visits away from a stopping pod (#1051). I checked the script against an address that answers `{"status":"unhealthy"}`: it does not count that as healthy, because "unhealthy" also holds the word, so it checks the whole `"status":"healthy"`.
4. **The weekly scheduled pipeline.** It must only scan, never deploy. The first rule of the job removes it from a scheduled pipeline, and the job only runs on a push pipeline of main, so a pipeline started by hand does not deploy either.
5. **Tried before it runs.** I ran `deploy-production` exactly as the merged pipeline file gives it, in its own image with the project's variables, from my computer, with my own GitLab token in place of the job token and the version that runs now. It filled the three Secrets, applied ("Apply complete! Resources: 0 added, 1 changed, 0 destroyed", the annotation left by a restart by hand), waited for the rollout, and the health address answered 200 healthy at the first try. The test site and the live version answered every check of their health address during it. A script found no secret value and no token in the log.
6. **Transferable.** The part From dev to main to production of the getting started guide gives the road from a branch through dev and the test site to main and production, the gates by stage, where to see which version runs, and the five steps when a deploy is red, so a teammate can release without me.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **The gates of others.** The scan of #22 is in stage scan, before deploy, so a hole found in the packages stops a release. The code quality check of #153 and the scan of the images of #154 get their own jobs before deploy when they are built, and then `deploy-production` waits for them without a change.
2. **What #163 needs from this job.** The owner of #163, going back to a previous version, is me. Its job uses the same environment `production`, the same `resource_group: production`, so a rollback never runs beside a deploy, and the same login `gitlab-deployer`. The deployer token is used only by `deploy-production` and, later, the jobs of #163.
3. **The test site stays as it is.** `deploy-test` and `e2e-test` of Gurpreet Singh are unchanged and still run on dev only.

## TODO

- [ ] Wesley: after the first merge of dev into main, paste the green `deploy-production`, the line in Operate, Environments, production, and the image version on production next to the short commit id of the pipeline.
- [ ] A teammate who did not build issue 96: task 96.3 (#1060), a small change from dev to main followed to production, with the name of who found the running version in GitLab from the guide.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

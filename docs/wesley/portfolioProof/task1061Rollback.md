# Proof: task 97.1 Going back to the previous version with one job (#1061)

Merge request !112, branch `chore/1061-rollback-production`, built on !111 (task 96.4). Issue: 97 Going back to the previous version is practised (#163). Files: the hidden job `.production-release` and the job `rollback-production` in `.gitlab-ci.yml`, with `deploy-production` now extending `.production-release`, and `docs/team/onboarding.md` (part 14, Going back in production).
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **Three ways to go back, and why the first.**
   - **Terraform with an older version**, the job `rollback-production`. It applies the files of main with the older image, so the files, the state and k3s keep agreeing on what runs.
   - **`kubectl rollout undo`.** It changes k3s behind Terraform's back: the files and the state still say the broken version, so the next plan shows it and the next deploy puts it back. It also needs the login that only the pipeline has.
   - **The Rollback button of Operate, Environments.** It runs the deploy job of an older pipeline again, with the pipeline file and the Terraform files of that older commit, so it would also undo every later change to production that was not broken.
2. **The traps I removed.**
   - **A wrong version.** An empty `ROLLBACK_IMAGE_TAG` stops with "Type the version to go back to, from Operate, Environments, production", and anything that is not 7 to 40 letters a to f and digits, or the drill tag `drill-97`, stops with "This is not a version". Nothing changes then.
   - **Drift.** The way back goes through Terraform, so nothing is changed by hand.
   - **The broken code still on main.** The guide says to revert it through dev and main before any other release, or the next merge puts it back.
   - **A main pipeline that never finishes.** The job is by hand with `allow_failure`, so a pipeline of main is not left waiting on its button.
3. **Tried on production, without the canary check.** On 26 October 2026 I ran `rollback-production` exactly as the pipeline file gives it, from my computer, in its own image with the project's variables:
   - With no version, it stopped with the first message.
   - With `notAVersion`, it stopped with the second.
   - With `8bc56049`, an older version, it ended with "Apply complete! Resources: 1 added, 1 changed, 1 destroyed", the rollout and a healthy answer at the first try.
   - With `292759f1`, forward again, the same.

   The migrations Job of the older version ran again under its own name and said "No migrations were applied. The database is already up to date." The test site and the live version answered every check of their health address during all of it.
4. **Why no canary check when going back.** The earlier version was healthy on production before, and a comparison with the broken one would stop the way back for the very difference it is meant to remove.
5. **Transferable.** The part Going back in production gives seven numbered steps for a person under stress who did not build any of it, and the parts The database stays and How far back.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **One set of steps for both jobs.** `deploy-production` and `rollback-production` share `.production-release`: the Secrets, the apply, the rollout and the health check, the login, the resource group and the environment. A rollback never runs beside a deploy, and both show in Operate, Environments, production.
2. **The migration rule of the team.** Going back only works while a migration only adds. That rule is proposed in #314 of Gurpreet Singh, which asks for a decision file `migrationsOnlyAdd.md`. It has no decision file yet, so I put it on the list for the stand up instead of writing it myself.
3. **What the recording and the demo need (#168, #169).** The job name `rollback-production`, the variable `ROLLBACK_IMAGE_TAG`, and the drill of task 97.2 (#1062) with the tag `drill-97`, which this job already accepts.

## TODO

- [ ] After the merge, no change to this branch: the refused runs and a run back and forward in the pipeline of main, visible in Operate, Environments, production, go in #163 after the first merge of dev into main.
- [ ] A teammate: read the steps of Going back in production back to me, with their question, in #163. A change it leads to comes in before anyone approves, or in a merge request of its own.
- [ ] Gurpreet Singh: the rule of #314, a migration only adds, as its decision file in a merge request of his. No change to this branch for it, and the point is on the list for the stand up.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

# Proof: task 94.1 The folder infra/terraform with pinned versions and the state in GitLab (#1047)

Merge request !107, branch `chore/1047-terraform-folder`, built on !105 (task 93.2). Issue: 94 Every production change shown as a plan first (#158). Files: `infra/terraform` (`terraform.tf`, `.terraform.lock.hcl`, `variables.tf`, `providers.tf`, `connectServerConfig.tf`), the job `terraform-check` in `.gitlab-ci.yml`, `.gitignore` and `docs/team/onboarding.md` (part 14 Changing production, Files and versions). The choice behind it: `docs/decisions/010-productionOnK3s.md`, merge request !102.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The same result next year.** Terraform 1.17.0 and the provider hashicorp/kubernetes 3.4.0 are pinned with `=`, each with its link and release date, and the lock file is committed as `terraform init` wrote it. The job `terraform-check` runs the image of exactly that Terraform version.
2. **The state cannot leak into Git.** `terraform.tf` has `backend "http" {}` with no address, project number or token, and `.gitignore` keeps `.terraform/`, `*.tfstate`, `*.tfstate.backup` and `*.tfplan` out. Before I chose the backend I checked on gitlab.fdmci.hva.nl that GitLab keeps a Terraform state, by sending a test state and deleting it again (decision 010).
3. **No cluster by accident.** `kubeconfig_path` has no default, so Terraform cannot reach a cluster unless the pipeline gives it a login.
4. **Tried before it reaches production.** On 24 October 2026 `terraform init -backend=false`, `terraform fmt -check -recursive` and `terraform validate` passed in the image `hashicorp/terraform:1.17.0`. On the copy of the server from task 93.1 (#1044), with a local state and through an ssh tunnel to its Kubernetes API, the plan said 1 to add, the apply ended with "Apply complete! Resources: 1 added, 0 changed, 0 destroyed", and adding one label gave a plan with 0 to add, 1 to change and 0 to destroy. The pipeline file passed the CI lint of the project. On the HvA runners `terraform-check` ran for the first time in !106 on 25 October 2026, where a file with a missing brace turned it red as it should (job 1352536).
5. **Transferable.** Part 14 of the getting started guide says what each file is for, which versions are used since when, where the state lives and which three commands a team member runs on a laptop before a merge request.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **The pipeline rules of the team.** `terraform-check` runs in stage test without `needs`, so it waits for the build like every other test job, and it skips the weekly scan pipeline with the same first rule as every job.
2. **What belongs to another task stays there.** The namespace `connect-production` comes from `infra/k3s/clusterAccess.yaml` of task 93.2 (#1045), and the helm provider comes with #164, so this folder creates neither.
3. **Shared files only get additions.** `.gitlab-ci.yml` gets one new job and `.gitignore` four lines, and nothing that is there changes.

## TODO

- [x] Wesley: `terraform-plan` and `terraform-apply`, the first merge request checks and the red merge request with a missing brace, in task 94.2 (#1048), merge requests !109 and !106, 25 October 2026.
- [x] Wesley: the part Plan, review, apply of part 14 Changing production, in !109.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

# Proof: task 93.2 The pipeline reaches k3s only through an ssh tunnel with two logins (#1045)

Merge request !105, branch `chore/1045-cluster-access`, built on !103 (task 93.1). Issue: 93 k3s on the production server (#157). Files: `infra/k3s/clusterAccess.yaml`, `infra/k3s/openTunnel.sh`, `infra/k3s/kubeconfigFromToken.sh`, the jobs `.production-access` and `kube-check` in `.gitlab-ci.yml`, and `docs/team/onboarding.md` (section Production server, part Pipeline access).
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **No open door for the pipeline.** The Kubernetes API stays closed in ufw. The pipeline logs in as the Linux user `tunnel`, which has the shell `/usr/sbin/nologin` and a key that may only forward to `127.0.0.1:6443`. Tried from a clean container like a runner on 25 October 2026: the forward to 6443 answered, a forward to port 22 of the server gave nothing, and a command as `tunnel` answered "This account is currently not available".
2. **Each login only what it needs.** `gitlab-planner` makes the plan on every merge request, also on branches nobody reviewed yet, so it may only read and never a Secret. `gitlab-deployer` may change things, only in `connect-production` and only in the protected pipelines of dev and main. `kubectl auth can-i` on the server: the planner may list pods but not create a deployment or read a Secret, the deployer may create a deployment in `connect-production` but not in `kube-system`.
3. **The host of the tunnel is checked.** `PRODUCTION_SSH_KNOWN_HOSTS` comes from `ssh-keyscan`, and I compared its three fingerprints with the host keys on the server itself before I stored it.
4. **A leak can be closed, measured instead of assumed.** When I made a token Secret again, the token stayed the same, because it only depends on the ServiceAccount and the name of the Secret. So the guide says to make the login itself new after a leak. I did that once already: the old tokens had been on the disk of my computer for under a minute, so I made both logins new and the old tokens then answered "Unauthorized".
5. **Pinned and checked.** The job image is `alpine:3.20.10`, ssh is `openssh-client-default=9.7_p1-r5`, and kubectl v1.36.5 is checked against its sha256 from dl.k8s.io. shellcheck 0.11.0 gives no remark on the two scripts, and the CI lint of the project says the file is valid. The before_script and script of `kube-check` ran in that image from this desktop and ended with `node/connect condition met`.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **One way for every production job.** The tunnel and the login file sit in the hidden job `.production-access`, so the jobs of #158 to #163 extend it instead of each writing their own.
2. **The pipeline rules of the team.** `kube-check` skips the weekly scan pipeline with the same first rule as every job, and runs in stage deploy after the build and the tests.
3. **Secrets the way the team keeps them.** Every key and token is a GitLab variable, masked where GitLab can mask it, and none is in a file of the repository, as part 4 of the getting started guide asks.

## TODO

- [ ] Wesley: after the merge into dev, paste the first green `kube-check` of dev in #157.
- [ ] Wesley: in the first merge request of #158, check whether `PRODUCTION_SERVER_HOST` must stop being protected, and write the answer with the reason in the part Pipeline access.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

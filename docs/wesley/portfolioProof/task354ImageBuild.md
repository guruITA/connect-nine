# Proof: task 20.3 The images built in the pipeline with kaniko (#354)

Merge request !89, branch `chore/20-dockerfiles-for-the-server-and-the-frontend`, together with task 20.1 (#352). Issue: 20 Connect starts with one command on any computer (#32). Files: `.gitlab-ci.yml` (jobs image-build and image-push), `docs/decisions/008-kanikoForImages.md`, `docs/team/gitlabSetup.md` (part Images and the registry).
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risks the two jobs remove.** A broken recipe found late: image-build builds both images on every merge request and pushes nothing. An unreviewed image in the registry: only image-push on dev, after a reviewed merge, writes to the registry. A registry that fills up: its cleanup policy keeps the ten newest versions of each image, set on 22 October 2026.
2. **What I found out about the runners first.** Before writing a job I tried every way to build an image on the HvA runners, on 22 October 2026. Docker in Docker failed on privileged mode, and Buildah, Podman and BuildKit rootless failed on a user namespace. Only kaniko built, and it pushed to the registry of the project, which is on. The kaniko of Google turned out to be archived since June 2025, so I tried the maintained fork, which also builds.
3. **A conclusion I checked again.** My first conclusion was that only kaniko works. Wesley asked whether GitLab should not work with the others, so I checked it at the sources: a job that read the settings of the runner (pipeline 586259) and GitLab's own documentation. GitLab does support Buildah and BuildKit, and its documentation names our exact BuildKit error: the seccomp profile of the runner blocks the calls. So the limit is the setting of the HvA runners, which only their administrator can change, not GitLab. That changed the decision: it names asking the HvA as the way back to a builder GitLab maintains itself, instead of calling kaniko the only way.
4. **The decision, with the options that lost.** `docs/decisions/008-kanikoForImages.md` holds every option with its measured result, why each lost, and when the choice is made again.
5. **The red run and the green run.** Measured in !89, see the TODO below until both runs are written here.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Next to the jobs of #266.** image-build sits in the stage build next to backend-build and frontend-build, and both new jobs start with the rule of #308 that skips the weekly scheduled pipeline, as every job except the two scans does.
2. **Only on dev.** image-push runs only after a merge into dev, so every image in the registry is a version the team reviewed and the test site (#355) can start by its commit id. Production gets its version from main later: #159 and #162 give the image tag of the commit to Terraform.
3. **The setup guide.** Part Images and the registry of `docs/team/gitlabSetup.md` says what the runners allow, which builder and why, where the images are and how the cleanup works, and the Pipeline part lists both jobs.

## TODO

- [ ] Wesley: the red run with a broken COPY line and the green run after the revert, with the minutes each took and how many minutes image-build adds to a merge request, in this file and in the merge request.
- [ ] Wesley: tell the team at the stand up that the registry holds a version of every merge into dev from now on.
- [ ] A teammate: after the merge, find the image `server` of the merge in Deploy, Container registry, and write your name in the review or on #354.
- [ ] The team: confirm decision 008 at the stand up and fill in Agreed by.

## References

GitLab. (n.d.). *Build Docker images with BuildKit*. GitLab Docs. Retrieved October 22, 2026, from https://docs.gitlab.com/ci/docker/using_buildkit/

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

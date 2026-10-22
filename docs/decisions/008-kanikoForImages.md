# 008 How the pipeline builds the images: the maintained fork of kaniko

**Status.** Proposed on 22 October 2026 by Wesley Oerlemans. The team confirms it at the stand up, and each member adds a line under Agreed by.
**Issue.** 20 Connect starts with one command on any computer (#32), task 20.1 (#352) and task 20.3 (#354).
**Who decides.** Wesley Oerlemans, who builds the images. The whole team confirms.

## Context

The pipeline turns the Dockerfile into two images, server and migrations, and puts them in the container registry of the project, so the test site (#355) and production (#159) run exactly the version that was reviewed. The pipeline runs on the six runners of the HvA (tag hva, GitLab Runner 19.4.1). The team uses them as they are and cannot change their settings.

How those runners run a job, read on 22 October 2026 by a job that only reads (pipeline 586259):

- every job runs as root in a Docker container, with Docker's default seccomp profile, the AppArmor profile docker-default and without the capability CAP_SYS_ADMIN
- the hosts run Ubuntu with kernel 6.8, where `kernel.apparmor_restrict_unprivileged_userns` is 1
- in the job, `unshare --user` and `unshare --mount` both fail with "Operation not permitted"

Docker's default seccomp profile allows `unshare`, `setns`, `mount` and a `clone` that makes a new namespace only for a container with CAP_SYS_ADMIN (The Moby Project, n.d.). Every builder that needs its own user namespace or its own mounts is therefore refused on these runners, whatever GitLab supports. The container registry of the project is on.

## Options

Each option was tried on the HvA runners on 22 October 2026 with the configuration of its own documentation, in pipelines 586250, 586252 and 586254, except the last two, which change the runner instead of the job.

| Option | What it needs from the runner | Result on the HvA runners | Maintained |
|---|---|---|---|
| Docker in Docker (`docker:dind`) | Privileged mode | Fails: "mount: permission denied (are you root?)" | Yes |
| Buildah (`quay.io/buildah/stable`) | A user namespace | Fails: "Error during unshare(CLONE_NEWUSER): Operation not permitted" | Yes |
| Podman (`quay.io/podman/stable`) | A user namespace | Fails: "cannot clone: Operation not permitted" | Yes |
| BuildKit rootless (`moby/buildkit:rootless`) | A user namespace | Fails: "[rootlesskit:parent] error: failed to start the child: fork/exec /proc/self/exe: operation not permitted" | Yes |
| kaniko of Google (`gcr.io/kaniko-project/executor`) | Nothing, it builds inside the filesystem of the job itself | Builds, and pushes to the registry of the project | No, its repository is archived, last change on 3 June 2025 |
| The maintained fork of kaniko (`ghcr.io/osscontainertools/kaniko`) | Nothing, the same way | Builds | Yes, release v1.28.6 of 11 October 2026 |
| Ask the HvA to change the runners: a custom seccomp profile through `security_opt`, or privileged mode | The administrator of the HvA runners | Not tried, only the HvA can do it | Not applicable |
| A runner of our own on the foundation's server | A GitLab Runner the team registers on that server | Not tried | Not applicable |

What GitLab's own documentation says:

- Docker in Docker is its first way, and "Some approaches require privileged mode on the runner" (GitLab, n.d.-b).
- For a runner without privileged mode it names BuildKit and Buildah (GitLab, n.d.-b). BuildKit rootless is "a replacement for Kaniko" (GitLab, n.d.-a).
- It names the exact BuildKit error above: "This issue occurs because the runner seccomp profile blocks the system calls that rootless BuildKit requires." The fix it gives is the `security_opt` setting of the runner with a custom seccomp profile, and it warns not to use `seccomp:unconfined` (GitLab, n.d.-a).
- It removed its kaniko page: "kaniko is no longer a maintained project" (GitLab, n.d.-c).

## Decision

The maintained fork of kaniko, pinned as `ghcr.io/osscontainertools/kaniko:v1.28.6-debug`, in the jobs image-build and image-push of `.gitlab-ci.yml`. It is the only way that builds on the runners the team has, and unlike the kaniko of Google it still gets fixes. Decided by Wesley Oerlemans on 22 October 2026.

Why the other options lost:

- **Docker in Docker, Buildah, Podman and BuildKit rootless.** They are the ways GitLab documents, but each needs privileged mode or a user namespace, which the HvA runners refuse. They fail before they build anything.
- **kaniko of Google.** It builds, but it is archived and gets no security fixes any more, and the builder of every image is a part a hole must never stay open in.
- **Asking the HvA to change the runners.** It would let Buildah or BuildKit work, the ways GitLab maintains itself, but it depends on someone outside the team, and the images are needed in sprint 2. The question can still go to the HvA, and a yes changes this decision (see Consequences).
- **A runner of our own on the foundation's server.** The team could configure it, but every branch of every member would then run with the rights of Docker on the machine that holds the test site and production, and share its 7.7 GB of memory with them.

## Consequences

- **The Dockerfile stays standard.** A laptop builds it with `docker build`, Docker on the test site and k3s in production run the same images. Changing the builder later changes the two jobs, never the Dockerfile.
- **The fork is pinned.** An update is a merge request that changes the tag in `.gitlab-ci.yml` after reading the release notes of the fork.
- **The registry login.** kaniko has no login command, so image-push writes the login of the registry from `CI_REGISTRY_USER` and `CI_REGISTRY_PASSWORD` into the job's own config file. No password is in `.gitlab-ci.yml`.
- **Slower than a cache.** kaniko builds each image from the start, so the .NET part is built twice in one job. The minutes are in the merge request of task 20.3.
- **When the HvA changes the runners.** Then BuildKit rootless or Buildah replace kaniko in the two jobs, and this file gets a dated line that says so.
- **When the fork stops.** The fork is a community project with one main maintainer. When it gets no release for six months, the same check as above is done again, and the outcome is written here.

## Agreed by

| Name | Date agreed |
|---|---|
| Bayon Eerhart | |
| Nick Hoebe | |
| Wesley Oerlemans | 22 October 2026 |
| Gurpreet Singh | |
| Kelvin van Voorst | |

## References

GitLab. (n.d.-a). *Build Docker images with BuildKit*. GitLab Docs. Retrieved October 22, 2026, from https://docs.gitlab.com/ci/docker/using_buildkit/

GitLab. (n.d.-b). *Use Docker to build Docker images*. GitLab Docs. Retrieved October 22, 2026, from https://docs.gitlab.com/ci/docker/using_docker_build/

GitLab. (n.d.-c). *Use kaniko to build Docker images (removed)*. GitLab Docs. Retrieved October 22, 2026, from https://docs.gitlab.com/ci/docker/using_kaniko/

Google. (2025). *kaniko* [Computer software]. GitHub. https://github.com/GoogleContainerTools/kaniko

The Moby Project. (n.d.). *seccomp/default.json* [Computer software]. GitHub. Retrieved October 22, 2026, from https://github.com/moby/profiles/blob/main/seccomp/default.json

osscontainertools. (2026). *kaniko* (Version 1.28.6) [Computer software]. GitHub. https://github.com/osscontainertools/kaniko

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

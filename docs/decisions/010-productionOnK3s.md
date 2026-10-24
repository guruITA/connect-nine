# 010 How the live version runs: k3s set up with Terraform, while the test site stays on docker compose

**Status.** The facts behind the choice were checked by Wesley Oerlemans on 24 October 2026 in task 92.1 (#1042). The choice and its consequences are written in task 92.2 (#1043). The team confirms it at the stand up, and each member adds a line under Agreed by.
**Issue.** 92 k3s and Terraform for production, compose for test (#156), task 92.1 (#1042) and task 92.2 (#1043).
**Related decision.** [008 How the pipeline builds the images](008-kanikoForImages.md), which makes the images production runs.
**Who decides.** Wesley Oerlemans, who builds production (#157 to #163). The whole team confirms.

## Context

The HvA DevOps workshop asks that the live version runs on an orchestrator that restarts crashed parts, swaps in a new version without downtime and keeps a declared state (slide 8), that its setup is kept in files with the steps write, plan, review and apply (slide 9), and it names Kubernetes set up with Terraform as the example tools (slide 16) (Hogeschool van Amsterdam, 2026). The foundation did not ask for a live version.

What the client answered in issue 91 (#155), in `docs/client/clientQuestions.md`:

- Question 111, a live version at all and where: open.
- Question 112, one server or two: open.
- Question 113, the address: www.youngamstellandconnect.nl, answered on 23 October 2026. The foundation still has to buy the domain.

The server the foundation lends to the team, read on 24 October 2026 with commands that only read:

- Ubuntu 24.04.5 LTS with kernel 6.8.0-147, 4 processor cores.
- 7.7 GB of memory. Task 91.1 (#1040) measured 7.0 GB available on 22 October 2026, before the test site ran. With the test site running, 6.7 GB was available on 24 October 2026.
- Docker CE 29.8.2 with docker compose, which runs the test site. Caddy 2.11.7 holds ports 80 and 443 with the padlock.
- The firewall ufw lets in only ports 22, 80 and 443.

The workshop showed its demo with kind on a laptop, "entirely local, no cloud account" (slide 14). kind "was primarily designed for testing Kubernetes itself, but may be used for local development or CI" (Kubernetes SIGs, n.d.). It is a way to try the same objects on a laptop, not a way to run a live version, so it is not one of the options below.

Some facts below were measured on a copy of the server: a virtual machine with the same Ubuntu 24.04.5, kernel 6.8.0-147, Docker CE 29.8.2, Caddy 2.11.7 and ufw rules, with a stand in for the test site on `127.0.0.1:8080`. The steps and the output are in task 93.1 (#1044). Nothing was installed on the foundation's server.

## Options

### Three ways to run the live version

| | docker compose | k3s | MicroK8s |
| --- | --- | --- | --- |
| Restarts a crashed part | Yes, a restart policy such as `unless-stopped` per service restarts a container that stops (Docker, n.d.-b) | Yes, a Pod restarts its containers by default, `restartPolicy` Always (Kubernetes, n.d.-d), and a Deployment keeps the number of pods it declares (Kubernetes, n.d.-a) | Yes, the same Kubernetes objects, MicroK8s is "a minimal, lightweight Kubernetes" (Canonical, n.d.-a) |
| Swaps a new version without downtime | No, `docker compose up` picks up a change "by stopping and recreating the containers" (Docker, n.d.-a) | Yes, a rolling update replaces pods "with zero downtime by incrementally updating Pods instances with new ones" (Kubernetes, n.d.-c) | Yes, the same Deployment |
| Desired state in files | Partly, the compose file declares the services, and a change only applies when someone runs `docker compose up` (Docker, n.d.-a) | Yes, "You describe a desired state in a Deployment, and the Deployment Controller changes the actual state to the desired state" (Kubernetes, n.d.-a) | Yes, the same Deployment |
| Memory it asks for | No number on the site of Docker. It already runs the test site, so it asks for nothing new | At least 2 processor cores and 2 GB for a server (K3s, n.d.-d). A server with a workload and a Prometheus and Grafana stack used 1,596 MB in their test (K3s, n.d.-e). On the copy of the server the used memory rose by 632 MB after the install, before any part of Connect ran | "runs in as little as 540MB of memory, but to accommodate workloads, we recommend a system with at least 20G of disk space and 4G of memory" (Canonical, n.d.-a) |
| Runs on the operating system of the server | Yes, Docker Engine lists Ubuntu Noble 24.04 (LTS) (Docker, n.d.-c), and it runs there now | "expected to work on most modern Linux systems", with a note for Ubuntu on ufw (K3s, n.d.-d). It ran on the copy of the server with the same Ubuntu and kernel | Yes, on a Linux with snapd (Canonical, n.d.-a), and snapd 2.76.3 is on the server. Not tried |
| How it is installed | From the apt repository of Docker, on the server since task 9.3 (#238) | One install script per release, with `INSTALL_K3S_VERSION` for one exact version (K3s, n.d.-a). The script checks the binary against its sha256 and makes a systemd service | `sudo snap install microk8s --classic --channel=1.35` (Canonical, n.d.-a). A snap checks for updates and refreshes itself "four times per day" within its channel unless the refresh is held (Canonical, n.d.-b) |
| License | Apache-2.0 (Docker, 2026) | Apache-2.0 (K3s, 2026) | Apache-2.0 (Canonical, 2026) |

### Two tools to set it up: Terraform and OpenTofu

This part checks the Terraform choice of 27 September 2026.

| | Terraform | OpenTofu |
| --- | --- | --- |
| License | Business Source License 1.1 for version 1.6.0 and later, with the grant that "Hosting or using the Licensed Work(s) for internal purposes within an organization is not considered a competitive offering" (HashiCorp, 2026a) | MPL-2.0, under the Linux Foundation, "a drop-in replacement for Terraform" (OpenTofu, n.d.-a, 2026) |
| What that means for the team and the foundation | Free to use: the foundation runs its own live version, which is internal use. It is no longer an open source license: OpenTofu was started "in response to HashiCorp's switch from an open-source license to the BUSL" (OpenTofu, n.d.-b) | Free to use, under an open source license, with the project held by the Linux Foundation instead of one company (OpenTofu, n.d.-b) |
| The providers kubernetes and helm | Both MPL-2.0 (HashiCorp, 2026b, 2026c) | The same providers, through the registry of OpenTofu |
| GitLab | GitLab-managed state and the plan in the merge request work with both. "GitLab no longer distributes the Terraform CI/CD templates and terraform-images" (GitLab, n.d.-b) | GitLab's documentation "refers primarily to OpenTofu" and offers an OpenTofu CI/CD component (GitLab, n.d.-b) |
| The workshop | Named on slides 14 and 16: "Kubernetes (kind) provisioned declaratively with Terraform" | Not named, slide 14 says "your assignment can use any equivalent" |
| Newest version | 1.17.0, 21 October 2026 | 1.13.2, 21 October 2026 |

The check holds on what the card asks: Terraform is free for the foundation, GitLab keeps its state, and it is the tool the workshop names. Against it: its license is no longer open source, and GitLab's own pipeline parts now assume OpenTofu. Connect does not need those parts: the jobs of #158 use the image `hashicorp/terraform` of HashiCorp itself. OpenTofu reads the same files, so a later switch is a change of the image and a new lock file, which the team would try in one merge request.

### Where the state lives

- Operate, Terraform states, is on at gitlab.fdmci.hva.nl (GitLab 19.5.1). On 24 October 2026 the project had no state. A test state named `probe`, sent to the state address of the project, appeared under Terraform states and was deleted again, which left the project with no state.
- What GitLab says about it (GitLab, n.d.-a): it is in every tier, the state is encrypted at rest, locking and writing with an apply needs the role Maintainer or Owner, and reading with `plan -lock=false` needs Developer. All five students have the role Maintainer in the project.
- The fallback, not needed now, is the backend kubernetes: it "Stores the state in a Kubernetes secret" and locks it "using a Lease resource" (HashiCorp, n.d.).

### The front door

- k3s starts Traefik by default as its front door. Its settings are changed with a HelmChartConfig, because k3s rewrites its own Traefik file at every start (K3s, n.d.-b, n.d.-c).
- ServiceLB, the load balancer of k3s, "uses available host ports" for every Service of type LoadBalancer (K3s, n.d.-c).
- On the copy of the server, with ServiceLB off, a Service of type LoadBalancer stayed pending. Its NodePort, which Kubernetes gives every such Service, answered from outside although ufw allows only 22, 80 and 443. After kube-proxy was limited to 127.0.0.1 with `nodeport-addresses` (Kubernetes, n.d.-b), nmap from outside showed that port as filtered.
- The Kubernetes API on 6443 and the kubelet on 10250 listen on every address of the copy. ufw blocks them: nmap from outside showed both as filtered.
- The plan: Caddy keeps 80 and 443 with the padlock and passes the production address to Traefik on `127.0.0.1:8081`. Traefik runs on the host network and listens only on 127.0.0.1. On the copy, a visit with the production host name went through Caddy and Traefik to a pod, and the test site kept answering.
- When question 112 chooses a second server, that server gets Caddy the same way as task 9.4 (#239), and k3s with the same script.

### Why the test site and the laptops stay on docker compose

- The test site runs on docker compose, and its deploy (#267) is built and owned by Gurpreet Singh. Changing it is not part of this choice.
- Every laptop already runs Connect with `docker compose --profile app up` (#353), and the team knows docker compose.
- The test site needs no swap without downtime: a short stop after a merge into dev only touches the team.
- k3s would take about 0.6 GB on every laptop for something the team does not need while it builds.

### Facts that go against the choice

- The workshop showed its demo without any server. The school requirement alone does not need the foundation's server, so a live version there rests on question 111.
- Terraform is no longer open source, and GitLab no longer ships its Terraform templates and images. OpenTofu does the same job under an open license.
- k3s advises to turn ufw off (K3s, n.d.-d). The plan keeps ufw with the two rules for the pod and service networks from that same page, which worked on the copy.
- A NodePort passes around ufw. Without the limit to 127.0.0.1, every Service of type NodePort or LoadBalancer would be a door from the internet.
- Traefik on the host network cannot start a new pod while the old one holds the ports. On the copy, with the chart's default update settings, the new pod stayed pending, so an update of Traefik stops the old pod first and the front door inside k3s is away for seconds.
- k3s takes about 0.6 GB before any part of Connect runs, more than the test site itself uses.

### What is not known yet, and what the team will try

- How much memory production uses with Connect, its database and the monitoring. The team measures it with `kubectl top` once #159 runs, and gives each part a memory limit.
- Whether k3s on the real server behaves as on the copy. The install of #157 repeats the nmap from outside and the test site check there.
- Whether the ingress of MicroK8s would collide with Caddy. Not tried, because MicroK8s was not installed.

## Decision

## Consequences

## Agreed by

| Name | Date agreed |
|---|---|
| Bayon Eerhart | |
| Nick Hoebe | |
| Wesley Oerlemans | |
| Gurpreet Singh | |
| Kelvin van Voorst | |

## References

Canonical. (n.d.-a). *Get started*. MicroK8s. Retrieved October 24, 2026, from https://microk8s.io/docs/getting-started

Canonical. (n.d.-b). *Selecting a snap channel*. MicroK8s. Retrieved October 24, 2026, from https://microk8s.io/docs/setting-snap-channel

Canonical. (2026). *MicroK8s* [Computer software]. GitHub. https://github.com/canonical/microk8s

Docker. (n.d.-a). *docker compose up*. Docker Docs. Retrieved October 24, 2026, from https://docs.docker.com/reference/cli/docker/compose/up/

Docker. (n.d.-b). *Define services in Docker Compose*. Docker Docs. Retrieved October 24, 2026, from https://docs.docker.com/reference/compose-file/services/

Docker. (n.d.-c). *Install Docker Engine on Ubuntu*. Docker Docs. Retrieved October 24, 2026, from https://docs.docker.com/engine/install/ubuntu/

Docker. (2026). *Compose* [Computer software]. GitHub. https://github.com/docker/compose

GitLab. (n.d.-a). *GitLab-managed Terraform/OpenTofu state*. GitLab Docs. Retrieved October 24, 2026, from https://docs.gitlab.com/user/infrastructure/iac/terraform_state/

GitLab. (n.d.-b). *Infrastructure as Code with OpenTofu and GitLab*. GitLab Docs. Retrieved October 24, 2026, from https://docs.gitlab.com/user/infrastructure/iac/

HashiCorp. (n.d.). *Backend type: Kubernetes*. HashiCorp Developer. Retrieved October 24, 2026, from https://developer.hashicorp.com/terraform/language/backend/kubernetes

HashiCorp. (2026a). *Terraform* (Version 1.17.0) [Computer software]. GitHub. https://github.com/hashicorp/terraform

HashiCorp. (2026b). *Terraform provider for Kubernetes* [Computer software]. GitHub. https://github.com/hashicorp/terraform-provider-kubernetes

HashiCorp. (2026c). *Terraform provider for Helm* [Computer software]. GitHub. https://github.com/hashicorp/terraform-provider-helm

Hogeschool van Amsterdam. (2026). *Level 3 DevOps advanced workshop* [Course document].

K3s. (n.d.-a). *Environment variables*. Retrieved October 24, 2026, from https://docs.k3s.io/reference/env-variables

K3s. (n.d.-b). *Helm*. Retrieved October 24, 2026, from https://docs.k3s.io/add-ons/helm

K3s. (n.d.-c). *Networking services*. Retrieved October 24, 2026, from https://docs.k3s.io/networking/networking-services

K3s. (n.d.-d). *Requirements*. Retrieved October 24, 2026, from https://docs.k3s.io/installation/requirements

K3s. (n.d.-e). *Resource profiling*. Retrieved October 24, 2026, from https://docs.k3s.io/reference/resource-profiling

K3s. (2026). *K3s* (Version 1.36.5+k3s1) [Computer software]. GitHub. https://github.com/k3s-io/k3s

Kubernetes. (n.d.-a). *Deployments*. Kubernetes. Retrieved October 24, 2026, from https://kubernetes.io/docs/concepts/workloads/controllers/deployment/

Kubernetes. (n.d.-b). *kube-proxy*. Kubernetes. Retrieved October 24, 2026, from https://kubernetes.io/docs/reference/command-line-tools-reference/kube-proxy/

Kubernetes. (n.d.-c). *Performing a rolling update*. Kubernetes. Retrieved October 24, 2026, from https://kubernetes.io/docs/tutorials/kubernetes-basics/update/update-intro/

Kubernetes. (n.d.-d). *Pod lifecycle*. Kubernetes. Retrieved October 24, 2026, from https://kubernetes.io/docs/concepts/workloads/pods/pod-lifecycle/

Kubernetes SIGs. (n.d.). *kind*. Retrieved October 24, 2026, from https://kind.sigs.k8s.io/

OpenTofu. (n.d.-a). *OpenTofu*. Retrieved October 24, 2026, from https://opentofu.org/

OpenTofu. (n.d.-b). *FAQ*. Retrieved October 24, 2026, from https://opentofu.org/faq/

OpenTofu. (2026). *OpenTofu* (Version 1.13.2) [Computer software]. GitHub. https://github.com/opentofu/opentofu

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

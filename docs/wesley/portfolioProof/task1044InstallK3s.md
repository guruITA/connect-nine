# Proof: task 93.1 The script that installs k3s (#1044)

Branch `chore/1044-install-k3s`. The merge request opens once the script has run on the server itself (TODO below). Issue: 93 k3s on the production server (#157). Files: `infra/k3s/installK3s.sh`, `infra/k3s/config.yaml`, `infra/k3s/traefikPorts.yaml` and `docs/team/onboarding.md` (section Production server). The choice behind it: `docs/decisions/010-productionOnK3s.md`, merge request !102.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **Tried on a copy, never first on the server.** The foundation's server also runs the test site, and nothing of the live version may run there before the director has said yes. So I tried the script on 24 October 2026 on a virtual machine set up like the server: Ubuntu 24.04.5 with kernel 6.8.0-147, Docker CE 29.8.2, Caddy 2.11.7, ufw open on 22, 80 and 443 only, and a stand in for the test site on `127.0.0.1:8080`.
2. **A door found before it was open.** A Service of type LoadBalancer gets a NodePort, and on the copy that NodePort answered from outside although ufw blocks it. `config.yaml` now limits NodePorts to 127.0.0.1 and switches ServiceLB off. After that nmap from outside showed the NodePort as filtered, next to 6443, 8081 and 10250.
3. **A stuck update found before it happened.** With the chart's default update settings, a new Traefik pod stayed Pending with "1 node(s) didn't have free ports for the requested pod ports", because on the host network the old pod holds them. `traefikPorts.yaml` stops the old pod first, and a restart of Traefik then finished.
4. **A port taken by the test site.** Traefik's own health check port is 8080 in the chart, which is where the server of the test site listens. It moved to 9000, and 9100 stays free for the node exporter of the monitoring.
5. **It stops safely.** The script checks the sha256 of the install script before it changes anything: with a wrong checksum it stopped with "Nothing was changed", and no file and no ufw rule was written. A stop later on names the line. While the copy went through the uninstall, a fresh install and a second run, the test site answered all 802 checks, one every 0.2 seconds.
6. **Transferable.** One pinned version with the date it was chosen, a second run that changes nothing (`is up to date`, `Skipping adding existing rule`, `already installed`), shellcheck 0.11.0 without a remark, and the section Production server in the getting started guide: how to run it, what to see, the closed doors and how to install again, with the warning that the uninstall script also removes the database.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **The test site stays as Gurpreet Singh built it.** The script does not touch `deploy/`, Caddy or the test site, and Traefik never takes 80, 443 or 8080. On the copy the test site and a visit through Caddy and Traefik to a pod both answered after the install and after a restart of the machine.
2. **The firewall of task 9.3 (#238) stays as it is.** ufw stays on and gets two rules, only for the pod and service networks inside the server, and no rule for 6443 or 8081.
3. **The plan of the team.** Caddy stays the one front door, as `docs/architecture/infrastructure.md` describes, and the install on the server waits for question 111.

## TODO

- [ ] Wesley: after the director's yes to question 111, announce the install in the team channel and run the script on the server. Then paste the node line, the `ss` line and an nmap from a laptop in the section Production server, and check that the test site still answers.
- [ ] Wesley: open the merge request with this proof once the part on the server is done.
- [ ] A teammate who built no task of #157: task 93.3 (#1046) on a spare computer, following only the section Production server.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

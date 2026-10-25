# Proof: task 92.1 The facts behind production on k3s with Terraform (#1042)

Merge request !102, branch `docs/1042-production-on-k3s`. Issue: 92 k3s and Terraform for production, compose for test (#156). File: `docs/decisions/010-productionOnK3s.md`, the parts Context and Options. The parts Decision and Consequences are task 92.2 (#1043).
Profile: Full Stack Software Developer, level 3. Portflow: Analysing and Advising AA2 and AA4.

A point that needs someone else is marked TODO with who and what.

## AA2 Questioning sources and my own assumptions

School asks: "You show an inquisitive and critical attitude by questioning sources, AI results, the problem statement and your own assumptions."

1. **My own choice checked, not repeated.** I chose k3s and Terraform on 27 September 2026. Task 92.1 checks that choice again: every cell of the table comes from the maker's own site, with the date read in the reference list.
2. **The assumption that ufw closes every door.** On a copy of the server, a NodePort answered from outside although ufw allows only 22, 80 and 443. The decision writes it under Facts that go against the choice, and task 93.1 (#1044) limits NodePorts to 127.0.0.1, after which nmap showed the port as filtered.
3. **The problem statement itself.** The workshop shows its demo with kind on a laptop, "entirely local, no cloud account". So the school requirement does not need the foundation's server, and a live version there rests on question 111. That is written down as a fact against the choice.
4. **Against my own tool.** I read the license file of Terraform itself: Business Source License 1.1 since version 1.6.0. GitLab no longer ships its Terraform templates and images and writes its documentation for OpenTofu. Both are in the decision as facts against Terraform, next to why the choice still holds.
5. **Checked by trying, not only by reading.** Whether GitLab keeps the Terraform state on gitlab.fdmci.hva.nl: I sent a test state named `probe` to the project, saw it under Terraform states and deleted it again.
6. **What no source says.** Where a source gives no number, such as the memory Docker asks for, or where I did not try something, such as the ingress of MicroK8s, the decision says so and says what the team will try.

## AA4 Giving advice that weighs alternatives and risks

School asks: "You give well-founded advice in which you weigh alternatives, identify risks and make a substantiated recommendation."

1. **Three ways held against what the school asks.** docker compose, k3s and MicroK8s on seven rows: restarting a crashed part, a new version without downtime, desired state in files, memory, the operating system of the server, how it is installed and the license.
2. **Two tools held against what the team and the foundation need.** Terraform and OpenTofu on the license and what it means for a school team and a foundation, the providers, GitLab and the workshop.
3. **The risks of the choice, measured where possible.** k3s takes about 0.6 GB of memory before Connect runs. Traefik on the host network is away for seconds when it is updated, because the new pod cannot start while the old one holds the ports. A NodePort passes around ufw. Each is in the decision with what was measured on the copy of the server.
4. **The order of the card.** The recommendation is written in task 92.2 (#1043), after I read the facts that go against my own choice, which are in a comment on #156. Reading them, I asked whether the license of Terraform still allows a public website, and checked the answer in the license itself before I kept Terraform.

## TODO

- [x] Wesley: read the facts that go against the choice in the comment on #156, then task 92.2 (#1043) writes the parts Decision and Consequences, 25 October 2026.
- [ ] Bayon Eerhart, Nick Hoebe, Gurpreet Singh and Kelvin van Voorst: a line under Agreed by, after task 92.2.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

# Proof: task 92.2 The choice for k3s and Terraform written down with its consequences (#1043)

Merge request !102, branch `docs/1042-production-on-k3s`, together with task 92.1 (#1042). Issue: 92 k3s and Terraform for production, compose for test (#156). Files: `docs/decisions/010-productionOnK3s.md` (the parts In plain words, Decision and Consequences), one line in `docs/architecture/architecture.md`, `docs/architecture/infrastructure.md` and `docs/team/onboarding.md` each, linking it.
Profile: Full Stack Software Developer, level 3. Portflow: Analysing and Advising AA4 and AA5.

A point that needs someone else is marked TODO with who and what.

## AA4 Giving advice that weighs alternatives and risks

School asks: "You give well-founded advice in which you weigh alternatives, identify risks and make a substantiated recommendation."

1. **The choice held against what came up.** I chose k3s and Terraform on 27 September 2026. Before writing it down I read the facts that go against it, which task 92.1 put in their own part and in a comment on #156. The biggest was the license of Terraform, which is no longer open source. I checked in the license itself whether that forbids a public website: it forbids offering Terraform as a product that competes with IBM, and says internal use is not that. So on 25 October 2026 I kept Terraform, and the decision says when OpenTofu would be the better choice.
2. **Why every other option lost, not only why mine won.** docker compose cannot swap a version without a stop, MicroK8s updates itself four times a day as a snap, kind is for testing, OpenTofu adds nothing we need, and the state inside k3s would be lost with the server.
3. **Per consequence the risk it removes and the risk it adds.** For example: Traefik only on 127.0.0.1 removes a way around the padlock, which a NodePort was on the copy of the server, and adds a few seconds offline when Traefik itself is updated. The state in GitLab removes a state lost with the server, and adds a dependency on the GitLab of the HvA, which has to move after the school year.
4. **A risk measured, not assumed.** The decision says the swap without downtime is not complete yet: during a planned restart of the server one check of 378 got no answer, and the fix is named (#1051).
5. **After the school year, and when to go back.** Two sentences for the foundation: keep k3s while someone can read the guide and run a plan, and go back to docker compose when nobody who looks after Connect knows k3s.
6. **The other view.** How the test site deploys is Gurpreet Singh's choice (#355). The decision gives both ways, with what was measured on each, and says it changes nothing for the test site.

## AA5 Adapting your communication to each audience

School asks: "You adapt your communication to different target groups in form and language."

1. **For the client.** The part In plain words at the top: two versions on one server, why the live version runs differently, that the school asks for it, that it costs nothing, and that visitors only come in through the front door. No program name a client has to look up besides k3s and Terraform.
2. **For the team.** The parts Decision and Consequences with the exact versions, file names, logins, ports and the cards behind each.
3. **For whoever looks after it later.** The advice after the school year, in two sentences.

## TODO

- [ ] A teammate who did not write the file: answer from it alone why not docker compose in production, where the state lives and how a visit reaches production, with the answers in #156. Wesley: change the file where an answer was wrong.
- [x] The team agreed to k3s with Terraform when Nick Hoebe, Kelvin van Voorst and Gurpreet Singh approved the architecture documents in !4, merged on 2 October 2026, written under Agreed by on 25 October 2026.
- [ ] Bayon Eerhart: a line under Agreed by.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

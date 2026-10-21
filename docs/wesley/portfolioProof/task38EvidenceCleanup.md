# Proof: issue 106 The sprint 1 evidence documents cite only files in the repository (#38)

Merge requests !17 and !18, branch `docs/38-clean-wesley-evidence`, both merged on 8 October 2026. Issue: 106 Every student's research proposal in Portflow (#38). Files: `docs/wesley/securityAndArchitectureEvidence.md`, `docs/wesley/securityAndArchitecture.md` and `docs/wesley/securityFundamentals.md`, and for !17 also `docs/wesley/researchPlan.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1, Analysing and Advising AA2.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk.** My sprint 1 evidence for datapoint 4 cited "the server notes" on 28 lines. Those notes are not in the repository, so a teacher or a teammate who reads the evidence could check none of those 28 statements.
2. **What changed in !18.** Each of the 28 lines now cites a document of the team that holds the same fact: part 10 The client's server of `docs/team/onboarding.md` for the server (Strato in Germany, Ubuntu 24.04, Docker and Compose, the firewall and fail2ban, received and rebuilt on 2 October 2026), and `docs/architecture/infrastructure.md` for the front door, Caddy, and the options that lost. That is 20 lines in `securityAndArchitecture.md`, 4 in `securityAndArchitectureEvidence.md` and 4 in `securityFundamentals.md`. On dev on 21 October 2026 none of the three files names the server notes any more.
3. **What changed in !17.** The evidence documents quoted the datapoint sheet and BoKSA in the form that speaks to the student, such as "what you encounter". !17 removed the quoted criterion from the introduction of `securityAndArchitectureEvidence.md` and wrote the other quoted criteria in neutral words, in four files. No fact, source or AI use text changed.
4. **Kept transferable.** Every source the evidence names is now a file in the repository, so anyone in the team can follow a statement to where it comes from without asking me.

## AA2 Questioning sources and my own assumptions

School asks: "You show an inquisitive and critical attitude by questioning sources, AI results, the problem statement and your own assumptions."

1. **A source nobody else could read.** Notes that only I have are not a source a reader can question. !18 points each of the 28 statements to the team document that holds the same fact.
2. **My own reasoning replaced by the team's record.** Why Nginx lost to Caddy now comes from `docs/architecture/infrastructure.md`, where the team weighed the options, instead of from my own reasoning.
3. **An assumption corrected.** "Connect has no code yet" was not exact. The evidence now says what the repository held on 6 October 2026: only an example solution structure for authentication in `api/`, added on 2 October, and no slice of Connect.

## TODO

- [ ] A teammate: follow one statement of `securityAndArchitecture.md` that cites part 10 of `docs/team/onboarding.md`, and write in the review whether part 10 holds that fact.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

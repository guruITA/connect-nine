# Proof: task 91.1 The six production questions for the client (#1040)

Merge request !88, branch `docs/1040-production-questions`. Issue: 91 The director agrees where production runs (#155). File: `docs/client/clientQuestions.md`, the part Production.
Profile: Full Stack Software Developer, level 3. Portflow: Analysing and Advising AA5, Managing and Collaborating MS1.

A point that needs someone else is marked TODO with who and what.

## AA5 Adapting the communication to each audience

School asks: "You adapt form and language to different target groups."

1. **One point, two readers.** Question 112 asks the director whether the live version may share the server with the test site. For the team the same point is three technical facts: k3s needs 2 GB, every part of production gets a memory limit, and `docker stats` measures the test site once it runs (`docs/architecture/infrastructure.md`, part Memory on a shared server, and my comment on #156). For the director it is one choice with a price: the same server for nothing, or a second one for 20 euros a month, with the measured room in plain numbers next to it.
2. **Plain words for every technical part.** Every program in the table of question 112 says what it does for the live version before what it needs, such as "keeps the live version running and restarts it when it stops" for k3s. The output of `free -h` and `df -h` stays as measured, because a later team must be able to measure the same way, and a sentence under it says the same in plain numbers: 7.7 GB of working memory with 7.0 GB free.
3. **The options before the advice.** Every question gives the options and what each means for the foundation first, and the advice of the team last with its reason, so the director chooses and does not only read what the team wants.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risks, and the question that removes each.**
   - Too little memory on the server: question 112 shows the measured room next to what each program documents, and marks the numbers no documentation gives as unknown, with how the team measures them.
   - The test site slowed down by the live version: question 112 names the risk, and the advice keeps it small with a memory limit for every part.
   - Building on a server without the agreement of the foundation: question 111 asks first, and names what the team does when the answer is no or not now: a live version on a spare computer of the team, for school only, with no real data.
   - A mail password in an email: question 116 never asks for the password in writing. The director types it into GitLab during a call, and nobody of the team sees it.
   - Real data of young people under 18 before the privacy rules exist: question 114 advises made-up data until #24 is done.
2. **Measured, with the date and the source.** `free -h` and `df -h` on the server on 22 October 2026. The memory each program needs from its own documentation, read the same day: k3s 2 GB and 2 cores (K3s Project Authors, n.d.), Grafana 512 MB and 1 core (Grafana Labs, n.d.-a), and no minimum given by Prometheus (Prometheus Authors, n.d.) or by Loki for a small setup (Grafana Labs, n.d.-b). The price of a second server like the current one: 20 euros a month including VAT and 9 euros once (STRATO, n.d.).
3. **My choice of 22 October 2026.** Production and the test site both run on the foundation's one server, so the advice of the team under question 112 is the same server. The director still chooses: the question keeps both options with their price.
4. **Kept transferable.** The six questions are in the client's own document, the answers go in the table above them and in #1041, and every unknown says how a later team measures it.

## TODO

- [ ] A teammate: read the part Production as if you were the director, and write in the review which words were unclear, so I can replace them before it goes to the client.
- [ ] Wesley: send the link to the part Production to the client two working days before the meeting (#1041).
- [ ] A team member with sudo on the server: add the output of `sudo docker stats --no-stream` under question 112 once the test site of #267 runs.

## References

Grafana Labs. (n.d.-a). *Install Grafana*. Grafana documentation. Retrieved October 22, 2026, from https://grafana.com/docs/grafana/latest/setup-grafana/installation/

Grafana Labs. (n.d.-b). *Loki deployment modes*. Grafana Loki documentation. Retrieved October 22, 2026, from https://grafana.com/docs/loki/latest/get-started/deployment-modes/

K3s Project Authors. (n.d.). *Requirements*. K3s. Retrieved October 22, 2026, from https://docs.k3s.io/installation/requirements

Prometheus Authors. (n.d.). *Storage*. Prometheus. Retrieved October 22, 2026, from https://prometheus.io/docs/prometheus/latest/storage/

STRATO. (n.d.). *VPS Linux bestellen vanaf € 2*. Retrieved October 22, 2026, from https://www.strato.nl/server/vps-linux/

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

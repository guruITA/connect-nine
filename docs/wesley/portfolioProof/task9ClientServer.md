# Proof: tasks 9.2 to 9.5 The client's server (#237, #238, #239, #240)

Branch `chore/10-the-clients-server`. Issue: 09 Every team member on the client's server (#10). Files: `docs/team/onboarding.md` part 10, `deploy/Caddyfile`, `deploy/.env.example`, `.gitlab-ci.yml` job ssh-check.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating, MS1 and MS4.

All steps on the server were done on 9 October 2026. A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **A guessed password (9.2).** Password login is off for everyone. A try as nick with only a password answered `nick@31.70.66.186: Permission denied (publickey).` Five personal users, each with only their own keys from their GitLab profile, no password.
2. **A team that locks itself out (9.2).** Root login went to keys only, not yet to `no`: wesley is in the group sudo but has no sudo password yet, so switching root off now would leave nobody who can change the server. Each step was tested with a new login before the next: root with key, wesley with key, then the password refusal.
3. **Docker ports past the firewall (9.3).** Docker writes its own firewall rules, so a container that publishes a port on all addresses is open to the internet whatever ufw says. The rule that every container publishes only on 127.0.0.1 is in part Open ports. A scan from outside on 9 October 2026 found only port 22 answering.
4. **A certificate that runs out, a browser warning (9.4).** Caddy got a Let's Encrypt certificate by itself (issuer YE1, valid until 6 January 2027) and renews it. http answers 308 to https.
5. **Where a secret could leak (9.5).** The database password was made on the server with `openssl rand` and printed nowhere. The private deploy key went from a file into the GitLab variable and was then deleted from the laptop. The variables are protected, so a branch that is not protected gets no key. The fingerprints of the host keys matched the three in the onboarding before they went into SSH_KNOWN_HOSTS.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **For #267.** Caddy sends every visit to 127.0.0.1:8080, so the Connect server of #267 must listen there. The user deploy, `/srv/connect-test`, the three variables and the names in `/srv/connect-test/.env` are written in part The pipeline on the server.
2. **An open point for #259 and #267.** The connection string has three names today: the code on dev reads `ConnectionStrings:Authentication`, `docs/architecture/configuration.md` says `ConnectionStrings:Default`, and task 9.5 says `ConnectionStrings__Connect`. The server follows the task. The owners of #259 and #267 choose one name.
3. **The five signed lines.**
   - TODO each teammate: log in with your own user and write the date in part First login per team member.

## TODO

- [x] Wesley: make the three variables, then run ssh-check on dev after the merge. Done on 9 October 2026: ssh-check green in job 1342015 of dev pipeline 583960, and red on an unprotected branch in pipeline 583976.
- [x] Wesley: root access without a root login. Done on 9 October 2026: `PermitRootLogin no`, and wesley and gurpreet use sudo without a password (`/etc/sudoers.d/20-connect-admins`), because every user has a locked password.
- [x] Team: choose the second member of the group sudo. Gurpreet Singh, 9 October 2026.
- [ ] Teammates: first login and signed line, screenshot of the padlock on a phone (9.4 and 9.6).
- [ ] Owner of #267: read part The pipeline on the server and write your name and the date in #10.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

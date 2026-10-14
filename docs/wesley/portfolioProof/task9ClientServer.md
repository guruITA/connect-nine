# Proof: tasks 9.1 to 9.5 The client's server (#236, #237, #238, #239, #240)

Merge request !5, branch `docs/10-the-clients-server`, merged 5 October 2026 (task 9.1, #236). Merge request !34, branch `chore/10-the-clients-server`, merged 9 October 2026. Open merge request !42, branch `docs/1-sprint1-checklist-fixes`, adds to tasks 9.1, 9.2 and 9.3 (#236, #237, #238). Issue: 09 Every team member on the client's server (#10). Files: `docs/team/onboarding.md` part 10, `docs/client/serverHandover.md`, `deploy/Caddyfile`, `deploy/.env.example`, `.gitlab-ci.yml` job ssh-check.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating, MS1 and MS4.

All steps on the server were done on 9 October 2026. A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **A guessed password (9.2).** Password login is off for everyone. A try as nick with only a password answered `nick@31.70.66.186: Permission denied (publickey).` Five personal users, each with only their own keys from their GitLab profile, no password.
2. **A team that locks itself out (9.2).** Root login went to keys only, not yet to `no`: wesley is in the group sudo but has no sudo password yet, so switching root off now would leave nobody who can change the server. Each step was tested with a new login before the next: root with key, wesley with key, then the password refusal.
3. **Docker ports past the firewall (9.3).** Docker writes its own firewall rules, so a container that publishes a port on all addresses is open to the internet whatever ufw says. The rule that every container publishes only on 127.0.0.1 is in part Open ports. A scan from outside on 9 October 2026 found only port 22 answering.
4. **A certificate that runs out, a browser warning (9.4).** Caddy got a Let's Encrypt certificate by itself (issuer YE1, valid until 6 January 2027) and renews it. http answers 308 to https.
5. **Where a secret could leak (9.5).** The database password was made on the server with `openssl rand` and printed nowhere. The private deploy key went from a file into the GitLab variable and was then deleted from the laptop. The variables are protected, so a branch that is not protected gets no key. The fingerprints of the host keys matched the three in the onboarding before they went into SSH_KNOWN_HOSTS.
6. **A guide that does not match the server (9.2).** Writing part Logging in showed that the old line "a key you added on GitLab is a key that works here" was wrong: the keys were copied once on 9 October 2026, so a later key needs an admin. On 10 October 2026 the new steps were followed as written with the user wesley, from a laptop with an empty list of known hosts. The server showed `ED25519 SHA256:jZUye4ZjNvmKksJkDOtlQ94+O/jKnpsfWvzAhqj2Nbk`, the same as the guide, the login gave `wesley@connect`, and `ssh -i` worked too. The two errors in the guide were caused on purpose: a key the server does not know answered `wesley@31.70.66.186: Permission denied (publickey).`, and a wrong saved fingerprint gave `WARNING: REMOTE HOST IDENTIFICATION HAS CHANGED!` and stopped the login. Both texts match the guide word for word.
7. **A server inherited with somebody else's access (9.1, merge request !5).** The server given on 2 October 2026 had carried an earlier project's whole stack: Dokploy as a Docker Swarm with Traefik, Postgres and Redis, a leftover Strapi volume, container images that were not Connect's, six ssh keys that were not the team's, and root logins from six addresses since May 2026. Ports 3000, 8080 and 8081 were open to the internet. Wesley Oerlemans wiped it the same day: left the swarm, removed every container, image and volume, removed every foreign key from root's authorized_keys, closed those ports, added fail2ban and rebooted. Disk use dropped from 21 GB to 2.9 GB, and the scripts and the log of the rebuild stay on the server.
8. **Open ports and the Docker version written down, so the next person checks instead of asking (9.3, merge request !42).** Part Open ports now holds the `sudo ufw status verbose` output of 9 October 2026 and the installed `docker --version`, `docker compose version` and a run of `hello-world`, which closes both open lines of #238's checklist.
   - TODO Wesley: tick the two lines of #238 in GitLab now that part 10 holds both.
9. **The client told about key-only access, written but not yet sent (9.2, merge request !42).** Part 10 now says why there is no password login and that the client's own access to the server also needs a key. Nobody has sent this text to the client yet, and part Who logged in still has no `last -n 20` output next to the five signed first-login lines.
   - TODO Wesley: send the client the text on why password login is off, then tick that line of #237.
   - TODO teammates: paste `last -n 20` in part Who logged in next to your signed first-login line.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **For #267.** Caddy sends every visit to 127.0.0.1:8080, so the Connect server of #267 must listen there. The user deploy, `/srv/connect-test`, the three variables and the names in `/srv/connect-test/.env` are written in part The pipeline on the server.
2. **An open point for #259 and #267.** The connection string has three names today: the code on dev reads `ConnectionStrings:Authentication`, `docs/architecture/configuration.md` says `ConnectionStrings:Default`, and task 9.5 says `ConnectionStrings__Connect`. The server follows the task. The owners of #259 and #267 were to choose one name. On 14 October 2026 I decided that the one name is `ConnectionStrings:DefaultConnection`, the name the code and the tests read today, and merge request !42 changes `docs/team/onboarding.md`, `docs/architecture/configuration.md` and `deploy/.env.example` to it.
3. **The five signed lines.** Part Logging in walks a teammate through the first login step by step, with the fingerprint check and the two errors people meet most. Part A new laptop or a new key tells an admin how to add a key that was not on GitLab on 9 October 2026, so nobody has to ask how.
   - TODO each teammate: log in with your own user and write the date in part First login per team member.
4. **The owner and the licence list, so NFR3 is traceable (9.1, merge request !42).** The table The machine now names the WailSalutem Foundation as owner, and part Open ports lists Docker Engine, the Compose plugin, Caddy and ufw with their licence, Apache 2.0 or GPL 3.0, so hosting costs no licence money.
   - TODO Wesley: paste the written request to the client, the three questions of #236, in #236. Only its date and the answer are recorded there now.
   - TODO the client: set the DNS record of the test address under the foundation's domain, then Wesley runs nslookup and pastes the answer in #236.
5. **For #24 and #23 (9.1).** The country of the data centre, Germany inside the EU, is the fact #24 needs for its privacy rules. The memory figure, 7.7 GiB, is the fact #23 needs for its log size limit.
   - TODO Wesley: tell the owners of #24 and #23 at the stand up that these facts are in part The machine, and write who and when here.

## TODO

- [x] Wesley: make the three variables, then run ssh-check on dev after the merge. Done on 9 October 2026: ssh-check green in job 1342015 of dev pipeline 583960, and red on an unprotected branch in pipeline 583976.
- [x] Wesley: root access without a root login. Done on 9 October 2026: `PermitRootLogin no`, and wesley and gurpreet use sudo without a password (`/etc/sudoers.d/20-connect-admins`), because every user has a locked password. On 10 October 2026 bayon, nick and kelvin got the same rights, so every team member is an admin. The old file is kept as `/root/20-connect-admins.bak-20261010`, and `visudo -c` parsed the new one before it was put in place.
- [x] Team: choose the second member of the group sudo. Gurpreet Singh, 9 October 2026.
- [ ] Teammates: first login and signed line, screenshot of the padlock on a phone (9.4 and 9.6).
- [ ] Owner of #267: read part The pipeline on the server and write your name and the date in #10.
- [ ] Wesley: send the client the text on why password login is off and that his own access needs a key, then tick that line of #237.
- [ ] Teammates: paste `last -n 20` in part Who logged in (#237).
- [ ] Wesley: tick both lines of #238 in GitLab, now that part 10 holds the ufw output and the Docker version.
- [ ] Wesley: paste the written request to the client, the three questions of #236, in #236.
- [ ] The client: set the DNS record of the test address, then Wesley runs nslookup and pastes the answer in #236.
- [ ] Wesley: tell the owners of #24 and #23 at the stand up that the data centre country and the memory figure are written in part The machine.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

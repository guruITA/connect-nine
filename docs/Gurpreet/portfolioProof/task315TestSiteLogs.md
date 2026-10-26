# Proof: task 17.5 The log of every container of the test site in a few files of fixed size (#315)

Merge request: still to open, branch `feature/315-test-site-logs` into `dev`. Task [#315](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/315) of issue [17 Connect says plainly whether it is healthy (#23)](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/23).
Profile: [Full Stack Software Developer](https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/), level 3. Portflow: [Managing and Collaborating](https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/manage-and-collaborating/), MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## What I make, in short

Docker keeps the log of every container of the test site in a file on the server. Without a limit that file only grows. This task gives every container of the test site a limit of five files of 10 MB, so the log can never fill the disk, and the oldest lines go on their own.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and keep the work transparent and transferable."

1. **The risk, seen before it happened.** On 26 October 2026 at 21:38 UTC I looked on the server, read only:
   - `/etc/docker/daemon.json` does not exist, so Docker uses `json-file` without any limit.
   - `docker inspect` on both containers gave `{"Type":"json-file","Config":{}}`: no `max-size`, no `max-file`.
   - The disk had 7.0 GB of 232 GB in use, so nothing was wrong yet.

   A log without a limit grows until the disk is full. PostgreSQL then can no longer write and stops, so the client loses Connect because of our own log. Since #312 every visit writes a JSON line, and a loop of errors writes many lines per second, so "the disk is large" is no protection.
2. **How the limit removes it.** `deploy/docker-compose.test.yml` has one block `x-logging` (driver `json-file`, `max-size: "10m"`, `max-file: "5"`), and `database`, `migrations` and `server` use it with `logging: *logging`. Docker starts a new file at 10 MB and removes the oldest when a sixth would come, so one container keeps at most 50 MB. `docker compose config` on my laptop showed the limit under all three services.
3. **Transferable.** Part 12 of `docs/team/onboarding.md`, section How long lines stay on the test server: the limit and why, that a new service gets `logging: *logging`, how to check it with `docker compose config` and `docker inspect`, and how to read the log with `sudo docker compose ... logs server --since 1h`. The block is one anchor at the top of the file, so nobody has to copy the numbers.
4. **What I found on the way.** `deploy/deployTest.sh` makes a new container for the server at every deploy (`--force-recreate`), and Docker removes the log of the old container with it. So the log of the server never holds more than the time since the last deploy. I wrote in the onboarding to read the lines of a bug before the merge that fixes it, or copy them into the issue.
5. **How many days 50 MB keeps (level 3).** On 26 October 2026 the log of the server grew 30,051 bytes between the deploy at 20:12 and 21:38 UTC, with a deploy and the browser tests of e2e-test in it. The log of the database grew 16,499 bytes in four days. The number for a normal day follows from the measurement after one day (TODO below).

   My judgment so far: at this pace 50 MB holds months of lines, longer than needed to find a bug. For the data of minors that is fine, because #312 keeps personal data out of every line: an account is a uuid, never an email address, and no query string, header or body is logged. The limit in practice is the next deploy, which empties the log of the server. If the day measurement shows that 50 MB is far more than a month, I propose a smaller `max-size` at stand-up, so the log never stays longer than the one month the client chose on 23 October 2026 for the address of a parent.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **The rule for new services.** TODO below: tell the stand up that every service added to `deploy/docker-compose.test.yml` gets `logging: *logging`, and write the date and who were there here.
2. **It fits the logs of #312.** #312 writes one JSON line per visit with the request id. `json-file` keeps the lines as they are, and `docker compose logs` still shows them, so finding one visit by `RequestId` (part 12) works the same.
3. **It fits #167 later.** #167 puts all log lines of Connect in one place. A collector reads the files of `json-file` or the output of `docker compose logs`, and reads them faster than 10 MB fills up, so the limit only decides how far back the server itself can look. The live version on k3s (!103 to !113 of Wesley) has the same kind of limit from the kubelet, whose defaults are also 10 MiB per file and five files (`containerLogMaxSize`, `containerLogMaxFiles`, https://kubernetes.io/docs/reference/config-api/kubelet-config.v1beta1/).

## TODO

- [ ] Me: after the merge, `sudo docker inspect -f '{{json .HostConfig.LogConfig}}'` on `connect-test-server-1` and `connect-test-database-1`, and paste the output here.
- [ ] Me: one day after the deploy, the size of the log file of the server, and the number of days 50 MB keeps, here and in part 12 of `docs/team/onboarding.md`.
- [ ] Me: tell the stand up the rule for new services, with the date and who were there in MS4 point 1.
- [ ] A teammate: read ten log lines on the test server with only the steps of part 12, and write their name and the date here.
- [ ] Me: the OR5 line of `task312JsonLogging.md` on how long lines stay, from this file.
- [ ] Me: open the merge request and link it and this file in Portflow under MS1 and MS4.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

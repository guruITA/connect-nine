# Proof: task 20.5 Run all of Connect with one command (#356)

Merge request !93, branch `chore/20-compose-all-of-connect`, together with task 20.2 (#353). Issue: 20 Connect starts with one command on any computer (#32). Files: `docs/team/onboarding.md` (part 13 Run all of Connect with one command), `README.md`.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **Who it is for.** The person who keeps Connect running after January 2027, who never saw the project and cannot ask the team. So part 13 uses no words of the team, gives a command in its own block, and says after each one what you should see.
2. **The three problems, and why these.** A port that is taken, because the team met it itself with port 5432 (part 3 of the guide holds that story). Docker that is not running, because Docker Desktop does not start by itself on every computer. And a password changed after the first start, because part 3 already warns about the same trap for the database alone. I made the first and the third happen with Docker on 22 October 2026 and copied the message from the screen, and the second from `docker compose` with no Docker to talk to.
3. **Checked by following it.** On 22 October 2026 I followed part 13 as written in a fresh clone, with the branch of #267 merged in on my machine only, because the screen is served by #267: the home page and `/create-account` answered, the first account was made (201), `whoami` answered `app`, the account was still there after stop and start, and `down -v` emptied it. The `ps` step first showed no migrations, because `ps` hides a container that stopped, so the guide now says `ps -a`.
4. **What I left out on purpose.** pgAdmin, the server without Docker, the tests and the test site. Those are parts 3, 5, 6 and 10 of the same guide, for the team, and a person who only wants Connect running does not need them.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **Next to #260 and #267, not instead of them.** Part 3 of the guide stays the way to start only the database for development, and part 13 is the way to run all of Connect. The test site of #267 is part 10. Part 13 repeats neither and points to no team word.
2. **Checked against the files.** The command, the profile `app`, the port 8080 and `SERVER_PORT` come from `docker-compose.yml` and `.env.example` of task 20.2, the user `app` and the three services from the Dockerfile of !89, the password rule of fifteen characters with a digit, a capital, a small letter and a sign from Nick's `CreateAccountRequestValidator`.
3. **The card and the form.** The card asks for a password typed twice. The form of #264 asks for it once, so part 13 says once. That is Nick's form and his choice.
4. **The README.** One line that links to part 13. The README is nine lines, within the ten the card allows.

## TODO

- [ ] A teammate who did not write it: follow part 13 alone on your own laptop, write a line in its History, and tell me in the review where you got stuck.
- [ ] Wesley: the walkthrough on a clean computer of task 20.6 (#357), with its minutes in the History.

## References

Docker Inc. (n.d.). *Docker Desktop license agreement*. Docker Docs. Retrieved October 22, 2026, from https://docs.docker.com/subscription/desktop-license/

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

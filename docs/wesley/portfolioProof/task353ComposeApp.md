# Proof: task 20.2 All of Connect with docker compose --profile app (#353)

Merge request !93, branch `chore/20-compose-all-of-connect`, built on !89 (#352, #354), together with task 20.5 (#356). Issue: 20 Connect starts with one command on any computer (#32). Files: `docker-compose.yml` (the services migrations and server), `.env.example` (`SERVER_PORT`).
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **Migrations before the database is ready.** The service migrations waits for the healthcheck of the database (`condition: service_healthy`), and the server waits until the migrations stopped with exit code 0 (`condition: service_completed_successfully`). So the server never starts against tables that are not up to date, and a failed migration leaves the server stopped instead of half working.
2. **A change that slows down every developer.** Both new services have the profile `app`, so the daily `docker compose up -d` of #260 still starts only the database and pgAdmin, and nobody waits for a build they did not ask for.
3. **Measured with Docker on 22 October 2026, on Ubuntu 24.04 with Docker 29.1.3, on a project of its own (`-p`).** The plain `up -d` started only the database and pgAdmin. `--profile app up -d --build` ran the migrations (exit 0) and the server answered `{"status":"healthy"}` on port 8080. An account made through `POST /api/accounts` (201) was still there after `down` and `up`: the same email then gave 409. After `down -v` and `up` the same email gave 201 again, so the copy was empty.
4. **The errors a person meets, measured too.** A port that is taken stops the start with "failed to bind host port 127.0.0.1:18080/tcp: address already in use". A password changed in `.env` after the first start stops the migrations with "28P01: password authentication failed for user", because the database keeps the first password on its volume. Both are in part 13 of the getting started guide (#356) with their fix.
5. **A false error found on the way.** Running the images this way showed "Error: libgssapi_krb5.so.2" in the log at every first connection, although the connection worked. I fixed it in the Dockerfile of !89, see the proof of task 20.1.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **The daily command of #260 still works as before.** `docker-compose.yml` is Gurpreet's file. I only added: two services below his pgAdmin, two lines in his list of commands at the top, and one setting in `.env.example`. His database and pgAdmin services are unchanged, and `docker compose up -d` started exactly those two, measured above. I told him on #353 what I added, as additions next to his services.
2. **The same way as the test site.** The connection string is built from the same `.env` values as his database, with `Host=database`, and the server runs as Staging, the same environment as `deploy/docker-compose.test.yml` of #267. The images are the ones of !89, the same the pipeline builds.
3. **Written in his style.** Every new setting has a comment and the documentation page it comes from, as every setting of #260 has.
4. **For a teammate who adds a service later.** A new service of a later issue goes below server with `profiles: ["app"]` and a `depends_on` on what it needs, so the plain `up -d` stays as it is.
5. **The card and the code.** The card names `ConnectionStrings__Connect` and a healthcheck of five seconds. The code on dev reads `ConnectionStrings:DefaultConnection`, as Nick built it and the team agreed, and Gurpreet's healthcheck asks every two seconds, which I left as it is.

## TODO

- [ ] A teammate: run `docker compose --profile app up -d --build` on your own laptop, stop and start it, and write your name, your system and what you saw in the review.
- [ ] Gurpreet: say on #353 or in the review whether the additions fit the way he wants `docker-compose.yml`.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

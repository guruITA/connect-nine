# Proof: task 20.1 The Dockerfile of the images server and migrations (#352)

Merge request !89, branch `chore/20-dockerfiles-for-the-server-and-the-frontend`, together with task 20.3 (#354). Issue: 20 Connect starts with one command on any computer (#32). Files: `Dockerfile`, `.dockerignore`, `docs/team/onboarding.md` (part The images of Connect).
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risks the recipe removes.**
   - A server that only starts on one laptop: the image holds the published server, the built screen and nothing of the laptop it was built on, and the pipeline builds it from a clean clone on every merge request.
   - A test site that runs something else than the laptops: the test site and production start the images of the merged commit, built from the same `Dockerfile` a laptop uses.
   - A secret baked into an image: no password and no real connection string is in the `Dockerfile` or an image. The connection string comes in as `ConnectionStrings__DefaultConnection` when a container starts. The one value in the file, `Host=bundle;Database=bundle`, only lets `dotnet ef` make the DbContext while it builds the bundle, connects with nothing and lives for that one command. `.dockerignore` keeps `.env` out of the build context.
2. **The option I did not choose.** Two images with a separate web server for the screen, such as nginx next to the server. The server of #267 serves the screen itself under the one address of the test site, so one image needs no second server, no second container and no second route in Caddy. I weighed it again on 22 October 2026, because production runs on Kubernetes, where a separate screen could get its own number of copies. I kept the screen in the server image. After the build the screen is only files, which the server hands out at almost no cost, so a copy of the server brings the screen along for free. All of production runs on the one server of the foundation, so a second copy of anything adds no capacity there. What does need to scale apart, the server and the database, is already apart: the database runs as a StatefulSet of its own and the server as its own Deployment (#159). A separate screen would have cost a third image, a route for `/api` in the Ingress and on the test site, and taking the screen out of the server of #267 again.
3. **The user app.** The image configuration of `mcr.microsoft.com/dotnet/aspnet:10.0` in Microsoft's registry, read on 22 October 2026 (.NET 10.0.13), sets `APP_UID=1654` and `ASPNETCORE_HTTP_PORTS=8080` but no user, so a container runs as root unless the Dockerfile asks otherwise. Both final stages therefore end with `USER $APP_UID`, the user `app` the image already has.
4. **The sizes, and what they changed.** Built with Docker 29.1.3 from a clean clone of the branch on 22 October 2026: `server` 391 MB and `migrations` 533 MB. The migrations image was the bigger one, because its bundle is self-contained and carries its own .NET on top of the ASP.NET of `aspnet:10.0`. On `runtime-deps:10.0`, which holds only what a self-contained program needs, the same image is 373 MB and applied both migrations the same way, so the stage migrations uses it now. That is the one place where the `Dockerfile` differs from the card, which names `aspnet:10.0` for both.
5. **Checked as it runs.** In one Docker network with PostgreSQL 16 as `database`, on 22 October 2026: `whoami` answers `app` in both images, `migrations` applied both migrations and stopped with exit code 0, and `server` answered `{"status":"healthy"}` on `/api/health` on port 8080 with `Host=database`. The home page gives 404 on this branch alone, because serving the screen comes with #267. With the branch of #267 merged into it on my machine only, the same image answered the home page (200, title Connect), a deep link such as `/create-account` (200) and an unknown address under `/api` with the 404 of #267.
6. **A false error in the log, found with Docker Compose.** Every first connection to the database wrote "Error: libgssapi_krb5.so.2: cannot open shared object file" in the log of both images, although the connection then worked. Npgsql tries a GSSAPI connection first by default (Npgsql, n.d.), and the .NET 10 images do not hold that library. A person who keeps Connect running after January 2027 would read that line as a failure, so both final stages now install `libgssapi-krb5-2`. Measured on 22 October 2026: no such line left in either log, the migrations still applied, and the images grew by 4 MB each, to 395 MB for `server` and 377 MB for `migrations`.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **The same steps as #267.** The stage server-build publishes `api/Connect/Connect.csproj`, and the stage server copies the built screen into `wwwroot`, where `UseStaticFiles()` of #267 finds it. The migration bundle comes from exactly the command and the placeholder connection string of the job deploy-test of #267, so the test site of Gurpreet and these images apply the migrations the same way.
2. **The versions of the team.** Node.js 22 from part 1 of `docs/team/onboarding.md` as `node:22-alpine`, the image frontend-build of the pipeline uses. .NET 10 from decision 003 as `sdk:10.0` and `aspnet:10.0`. dotnet-ef in the version pinned in `.config/dotnet-tools.json` (10.0.12), through `dotnet tool restore`.
3. **For a teammate who adds a project.** One line in the `Dockerfile` says it: a new project gets one COPY line for its `.csproj` in the stage server-build.
4. **The card and the code.** Task 20.1 names `Presentation.csproj`, `ConnectionStrings__Connect` and `Connect.Presentation` as startup project. The team agreed that Nick's .NET code sets the way, and on dev the startup project is `api/Connect` and the connection string is `ConnectionStrings:DefaultConnection`, so the `Dockerfile` follows the code. Measured on PostgreSQL 16 on 22 October 2026: the bundle with only `--connection` stops, because the startup project reads `DefaultConnection` itself, and with `ConnectionStrings__DefaultConnection` set it applied both migrations.
5. **The same way as the test site of #267.** The card asks for `./efbundle --connection "$ConnectionStrings__Connect"`. The migrations of `deploy/docker-compose.test.yml` of #267 run the bundle without `--connection` instead, so the password never stands on a command line that `ps` shows on the host. The image `migrations` now does the same: `ENTRYPOINT ["./efbundle"]`, which reads `ConnectionStrings__DefaultConnection` from the environment. Checked with Docker on 22 October 2026: both migrations applied, exit code 0.
6. **Checked in the pipeline.** The job image-build of !89 builds both targets with kaniko from the clean clone of the merge request, see the proof of task 20.3.

## TODO

- [ ] A teammate with Docker: build both images from a clean clone (`docker build --target server .` and `--target migrations .`), run `whoami` in each (it answers `app`) and `docker image ls`, and write the output and the sizes in the review, with your name.
- [x] Wesley: the check of the home page on port 8080 in one network with `Host=database`. Done on 22 October 2026 with Docker on my machine, MS1 point 5. The home page itself answers once #267 is merged. The compose services of task 20.2 (#353) make the same check one command.

## References

Npgsql. (n.d.). *Security and encryption*. Npgsql documentation. Retrieved October 22, 2026, from https://www.npgsql.org/doc/security.html

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

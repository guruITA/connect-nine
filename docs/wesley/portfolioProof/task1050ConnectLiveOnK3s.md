# Proof: tasks 95.1 to 95.3 Connect live on k3s, its database, its server and its address (#1050, #1051, #1052)

Branch `chore/159-connect-in-production`, built on `chore/1047-terraform-folder` (task 94.1). The merge request opens with the jobs that plan and apply in the pipeline (TODO below). Issue: 95 Connect and its database described in Terraform (#159). Files: `infra/k3s/applySecrets.sh`, `infra/terraform/connectDatabase.tf`, `connectMigrations.tf`, `connectServer.tf`, `connectIngress.tf` and `variables.tf`, `deploy/Caddyfile`, and `docs/team/onboarding.md` (part 4 Production secrets, part 14 The live version). The live version: https://live.31-70-66-186.sslip.io.
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1 and MS4.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **No password in a file, the state or a log.** The password of the production database is 32 random characters, made in memory and stored only as the masked and protected GitLab variable `DATABASE_PASSWORD_PRODUCTION` and as Kubernetes Secrets. k3s pulls the images with a deploy token that may only read the registry, also stored masked and protected. `applySecrets.sh` writes each value to a file with rights 600 before kubectl reads it, so no value stands on a command line that `ps` can show, and it never prints one. Terraform only names the Secrets.
2. **A token on the disk, so it was made new.** While I built the login files, the tokens of the two pipeline logins were for under a minute in a folder on the disk of my computer, not in memory. A deleted file on a disk cannot be trusted to be gone, so I made both logins new. The old tokens then answered "Unauthorized". A token of a login stays the same when only its Secret is made again, which I measured, so I made the logins themselves new.
3. **The data of young people is protected against a mistake.** The database has its own volume of 10 GB and `prevent_destroy`, so Terraform refuses a plan that would remove it. I removed the database pod on purpose: k3s started a new one within five seconds, the three migrations were still there and the server was never restarted.
4. **Visitors only reach a server that answers.** The server is ready only when `/api/health` says healthy, and is restarted only when port 8080 stops answering, never on a database that is away for a moment. Every part has a memory and processor limit, so the live version cannot push the test site off the shared server. On 25 October 2026 the server used 64Mi and the database 19Mi.
5. **It worked before anyone could visit it.** The plan said 7 to add, the apply ended with "Apply complete! Resources: 7 added, 0 changed, 0 destroyed" in 44 seconds, the migrations Job ended Complete, and `/api/health` through Traefik answered healthy before Caddy got the address. Then https://live.31-70-66-186.sslip.io answered healthy with a certificate of Let's Encrypt, http went to https with 308, and the test site answered all 578 checks of its health address, one every half second, during the apply and the change of Caddy. A plan after it said `No changes`, with the state `production` in GitLab.
6. **Transferable.** Part 14 of the getting started guide says where the address, the database and the server are described, the order of Secret, Job and Deployment, why the two probes differ, the limits with what was used, how to make a new password, and how the address changes when the foundation's domain comes.

## MS4 Making the work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **The test site stays as it is.** Caddy got a second block for the live address, and the block of the test site did not change. The live version has its own database and its own Secrets, never those of the test site.
2. **The images of the pipeline.** Production runs the images that image-push of task 20.3 (#354) puts in the registry, the same Dockerfile and the same version number as on dev, so what runs live is what the pipeline built.
3. **The agreed decision.** It follows `docs/decisions/010-productionOnK3s.md`: Caddy is the one front door, Traefik listens only on 127.0.0.1, and no real data goes in before the director's yes.
4. **What the code does today.** The server reads `ConnectionStrings__DefaultConnection`, as `api/Connect.Infrastructure/DependencyInjection.cs` does, not the name on the card. The volume for uploads and the setting of the forwarded headers wait for #134 and #57, which are not built yet.

## TODO

- [ ] Wesley: the jobs `terraform-plan` and `terraform-apply` of task 94.2 (#1048), with `applySecrets.sh` before the apply and the image version from the pipeline, then the merge request with this proof.
- [ ] Wesley: image-push also on main, so a merge into main puts its own version live (#162).
- [ ] A teammate who built none of #159: task 95.4 (#1053), no password shows, a crashed server comes back and the data survives a restart of the database.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

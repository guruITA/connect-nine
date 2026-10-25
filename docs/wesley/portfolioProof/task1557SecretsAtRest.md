# Proof: task 93.4 The Secrets of production encrypted before k3s writes them to its database (#1557)

Merge request !103, branch `chore/1044-install-k3s`, together with task 93.1 (#1044). Issue: 93 k3s on the production server (#157). Files: `infra/k3s/config.yaml` and `docs/team/onboarding.md` (section Production server, part Secrets at rest).
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating MS1.

A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk, counted instead of assumed.** On 25 October 2026 `k3s secrets-encrypt status` answered "Disabled, no configuration file found". I counted in the database file of k3s, without printing any value, how often the marker of an encrypted Secret and the key names of our Secrets appear: the marker 0 times, `POSTGRES_PASSWORD` 3 times. So the password of the production database, the login of the registry and the tokens of the pipeline were in that file without encryption, and a copy of the file alone was enough to read them.
2. **What it protects, and what it does not.** Encryption protects a copy of the database file without its key. It does not protect against root on the server, the login `gitlab-deployer`, the running server, or a copy of the whole disk, because the key is on the same disk. I wrote both lists in the guide, so nobody reads more into it than it does.
3. **On a copy first, with the undo.** On the virtual copy of the server I ran the steps the k3s documentation gives for an existing cluster, then turned encryption off and on again, then removed k3s and installed it again with the new `config.yaml`. The test site and the production name of the copy answered all 584 checks during the first steps and all 232 during the undo, and a Secret made after the fresh install was stored encrypted from the start.
4. **What the documentation did not say.** Right after the restart, `k3s secrets-encrypt status` answered "secret-encrypt error ID 90149". The log said the node had no annotation yet, and a few seconds later it answered as expected, so the guide says to ask again. More important: after the re-encryption the database still held the passwords without encryption. I read the database of k3s with a small read only script that prints only names, revisions and true or false. The database keeps the version before each change next to the new one, so the newest, encrypted copy of every Secret still carried its old unencrypted version, and the cleanup of k3s never removes the newest copy. On the copy a second key rotation wrote every Secret again with an encrypted version before it, so the guide has that as step 6.
5. **On the server.** I saved the old `config.yaml` with the undo under `/root/`, then ran the six steps. `k3s secrets-encrypt status` says Enabled, reencrypt_finished and All hashes match, and the newest copy of each of the 12 Secrets holds no unencrypted value. A new pod of `connect-server` started with the encrypted Secrets and `/api/health` answered healthy. The two pods of Connect were not restarted by the restarts of k3s. The test site answered all 605 checks of its health address during the steps, and the live version all but one: that one fell at the moment the old server pod stopped, in the restart of that pod I did on purpose afterwards (TODO below). After the cleanup of k3s at 18:18, the database holds no row with an unencrypted Secret, and the key names `POSTGRES_PASSWORD` and `.dockerconfigjson` and the start of a pipeline token appear 0 times in the database file and its journal, counted by a read only script.
6. **Transferable.** A new install starts encrypted, because `config.yaml` has `secrets-encryption: true`. The part Secrets at rest gives how to check it, the six steps for an existing cluster, how long the old copies stay, the undo, and the warning not to change the key by hand.

## TODO

- [x] Wesley: count again after the cleanup of k3s that no Secret is left in the database file without encryption, 25 October 2026, 18:20.
- [ ] Wesley: the restart of `connect-server` that proved the encrypted Secrets work left one check of the live version without an answer, at the moment the old pod stopped. That is a gap of #1051, not of the encryption: a short wait before the old pod stops, so Traefik stops sending visits to it first (comment on #1051).

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

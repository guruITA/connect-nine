# Proof: task 16.4 The security scan green again after two new advisories (#1555)

Merge request !99, branch `chore/1555-npm-audit-fix`. Issue: 16 A building block with a known security hole is stopped (#22). File: `web/package-lock.json`.
Profile: Full Stack Software Developer, level 3. Portflow: Designing and Realising OR4.

A point that needs someone else is marked TODO with who and what.

## OR4 Validating the solution

School asks: "You systematically validate that the solution does what it should, and draw conclusions for improvement."

1. **The red run.** On 24 October 2026 frontend-scan of my merge request !98 turned red (pipeline 587409, job 1351396), on a change that only touched documents. `npm audit --audit-level=high` named two packages with a high advisory: undici 7.30.0 (GHSA-4qv8-2p9j-2685, GHSA-f72v-6w45-q472 and GHSA-xjj6-f33q-929c) and zod 4.6.5 (GHSA-mrhx-c7xc-m3w5). The pipeline of dev on 23 October 2026 at 16:46 (587254) was still green, so the advisories were published after it.
2. **Where they come from.** `npm ls undici zod`: undici through jsdom, the browser stand-in of the screen tests, and zod through eslint-plugin-react-hooks. Both are tools of the team and are not part of the screen a member loads.
3. **The fix.** As part 11 The security scan of `docs/team/onboarding.md` says for a red scan: update the package to a version without the advisory. `npm audit fix` in `web` changed only `web/package-lock.json`: undici 7.30.1 and zod 4.6.7. `web/package.json` stays the same.
4. **Checked before the push, with Node 22 as in the pipeline.** `npm audit --audit-level=high` found 0 vulnerabilities, where the lock file of dev still names both. `npm run build`, `npm run lint`, `npx prettier --check .` and `npm test` stayed green, 6 test files and 35 tests, none edited. The built screen in `web/dist` is byte for byte the same as with the lock file of dev, 38 files compared by sha256, so nothing that reaches the browser changed.
5. **The green run.** frontend-scan of !99 is green, pipeline 587414, job 1351411: "found 0 vulnerabilities".
6. **What this run shows that #309 did not.** The throwaway branch of #309 added a package with a known hole on purpose, so the scan stopped a change that brought the hole in. This time the hole was published after the packages were already on dev, and the scan stopped the next merge request that ran, a change to documents only. Every merge request that started before the fix stays red until it takes in dev again. The weekly run on Monday would have found it too, but the first pipeline after the advisory was earlier.
7. **Conclusion for improvement.** A red scan can come from a package nobody touched, so the first look in the red job is the package and its advisory, not the change of the merge request. That is already the first step of part 11.

## TODO

- [x] Wesley: after the merge, a comment on every open merge request that it needs dev again for a green frontend-scan. Done on 24 October 2026 another way: !99 was merged that day, and my open merge requests that merge cleanly with dev (!68, !69, !79, !88, !98 and !100) and Gurpreet's !97 were rebased onto dev in GitLab, after which their pipelines were green. The chain !70 to !76 gets dev with its own rebase (#1542).

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

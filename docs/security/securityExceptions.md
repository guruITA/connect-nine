# Accepted security risks

A package with a known high or critical advisory stops the pipeline (job backend-scan or frontend-scan, issue #22). The first answer is always to update the package to a version without the advisory. When no fixed version exists yet, only the client of the foundation may accept the risk, for at most 30 days, with a reason. The team never accepts a risk itself.

## How a risk is accepted

1. The team writes to the client which package, which advisory, what an attacker could do with it in Connect, and why there is no fix yet.
2. The client answers in writing. That answer goes into `docs/client/clientContact.md`.
3. One row below, with an end date no later than 30 days after the answer.
4. The server: one line `<NuGetAuditSuppress Include="<advisory address>" />` in `api/Directory.Build.props`. The screen: `allow_failure: true` on frontend-scan, with a comment naming the advisory and the end date.
5. On the end date the line is removed. The pipeline turns red again if the advisory is still open, and the steps start over.

## Accepted risks

| Advisory | Package | Reason | Accepted by | End date |
|---|---|---|---|---|
| None | | | | |

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.

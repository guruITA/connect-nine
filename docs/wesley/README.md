# Wesley Oerlemans: research and sprint 1 evidence

This folder holds my research plan and my evidence for two sprint 1 datapoints of the Full Stack Software Developer profile at level 3. Three files are for the learning group teacher, and the other three are the full documents behind them.

## For the teacher, one per datapoint

| File | Datapoint | What it is for |
| --- | --- | --- |
| `topicProposalForm.docx` | Datapoint 3, Teacher Review of Research Topic (Analysing & Advising) | The course's own topic proposal form, filled in. This is the file to upload to Portflow. It gives my name and student number, the title, a one-paragraph description, the main question with three sub-questions and their hypotheses, the methods and three sources. That covers the criterion: a research question with sub-questions, an experiment and a reasonable scope. |
| `topicProposalForm.md` | Datapoint 3 | The same content in Markdown, so it reads in GitLab without opening Word. The Word file is built from it, so the two match. |
| `securityAndArchitectureEvidence.md` | Datapoint 4, Teacher Review of Progress in Security and Architecture (Designing & Realising) | A summary of about 2,800 words that the teacher can read on its own. It answers each example the datapoint names: an architecture, research into possible architectures, security requirements and design patterns. It shows the two key diagrams, the patterns, the decision records with their real status, the security fundamentals, what comes next and an honest status table. Each claim points to the section of the full document that holds the detail. |

## Proof per task, for Portflow

One file per task I did, with the proof block of the task filled in. A point that needs someone else is marked TODO with who and what.

| File | Task | Portflow |
| --- | --- | --- |
| `portfolioProof/task2RepositorySkeleton.md` | Task 1.1 The starting structure (#2), merge request !13 | Managing and Collaborating MS1 and MS4 |
| `portfolioProof/task9LoggedAfterwards.md` | Task 9 Templates and rules (#9), work logged afterwards as issues #1500 to #1515, merge request !38 | Managing and Collaborating MS1 and MS4 |

## The full documents behind them

| File | What it is for |
| --- | --- |
| `researchPlan.md` | My complete research plan (Plan van Aanpak) behind the topic proposal form. It holds the problem and the W-questions, the business case and stakeholders, the questions and hypotheses, the method, validity and reliability, the measurement ranges, acceptance criteria linked to the BoKSA level 3 outcomes, the planning, the risks and the sources, with mermaid diagrams for the hypotheses, the experiment set-up, the measurement matrix and the protocol steps. It is the basis for the R&D report reviewed in sprint 3. |
| `securityAndArchitecture.md` | The full architecture document, "Architecture of Young Amstelland Connect". It holds the drivers and quality attribute scenarios, the choice of style, the dependency rule and all five C4 diagrams including the deployment on both environments, the test site on Docker Compose and production on single-node k3s, a diagram for one request through the layers, the four design patterns with the CQRS folder layout, and eleven decision records, including Caddy as the front door for both environments and Traefik inside production k3s behind it, all as mermaid diagrams, and how the design stays in step with the code. The datapoint 4 summary takes its architecture half from here. |
| `securityFundamentals.md` | The full security document, "Security and access fundamentals: applied to Young Amstelland Connect". It holds the seven BoKSA security and access fundamentals applied to Connect, the mini threat model, security in the deployment and the OWASP Top 10 table, with mermaid diagrams for the sign-in sequence, the authentication and authorization check and the STRIDE data flow. The datapoint 4 summary takes its security half from here. |

## What goes into Portflow

- Datapoint 3: upload `topicProposalForm.docx`.
- Datapoint 4: link merge request !10 or `securityAndArchitectureEvidence.md`.
- The full documents stay here for when the teacher wants the detail.

## AI use declaration

AI was used as a writing support tool while working on this document. I used AI to draft the formulation of sentences, the structure of wording, readability, and clarity from content that I had provided. The content itself, including my experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is my own. I reviewed the drafts and decided which text to include.

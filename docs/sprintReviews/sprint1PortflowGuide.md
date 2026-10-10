# Sprint 1 review: Portflow steps

Young Amstelland Connect, team Sprint 1 review of 9 October 2026. Everything except the personal blocks in step 9 is in team voice. Only Kelvin and Bayon adjust the profile, the level and the feed-up lines (step 8). Two pieces of evidence, both an **Add link** to the files in the team repository (merge commit c0ca49c7, merge request !43, issue #1519). Evidence 1 is the deck, for Analysing & Advising and Designing & Realising. Evidence 2 is the feedback on our process and teamwork, for Managing & Collaborating. Each evidence gets one comment with the feedback of both the client and the teacher, in one section per sub-goal. Each section is highlighted and linked to its own sub-goal. Every claim comes from the deck, its speaker notes, the board and Figma shown in the review, the client's form, what the client said in the review, the teacher's feedback or GitLab.

## 1. DLO (deadline was 9 October, 23:59)

If not uploaded yet: download both files from the GitLab links in steps 2 and 4 and upload them to the folder "Sprint Review 1", so DLO and Portflow hold the same version. The upload is then late.

## 2. Evidence 1: the Sprint Review

**Add it:** Collections, then **Project Group**, then **Add evidence**, then **Add link**.
**Name:** `Sprint 1 review: Young Amstelland Connect`
**Link:**
```
https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/blob/c0ca49c7ef6bf29574bb6fedd066b548b0c15c1b/docs/sprintReviews/sprint1ReviewDeck.pptx
```

**Description:** open the evidence, click the pencil next to **Evidence description**, and paste the text below. Then add the personal block of the student from step 9 at the end, after an empty line.
```
Sprint 1 review of Young Amstelland Connect, 9 October 2026, for our client Rachid Kherrazi (WailSalutem Foundation), by Team 9 (Connect Nine). All five of us presented, each part with its own speakers, time and speaker notes. We showed this deck, the GitLab board and the Figma prototype. The deck and the client's form are in docs/sprintReviews in our team repository (merge request !43, issue #1519). Feedback on analysis and design is in the comment on this evidence, and feedback on process and teamwork is in the evidence "Sprint 1 review: feedback from the client".

ABOUT THE LEVELS
Levels are measured against BoKSA Full-Stack Developer level 3 (Advanced): "beyond expectation" from the client is above level 3 and "as expected" is at level 3. Under points to improve he wrote "n.a.", so he saw nothing to improve. Without a rating and without a point to improve from the teacher, a sub-goal is above level 3 when the review gives proof. That is our own assumption, and the section names the proof. If a teacher disagrees with a level, we would like to know on what basis, and why that feedback was not given during the review, so we can ask for it there next time.

AA3 - above level 3 (client: "Clear idea of project needs" beyond expectation)
We opened with the client's question and the frame of Connect: standalone next to the award website, aimed at impact, not revenue (young talent visible, young people motivated, the HvA partnership shown). We weighed the ethics: our members are 15 to 30, so some are minors on an open platform. That is why a parent gives permission before the profile of anyone under 16 is visible, Connect never asks for an exact age, birth date, school or street address, and a member can delete everything. We asked the client whether an organisation may see and contact a member under 18. Our requirements list holds 10 functional requirements, each with a MoSCoW priority, and 6 non-functional ones, among them GDPR and authorization.

AA4 - above level 3 (no rating and no point to improve on our advice, so our own assumption)
Our proposed order: profiles, account and matching first, then opportunities, events and admin. Alternatives weighed: the webshop gets no sprint (the client said on 25 September that it is not required, and no must item waits for it), and PostgreSQL 16 replaced SQL Server (no license cost, a plain container on the foundation's server). Our plan for Sprints 2 to 5 (Decision 004, #1516) puts school goals first, because every one of us is assessed on them, then every must item, and no should or could before the last must. Risk: most of us are new to .NET, but Nick knows it and can unblock the other four.

AA5 - above level 3 (client: "Presentation skills" beyond expectation)
For the client: his question word for word in Dutch with an English translation, the four server layers explained as waiter, chef, menu and storeroom, and two plain questions at the end. For the young people: the prototype has the look of the Young Amstelland Awards website, and the deck explains its screens in plain words, such as "First name, age band and town. Never an address or a school."

OR1 - above level 3 (praised by the client in the review, without a rating, so our own assumption)
On purpose, we chose a stack most of us did not know, because learning is the main goal of the semester. Each part has its reason on the slide: learning for React and .NET, support until November 2028 for .NET 10, no license cost and a plain container for PostgreSQL 16. The server code has four layers (#12), and a wrong reference between them does not build. Decisions 001 to 003 record the conventions, the layers and .NET 10. A backend with a health check, the account table and a local PostgreSQL 16 database in one command are in. When PostgreSQL replaced SQL Server on 26 September, only one layer was affected. The Figma prototype, shown live, walks the Sprint 1 flow from a new account to another member's profile and holds the six security and privacy rules from our requirements.

OR2 - above level 3 (strong point named by the client, without a rating, so our own assumption)
In our repository (#1, #22, #1513) changes reach main only from dev, and every merge needs a teammate's approval and a green pipeline. The pipeline refuses .env files with secrets, and a weekly scan, pulled forward from Sprint 2, stops packages with a known high or critical risk. Code conventions come with examples, templates and an onboarding guide. We presented this as a foundation every next screen can land on. Why it fits, in our own reasoning: a second pair of eyes suits a team where most of us are learning the stack, and blocking secrets and risky packages suits a platform with personal data of members under 18.

OR3 - above level 3 (no rating and no point to improve, so our own assumption)
We involve the client systematically: his choices of 25 September (the webshop is not required, the test site holds only invented accounts) are in our plan and rules, we keep a list of 134 questions only the foundation can answer, and we asked him for one meeting on the order and these questions, and for a free Figma account, so every link in our page descriptions (#1496) opens the right screen for him. We acted on a teacher's request: 16 items of work done before they had an issue are now on the board. Through the client we brought in the interests of others: does the prototype fit the young people he knows, and may an organisation contact a member under 18? In our choices, the profile of anyone under 16 is visible only with a parent's permission, and school goals come first, because every one of us is assessed on them.
```

**Link the evidence:**
1. Top right on the evidence, click **⋯**, then **Manage (sub-)goals**.
2. Under **Select goals to add and remove**, pick: Analysing & Advising, Designing & Realising.
3. Under **Select sub-goals to add and remove**, pick: AA3, AA4, AA5, OR1, OR2, OR3.
4. Click **Save**.

## 3. Comment 1, on evidence 1

**Add it:** under **Evidence feedback**, click **Add comment** (in "Add comment or request feedback"), paste the whole text below and save it.

**Link each section:** do this six times, once per section (AA3, AA4, AA5, OR1, OR2, OR3):
1. Click **⋮** on the comment, then **Link to sub-goal(s)**. The window **Create link to goal(s) or sub-goal(s)** opens.
2. Leave **Link to goal(s)** empty. Under **Link to sub-goal(s)**, pick the code of the section.
3. Under **Highlight feedback**, click the marker button and select that section only: from its first line (the code) to its last line (the one that starts with "Feed-forward").
4. Click **Save**.

```
Feedback on our team Sprint 1 review of 9 October 2026 from our client Rachid Kherrazi (WailSalutem Foundation, Sprint Review form and what he said in the review) and our teacher Rob Kiebert, with one section per sub-goal. Levels are measured against BoKSA Full-Stack Developer level 3 (Advanced). If a teacher disagrees with a level, we would like to know on what basis, and why that feedback was not given during the review, so we can ask for it there next time.

AA3 - above level 3 (based on the client's rating)
Client: "Clear idea of project needs": beyond expectation. Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: rated beyond expectation, with nothing to improve.
Feed-up: at level 3, issues are analysed from multiple perspectives and the societal and ethical consequences are weighed consciously.
Feed-forward: we keep opening each review with the client's question and the frame of Connect, and in Sprint 2 we finish the analysis behind the rules for minors and check it with the client.

AA4 - above level 3 (no rating and no point to improve on our advice, so our own assumption)
Client: no rating. Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on our advice. We read his point on not taking on too much as being about the weight we plan, not the order we advise.
Why this level: each part of the criterion is in the review. Recommendation: profiles, account and matching first, and in the plan (Decision 004, #1516) school goals first, because every one of us is assessed on them. Alternatives: the webshop without a sprint and PostgreSQL 16 in place of SQL Server, each with its reason. Risk: most of us are new to .NET, and a teammate who knows it can unblock the others.
Feed-up: at level 3, advice is well-founded: it weighs alternatives, names risks and ends in a substantiated recommendation.
Feed-forward: we repeat this in the Sprint 2 review: advice with the alternatives we weighed, the risks and the reasons.

AA5 - above level 3 (based on the client's rating)
Client: "Presentation skills": beyond expectation. In the review he said the prototype has a nice theme and takes over the style of the award website. Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on how we communicated. We read his point on who says what as being about dividing the roles, not about how we adapted our words and screens.
Why this level: rated beyond expectation, with nothing to improve.
Feed-up: at level 3, communication is consciously adapted in form and language to target groups with diverse interests.
Feed-forward: we keep using everyday comparisons for the client, and in Sprint 2 we show working screens with plain texts for the young people.

OR1 - above level 3 (praised by the client in the review, without a rating, so our own assumption)
Client: no rating. In the review he said everything in the prototype was good, nicely worked out and clear. Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: the client called the prototype nicely worked out and clear, and the review shows a systematic design and realisation (four layers in #12 that refuse a wrong reference, a backend with a health check and a local PostgreSQL 16 database in one command, Decisions 001 to 003, and a live Figma prototype of the Sprint 1 flow with the six rules from our requirements), a reason for every part of the stack, PostgreSQL in place of SQL Server with its reasons, and a design that held up when we switched databases on 26 September, with only one layer affected.
Feed-up: at level 3, design and realisation are systematic, choices are justified against alternatives, and the design is adapted to new insights.
Feed-forward: we repeat this in the Sprint 2 review: a reason for every design choice and how the design holds up against a change.

OR2 - above level 3 (strong point named by the client, without a rating, so our own assumption)
Client: strong point of the team: "using the development tools". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: the client named it our strong point. The review shows the methods we selected (a teammate's approval and a green pipeline for every merge, secrets refused, and a weekly scan for risky packages that we pulled forward from Sprint 2), presented as a foundation every next screen can land on. Our own reasoning adds why they fit: a second pair of eyes suits a team where most of us are learning the stack, and blocking secrets and risky packages suits a platform with personal data of members under 18.
Feed-up: at level 3, relevant standards and methods from the field are selected independently, and why they fit the context is substantiated.
Feed-forward: every change keeps going through a merge request with a teammate's approval and a green pipeline as the code grows in Sprint 2.

OR3 - above level 3 (no rating and no point to improve, so our own assumption)
Client: no rating. Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: the review shows systematic involvement of the client (his choices in our plan and rules, a list of 134 questions only the foundation can answer, and our requests for one meeting on them and for his own Figma account), a teacher's request acted on, the interests of young people and organisations brought in through our questions to the client, and those of parents and the HvA weighed in our rules and plan.
Feed-up: at level 3, multiple stakeholders with diverse interests are involved proactively, the coordination with them is systematic, and their feedback is weighed consciously in the choices.
Feed-forward: we repeat this in the Sprint 2 review: the client's choices in our plan, the open questions, what we need from the foundation, and questions for his feedback.
```

## 4. Evidence 2: feedback from the client and the teacher

**Add it:** Collections, then **Project Group**, then **Add evidence**, then **Add link**.
**Name:** `Sprint 1 review: feedback from the client`
**Link:**
```
https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/blob/c0ca49c7ef6bf29574bb6fedd066b548b0c15c1b/docs/sprintReviews/sprint1ReviewForm.pdf
```

**Description:** pencil next to **Evidence description**, and paste:
```
Feedback on our team Sprint 1 review of 9 October 2026 from our client Rachid Kherrazi (the Sprint Review form behind this link) and our teacher Rob Kiebert, on our process and teamwork. The comment has one section per sub-goal, each linked to it. The client's feedback on project needs, presentation, the prototype and development tools is with the evidence "Sprint 1 review: Young Amstelland Connect". The form is in docs/sprintReviews in our team repository (merge request !43, issue #1519), which is how DLO asks us to share it with the team coach.

ABOUT THE LEVELS
Levels are measured against BoKSA Full-Stack Developer level 3 (Advanced): "beyond expectation" from the client is above level 3 and "as expected" is at level 3. Under points to improve he wrote "n.a.", so he saw nothing to improve. Where the teacher gave a point to improve, the sub-goal is at level 3, also when the client rated the topic beyond expectation. If a teacher disagrees with a level, we would like to know on what basis, and why that feedback was not given during the review, so we can ask for it there next time.

MS1 - above level 3 (client feedback)
Client: "Clear on what will be delivered in next sprint (user stories)": beyond expectation.

MS2 - at level 3 (client and teacher feedback)
Client: "User stories delivered in line with planning": beyond expectation. Teacher: plan to the weight done in earlier sprints, so we do not take on too much. That point keeps MS2 at level 3.

MS4 - at level 3 (client and teacher feedback)
Client: "Leading the meeting - clear structure and roles": beyond expectation. "Each team member actively participated": as expected. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Teacher: prepare better beforehand who says what. The "as expected" and the teacher's point put MS4 at level 3.
```

**Link the evidence:**
1. **⋯**, then **Manage (sub-)goals**.
2. Under **Select goals to add and remove**, pick: Managing & Collaborating.
3. Under **Select sub-goals to add and remove**, pick: MS1, MS2, MS4.
4. Click **Save**.

## 5. Comment 2, on evidence 2

**Add it:** under **Evidence feedback**, click **Add comment**, paste the whole text below and save it.

**Link each section:** the same way as in step 3, three times, once per section (MS1, MS2, MS4): **⋮**, **Link to sub-goal(s)**, the code of the section, highlight only that section (from the code line to the line that starts with "Feed-forward"), **Save**.

```
Feedback on our team Sprint 1 review of 9 October 2026 from our client Rachid Kherrazi (WailSalutem Foundation, Sprint Review form) and our teacher Rob Kiebert, with one section per sub-goal. Levels are measured against BoKSA Full-Stack Developer level 3 (Advanced). If a teacher disagrees with a level, we would like to know on what basis, and why that feedback was not given during the review, so we can ask for it there next time.

MS1 - above level 3 (based on the client's rating)
Client: "Clear on what will be delivered in next sprint (user stories)": beyond expectation. Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on how clear the next sprint is. We read his point on not taking on too much as being about the weight we plan.
Why this level: rated beyond expectation. On the board of 9 October 2026 we showed the new Sprint 1 and Sprint 2, with the Sprint 2 goal and its 30 items, all with acceptance criteria and a definition of done, 17 already with an owner. We asked the client for the DNS record our test address waits for and for one meeting on the open questions. Every part of Connect is on one board, with risks and open points in one place, so the work stays transparent for anyone who joins.
Feed-up: at level 3, risks and bottlenecks are anticipated before they become problems, and the work stays transparent and transferable.
Feed-forward: at every review we show the next sprint goal and what we wait on, and we ask the foundation in time for what we need.

MS2 - at level 3 (the teacher gave a point to improve)
Client: "User stories delivered in line with planning": beyond expectation. Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: plan to the weight done in earlier sprints, so we do not take on too much.
Why this level: the teacher's point to improve keeps it at level 3.
Feed-up: at level 3, progress and collaboration are evaluated critically, patterns are recognised, and the process is adjusted proactively where necessary.
Feed-forward: on the board of 9 October 2026, 46 of the 48 weight in Sprint 1 was done, counting work waiting in a merge request (38 closed). Sprint 2 is planned at 63, because Sprint 1 was preparation and we expect to go faster now. After the Sprint 2 review we compare the weight done in Sprint 2 with the 63 and use that difference for the plan of Sprint 3.

MS4 - at level 3 (the teacher gave a point to improve)
Client: "Leading the meeting - clear structure and roles": beyond expectation. "Each team member actively participated": as expected. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: prepare better beforehand who says what.
Why this level: participation was rated as expected, and the teacher's point on who says what also keeps it at level 3.
Feed-up: at level 3, each member makes sure their work aligns with the work of the others and contributes to the joint result.
Feed-forward: before the next review every speaker knows their part and which questions they answer, and we rehearse once together.
```

## 6. Validation

On comment 1 and on comment 2: **⋮**, then **Request comment validation**, then choose Rob Kiebert. The client cannot validate, because he is not a teacher. Each comment holds his feedback too, which Rob can check against the client's form behind evidence 2 and, for what the client said about the prototype, against the review itself.

## 7. Check

Open **Goals & Progress**. Each of these sub-goals should show its evidence and a highlighted section of a comment: AA3, AA4, AA5, OR1, OR2, OR3, MS1, MS2, MS4. A sub-goal without both means a link is missing. No comment section should be linked to a goal name, only to a sub-goal code. The evidence 1 description and comment 1 each end with the personal part of the student from step 9, and that sub-goal shows a highlighted section too.

## 8. Team

- Bayon, Nick, Gurpreet and Kelvin follow steps 2 to 7 in their own Portflow with the same names, GitLab links, descriptions, comments, sub-goal links and highlights, plus their own personal block from step 9. Kelvin and Bayon change only what their own line below says.
- Validation works per portfolio, so each of the five sends the two validation requests from their own Portflow. Rob Kiebert then gets ten requests in total, so we tell him beforehand in Teams.
- Kelvin's profile is Security Specialist: he replaces "Full-Stack Developer" with "Security Specialist" everywhere and rewrites each feed-up line to his own level 3 criterion, in the same style ("at level 3, ...") and without "you".
- Bayon is assessed on level 2: he first replaces "level 3 (Advanced)" with "level 2", then every other "level 3" with "level 2", and rewrites each feed-up line to the level 2 criterion of his profile, in the same style ("at level 2, ...") and without "you".

## 9. Personal blocks

The expectations of the studio lecturers ask for proof of the personal contribution. Each student, Wesley included, adds only their own part to evidence 1, with every sub-goal of their own profile that their own work proves, in three places: the codes in **Manage (sub-)goals**, the paragraphs at the end of the description, and the sections at the end of comment 1, each linked and highlighted like the sections in step 3. The numbers come from the current board in GitLab (assignee, closed or merged). Measure them again on the day of pasting, because they change when a merge request is merged.

### Wesley Oerlemans (Full Stack Software Developer, level 3): MS1, MS2, MS4, MS5, OR1

1. **Manage (sub-)goals** on evidence 1: add the sub-goals MS1, MS2, MS4, MS5, and the goal Managing & Collaborating. OR1 is already there from step 2.
2. At the end of the description, after an empty line:
```
MS1 Personal contribution of Wesley Oerlemans - above level 3 (client rated this beyond expectation for the team, so own assumption for the personal part)
Wesley Oerlemans owns the closed issues that keep the risks in view and the work transparent: the biggest risks and every open point in one place (#1511), every part of Connect on one board with its sprint and priority (#1509), a calendar with one goal per sprint (#1505), a testing plan (#1508) and a plan of the evidence each student needs per sprint (#1510).

MS2 Personal contribution of Wesley Oerlemans - at level 3 (the teacher gave a point to improve)
As Scrum Master of Sprint 1, Wesley Oerlemans made the new plan for Sprints 2 to 5 after Sprint 1 (Decision 004, #1516, waiting in an open merge request): school goals first, then every must item, and a check of the plan after the Sprint 2 review.

MS4 Personal contribution of Wesley Oerlemans - at level 3 (the teacher gave a point to improve)
Wesley Oerlemans kept the team documents in line with the shared work: every page description of Sprints 1 and 2 points to its screen in the prototype (#1496), and the team rules describe the Figma board as it is now (#1498). Of the 37 merged merge requests, 23 are by Wesley Oerlemans.

MS5 Personal contribution of Wesley Oerlemans - above level 3 (no rating and no point to improve, so own assumption)
Wesley Oerlemans approved 6 of the 14 merged merge requests of teammates.

OR1 Personal contribution of Wesley Oerlemans - above level 3 (no rating and no point to improve, so own assumption)
Wesley Oerlemans set up the one clear structure for the server code (#12, merge request merged): four layers where a wrong reference does not build. When PostgreSQL replaced SQL Server on 26 September, only one layer was affected.
```
3. At the end of comment 1, after an empty line, then link each section to its own code and highlight it:
```
MS1 - above level 3 (client rated this beyond expectation for the team, so own assumption for the personal part)
Client: "Clear on what will be delivered in next sprint (user stories)": beyond expectation for the team. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on risks or transparency. His point on not taking on too much is read as being about the weight planned.
Why this level: the client rated the team beyond expectation on this, and the board shows the personal part: risks and open points in one place (#1511), every part of Connect on one board (#1509), one goal per sprint (#1505), a testing plan (#1508) and an evidence plan per student (#1510).
Feed-up: at level 3, risks and bottlenecks are anticipated before they become problems, and the work stays transparent and transferable.
Feed-forward: in Sprint 2, Wesley Oerlemans keeps the list of risks and open points and the board up to date at every change.

MS2 - at level 3 (the teacher gave a point to improve)
Client: "User stories delivered in line with planning": beyond expectation for the team. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: plan to the weight done in earlier sprints, so not too much is taken on.
Why this level: the client rated the team beyond expectation, but the teacher's point to improve is about this plan and keeps it at level 3.
Feed-up: at level 3, progress and collaboration are evaluated critically, patterns are recognised, and the process is adjusted proactively where necessary.
Feed-forward: after the Sprint 2 review, Wesley Oerlemans compares the weight done in Sprint 2 with the 63 planned and adjusts the plan for Sprint 3 to it.

MS4 - at level 3 (the teacher gave a point to improve)
Client: "Leading the meeting - clear structure and roles": beyond expectation. "Each team member actively participated": as expected. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: prepare better beforehand who says what.
Why this level: participation was rated as expected, and the teacher's point on who says what keeps it at level 3.
Feed-up: at level 3, each member makes sure their work aligns with the work of the others and contributes to the joint result.
Feed-forward: before the next review, Wesley Oerlemans agrees with every speaker which part and which questions they take, and keeps the page descriptions in line with the prototype.

MS5 - above level 3 (no rating and no point to improve, so own assumption)
Client: no rating. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: nobody gave a point to improve, and GitLab shows 6 approvals of merge requests of teammates.
Feed-up: at level 3, others are supported in their contribution to the team goal.
Feed-forward: in Sprint 2, Wesley Oerlemans keeps reviewing merge requests of teammates.

OR1 - above level 3 (no rating and no point to improve, so own assumption)
Client: no rating. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: nobody gave a point to improve, and the merged structure shows a planned design that held up against a new insight: four layers that refuse a wrong reference, and only one layer affected by the switch to PostgreSQL.
Feed-up: at level 3, design and realisation are systematic, choices are justified against alternatives, and the design is adapted to new insights.
Feed-forward: in Sprint 2, Wesley Oerlemans builds the profile issues (#25, #27, #30) in the same four layers.
```

### Nick Hoebe (Full Stack Software Developer, level 3): OR1, OR2, OR3, AA1, MS5

1. **Manage (sub-)goals** on evidence 1: add the sub-goals AA1, MS5, and the goal Managing & Collaborating. OR1, OR2, OR3 are already there from step 2.
2. At the end of the description, after an empty line:
```
OR1 Personal contribution of Nick Hoebe - above level 3 (no rating and no point to improve, so own assumption)
Nick Hoebe built the backend from architecture documentation with pattern evidence (#1476): the solution structure (!11), the project structure with a health endpoint (!22) and the account table (!36).

OR2 Personal contribution of Nick Hoebe - above level 3 (the client named using the development tools a strong point of the team, without a rating, so own assumption)
Nick Hoebe added the pipeline stage that refuses .env files, so no secret reaches the repository (!7, #1513), and the configuration documentation with a safe .env.example for every setting (!6, #1512).

OR3 Personal contribution of Nick Hoebe - above level 3 (no rating and no point to improve, so own assumption)
Nick Hoebe kept the contact log with the client and worked his answers into the open questions (!19), and arranged a server of the foundation for Connect (#1514).

AA1 Personal contribution of Nick Hoebe - above level 3 (no rating and no point to improve, so own assumption)
Nick Hoebe wrote a research proposal on Google OAuth and registration friction (!9) and revised it after feedback (!14).

MS5 Personal contribution of Nick Hoebe - above level 3 (no rating and no point to improve, so own assumption)
Nick Hoebe approved 14 of the 29 merged merge requests of teammates.
```
3. At the end of comment 1, after an empty line, then link each section to its own code and highlight it:
```
OR1 - above level 3 (no rating and no point to improve, so own assumption)
Client: no rating. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: nobody gave a point to improve, and the merged work shows a planned realisation: architecture documentation first (#1476), then the structure, the health endpoint and the account table (!11, !22, !36).
Feed-up: at level 3, design and realisation are systematic, choices are justified against alternatives, and the design is adapted to new insights.
Feed-forward: in Sprint 2, Nick Hoebe builds the login (#15) on this structure.

OR2 - above level 3 (the client named using the development tools a strong point of the team, without a rating, so own assumption)
Client: strong point of the team: "using the development tools". Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: the client named the development tools a strong point of the team, and the merged work of Nick Hoebe shows methods from the field chosen with a reason: a pipeline stage that keeps secrets out of the repository (!7) and a safe example for every setting (!6).
Feed-up: at level 3, relevant standards and methods from the field are selected independently, and why they fit the context is substantiated.
Feed-forward: in Sprint 2, Nick Hoebe writes down for the login (#15) which method is chosen and why it fits Connect.

OR3 - above level 3 (no rating and no point to improve, so own assumption)
Client: no rating. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: nobody gave a point to improve, and the merged log shows systematic coordination with the client and his answers worked into the open questions (!19), plus the server arranged with the foundation (#1514).
Feed-up: at level 3, multiple stakeholders with diverse interests are involved proactively, the coordination with them is systematic, and their feedback is weighed consciously in the choices.
Feed-forward: in Sprint 2, Nick Hoebe keeps the contact log and the open questions up to date after every contact with the client.

AA1 - above level 3 (no rating and no point to improve, so own assumption)
Client: no rating. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: nobody gave a point to improve, and the two merged versions show a research approach chosen and adapted based on feedback (!9, !14).
Feed-up: at level 3, a research approach that suits the issue is chosen independently and adapted based on what is encountered.
Feed-forward: in Sprint 2, Nick Hoebe carries out the research and writes down where the approach changes and why.

MS5 - above level 3 (no rating and no point to improve, so own assumption)
Client: no rating. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: nobody gave a point to improve, and GitLab shows 14 approvals of merge requests of teammates.
Feed-up: at level 3, others are supported in their contribution to the team goal.
Feed-forward: in Sprint 2, Nick Hoebe keeps reviewing merge requests of teammates.
```

### Gurpreet Singh (Full Stack Software Developer, level 3): OR1, OR3, AA1, MS5

1. **Manage (sub-)goals** on evidence 1: add the sub-goals AA1, MS5, and the goal Managing & Collaborating. OR1, OR3 are already there from step 2.
2. At the end of the description, after an empty line:
```
OR1 Personal contribution of Gurpreet Singh - above level 3 (no rating and no point to improve, so own assumption)
Gurpreet Singh designed all the data Connect could keep, with schema, dummy data and local setup (#1466, !12), set up the local PostgreSQL 16 database with pgAdmin in one command (!21), built the React app with the home page of the Figma design (!38), and renamed the partner banner so ad blockers do not block it (!41).

OR3 Personal contribution of Gurpreet Singh - above level 3 (no rating and no point to improve, so own assumption)
Gurpreet Singh arranged the contact between the team and the client in Sprint 1 (#1515).

AA1 Personal contribution of Gurpreet Singh - above level 3 (no rating and no point to improve, so own assumption)
Gurpreet Singh wrote a privacy by design research plan (!8) and replaced it with an election import proposal (!20).

MS5 Personal contribution of Gurpreet Singh - above level 3 (no rating and no point to improve, so own assumption)
Gurpreet Singh approved 20 of the 31 merged merge requests of teammates, the most in the team.
```
3. At the end of comment 1, after an empty line, then link each section to its own code and highlight it:
```
OR1 - above level 3 (no rating and no point to improve, so own assumption)
Client: no rating. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: nobody gave a point to improve, and the merged work shows a planned design and realisation that is adapted to a new insight: the data design (#1466, !12), the local database (!21), the home page from the Figma design (!38) and the rename so ad blockers do not block the banner (!41).
Feed-up: at level 3, design and realisation are systematic, choices are justified against alternatives, and the design is adapted to new insights.
Feed-forward: in Sprint 2, Gurpreet Singh builds the health check of Connect (#23) on the same setup.

OR3 - above level 3 (no rating and no point to improve, so own assumption)
Client: no rating. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: nobody gave a point to improve, and the board shows the contact with the client arranged (#1515).
Feed-up: at level 3, multiple stakeholders with diverse interests are involved proactively, the coordination with them is systematic, and their feedback is weighed consciously in the choices.
Feed-forward: in Sprint 2, Gurpreet Singh, as Scrum Master, plans the meeting with the client on the order and the open questions.

AA1 - above level 3 (no rating and no point to improve, so own assumption)
Client: no rating. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: nobody gave a point to improve, and the two merged versions show a research approach chosen and changed (!8, !20).
Feed-up: at level 3, a research approach that suits the issue is chosen independently and adapted based on what is encountered.
Feed-forward: in Sprint 2, Gurpreet Singh carries out the research and writes down why the topic changed.

MS5 - above level 3 (no rating and no point to improve, so own assumption)
Client: no rating. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: nobody gave a point to improve, and GitLab shows 20 approvals of merge requests of teammates, the most in the team.
Feed-up: at level 3, others are supported in their contribution to the team goal.
Feed-forward: in Sprint 2, Gurpreet Singh keeps reviewing the merge requests of teammates and shows that work as Scrum Master in #1483.
```

### Kelvin van Voorst (Security Specialist, level 2): OR5

1. **Manage (sub-)goals** on evidence 1: add the sub-goal OR5.
2. At the end of the description, after an empty line:
```
OR5 Personal contribution of Kelvin van Voorst - no level this time (no finished own work on the board yet)
On the current board, Kelvin van Voorst owns the four privacy and security analysis issues of Sprint 2: the personal data per module and its risks (#21), the privacy law on the data of minors (#24), who may see and change what (#28) and how profile data travels through Connect (#29).
```
3. At the end of comment 1, after an empty line, then link each section to its own code and highlight it:
```
OR5 - no level this time (no finished own work on the board yet)
Client: no rating. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: the four analysis issues of Kelvin van Voorst (#21, #24, #28, #29) moved from Sprint 1 to Sprint 2 and are still open, so there is no finished own work to assess yet.
Feed-up: at level 2, the interests of end users, such as privacy, security or accessibility, are taken into account.
Feed-forward: in Sprint 2, Kelvin van Voorst finishes #21, #24, #28 and #29 and shows the result in the Sprint 2 review.
```

### Bayon Eerhart (Full Stack Software Developer, level 2): MS5

1. **Manage (sub-)goals** on evidence 1: add the sub-goal MS5, and the goal Managing & Collaborating.
2. At the end of the description, after an empty line:
```
MS5 Personal contribution of Bayon Eerhart - above level 2 (no rating and no point to improve, so own assumption)
Bayon Eerhart took on the role of Product Owner for Sprint 2, shown in the review, and owns two Sprint 2 issues, each with acceptance criteria and a definition of done: moving between the five tabs of Connect from every page (#19) and seeing the own name under the tab Profile (#20).
```
3. At the end of comment 1, after an empty line, then link each section to its own code and highlight it:
```
MS5 - above level 2 (no rating and no point to improve, so own assumption)
Client: no rating. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: nobody gave a point to improve, and the review and the board show the tasks taken on: Product Owner of Sprint 2 and the Sprint 2 issues #19 and #20.
Feed-up: at level 2, tasks that contribute to the team goal are taken on, also when they fall outside the own responsibilities.
Feed-forward: in Sprint 2, Bayon Eerhart finishes #19 and #20 and shows them working in the Sprint 2 review.
```

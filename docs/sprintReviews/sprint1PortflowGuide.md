# Sprint 1 review: Portflow steps

Young Amstelland Connect, team Sprint 1 review of 9 October 2026. All texts are in team voice; only Kelvin and Bayon adjust the profile, the level and the feed-up lines (step 8). Two pieces of evidence, both an **Add link** to the files in the team repository (merge commit c0ca49c7, merge request !43, issue #1519). Evidence 1 is the deck, for Analysing & Advising and Designing & Realising. Evidence 2 is the feedback on our process and teamwork, for Managing & Collaborating. Each evidence gets one comment with the feedback of both the client and the teacher, in one section per sub-goal. Each section is highlighted and linked to its own sub-goal. Every claim comes from the deck, its speaker notes, the board and Figma shown in the review, the client's form, the teacher's feedback or GitLab.

## 1. DLO (deadline was 9 October, 23:59)

If not uploaded yet: download both files from the GitLab links in steps 2 and 4 and upload them to the folder "Sprint Review 1", so DLO and Portflow hold the same version. The upload is then late.

## 2. Evidence 1: the Sprint Review

**Add it:** Collections, then **Project Group**, then **Add evidence**, then **Add link**.
**Name:** `Sprint 1 review: Young Amstelland Connect`
**Link:**
```
https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/blob/c0ca49c7ef6bf29574bb6fedd066b548b0c15c1b/docs/sprintReviews/sprint1ReviewDeck.pptx
```

**Description:** open the evidence, click the pencil next to **Evidence description**, and paste:
```
Sprint 1 review of Young Amstelland Connect, 9 October 2026, for our client Rachid Kherrazi (WailSalutem Foundation), by Team 9 (Connect Nine). The agenda gave every part one or two speakers and a time, all five of us presented, and the deck has speaker notes for every part. The board slide shows an owner for each of the 25 items it marks as done. Our Scrum Master of Sprint 1 led the review. In the review we showed this deck, the GitLab issue board and the prototype in Figma. The deck and the client's form are in our team repository in docs/sprintReviews (merge request !43, issue #1519). The feedback on our analysis and design is in one comment on this evidence, with a section per sub-goal; the feedback on our process and teamwork is in the evidence "Sprint 1 review: feedback from the client".

ABOUT THE LEVELS
Levels are measured against BoKSA Full-Stack Developer level 3 (Advanced). A client rating of "beyond expectation" means above level 3, and "as expected" means at level 3. Under points to improve for the team the client wrote "n.a.": he saw nothing to improve. Where a sub-goal has no rating and the teacher gave no point to improve on it, and the presentation, the GitLab board, the prototype or our work shown or linked in the review gives proof, we assess it as above level 3; that is our own assumption, the section gives that proof, and our next step is to repeat what we did. Only where there is no proof at all do we claim no level. If a teacher disagrees with a level, we would like to know on what basis. If there is extra feedback, we would also like to know why it was not given during the review, so that next time we can ask for it at the review itself.

AA3 - above level 3 (client: "Clear idea of project needs" beyond expectation)
In the opening we showed the client's question word for word in Dutch, with the English translation, and the frame of Connect. Connect stands on its own next to the award website, and its aim is impact, not revenue: young talent made visible, young people motivated and the HvA partnership shown. We weighed the ethical side: our members are 15 to 30, so some are minors on an open platform. That is why a young person under 16 needs a parent's permission before the profile is visible, Connect never asks for an exact age, a birth date, a school or a street address, and a member can delete everything. We asked the client whether an organisation may see and contact a member under 18. The analysis behind these rules (#21, #24, #28, #29) is still open from Sprint 1 and moved to Sprint 2.

AA4 - above level 3 (no rating; the client saw nothing to improve and the teacher gave no point to improve on our advice; our own assumption)
We gave the client advice with reasons. Our proposed order: profiles, account and matching first, then opportunities, events and admin. In our plan the webshop stays without a sprint, because the client told us on 25 September that it is not required and no must item waits for it. Our plan for Sprints 2 to 5 (Decision 004, #1516, on the GitLab board) puts school goals first, because every one of us is assessed on them, then every must item, and no should or could item before the last must. We asked the client whether this order is right for the foundation. For the stack we gave a reason for each choice, and for the database we named the alternative it replaced: PostgreSQL 16 took the place of SQL Server, for no license cost for the foundation and a plain container on its server. We also named how we handle the risk that most of us are new to .NET: Nick already knows it and can unblock the other four.

AA5 - above level 3 (client: "Presentation skills" beyond expectation)
We adapted the review to the client: his question word for word in Dutch with an English translation, the four server layers explained as waiter, chef, menu and storeroom, a slide with the three things only the foundation can give us, and two plain questions for him at the end. For the young people who will use Connect, the prototype has the look of the Young Amstelland Awards website, and the deck explains its screens in plain words, such as "First name, age band and town. Never an address or a school."

OR1 - above level 3 (no rating; the client saw nothing to improve and the teacher gave no point to improve; our own assumption)
On purpose, we chose a stack most of us did not know yet, because learning is the main goal of the semester, and named how we handle that: Nick already knows .NET and helps the others. Each part of the stack has its reason on the slide: learning for React and .NET, long-term support until November 2028 for .NET 10, and for PostgreSQL 16, which replaced SQL Server on 26 September, no license cost for the foundation and a plain container on its server. The server code has one clear structure (#12): four layers where a wrong reference between layers does not build. Decisions 001 to 003 cover the conventions, the layers and .NET 10. The design held up against that real change: when we switched from SQL Server to PostgreSQL, only one layer was affected.

OR2 - above level 3 (strong point named by the client, without a rating; our own assumption)
In our repository (#1, #22, #1513) changes reach main only from dev, and every merge needs a teammate's approval and a green pipeline, so nothing gets in without a second pair of eyes and a green check. The pipeline refuses .env files with secrets, and a weekly scan stops packages with a known high or critical risk, a check we pulled forward from Sprint 2. Code conventions come with examples, templates and an onboarding guide. In the review we presented these methods as a foundation every next screen can land on and as the safe way of working underneath that was half of our Sprint 1 goal. Our own reasoning adds why they fit our context: a second pair of eyes fits a team where most of us are learning the stack, and refusing secrets and risky packages fits a platform that will hold personal data of members under 18. Sprint 1 had 31 merge requests.

OR3 - above level 3 (no rating; the client saw nothing to improve and the teacher gave no point to improve; our own assumption)
We involve the client directly and coordinate with him systematically: his choices of 25 September (the webshop is not required, the test site holds only invented accounts) are in our plan and rules, we keep a list of 134 questions only the foundation can answer (4 answered so far), and in the review we asked him for one meeting to set the order and go through the open questions, starting with the Sprint 2 ones, for a free Figma account so every link in our page descriptions (#1496) opens the right screen for him, and to fill in the Sprint Review form. We also acted on a teacher's request: 16 items of work we did before they had an issue are now on the board. Through the client we brought in the interests of others: we asked whether the prototype fits the young people he knows and whether an organisation may contact a member under 18. Our design lets a parent decide whether the profile of a young person under 16 is visible, and school goals come first in our plan, because every one of us is assessed on them.
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
Feedback on our team Sprint 1 review of 9 October 2026, from our client Rachid Kherrazi (WailSalutem Foundation, on the Sprint Review form) and our teacher Rob Kiebert. Each section below is about one sub-goal and is linked to it. Levels are measured against BoKSA Full-Stack Developer level 3 (Advanced). If a teacher disagrees with a level below, we would like to know on what basis, and why that feedback was not given during the review, so that next time we can ask for it at the review itself.

AA3 - above level 3 (based on the client's rating)
Client: "Clear idea of project needs": beyond expectation; under points to improve for the team he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: the client rated this beyond expectation and saw nothing to improve, and the teacher gave no point to improve on it.
Feed-up: at level 3, issues are analysed from multiple perspectives and the societal and ethical consequences are weighed consciously.
Feed-forward: we keep opening every review with the client's question and the frame of Connect, and in Sprint 2 we finish the analysis behind the rules for minors and check it with the client.

AA4 - above level 3 (no rating; the client saw nothing to improve and the teacher gave no point to improve on our advice; our own assumption)
Client: no rating for this in the Sprint Review form; under points to improve for the team he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on our advice; we read his point on not taking on too much as being about the amount of weight we plan, not about the order we advise.
Why this level: the client saw nothing to improve and we read the teacher's point as being about the amount of weight, not about our advice, and the review shows each part of well-founded advice. The recommendation: profiles, account and matching first, and a plan for Sprints 2 to 5 (Decision 004, #1516) with school goals first, because every one of us is assessed on them, and the question to the client whether that order is right. The alternatives we weighed: the webshop stays without a sprint, because the client does not require it and no must item waits for it, and PostgreSQL 16 took the place of SQL Server, for no license cost for the foundation and a plain container. The risk we handle: most of us are new to .NET, and a teammate who already knows it can unblock the other four.
Feed-up: at level 3, advice is well-founded: it weighs alternatives, names risks and ends in a substantiated recommendation.
Feed-forward: we repeat this in the Sprint 2 review: our advice with its reasons, the plan with its rules, and the question to the client whether our proposed order is right.

AA5 - above level 3 (based on the client's rating)
Client: "Presentation skills": beyond expectation; under points to improve for the team he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on how we communicated; we read his point on who says what as being about dividing the roles.
Why this level: the client rated this beyond expectation, and we read the teacher's point as being about the roles, not about how we adapted our words and screens to the client and the young people.
Feed-up: at level 3, communication is consciously adapted in form and language to target groups with diverse interests.
Feed-forward: we keep explaining the technical parts with everyday comparisons for the client, and in Sprint 2 we show working screens with plain texts for the young people.

OR1 - above level 3 (no rating; the client saw nothing to improve and the teacher gave no point to improve; our own assumption)
Client: no rating for this in the Sprint Review form; under points to improve for the team he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: the client saw nothing to improve and the teacher gave no point to improve on it, and the review shows a systematic design with justified choices, one of them against an alternative: a stack most of us did not know yet, chosen on purpose because learning is the goal, a reason on the slide for every part of the stack, PostgreSQL in place of SQL Server with its reasons, one clear structure for the server code (#12) in four layers that refuse a wrong reference, the decisions written down, and a design that held up when we switched databases on 26 September, with only one layer affected.
Feed-up: at level 3, design and realisation are systematic, choices are justified against alternatives, and the design is adapted to new insights.
Feed-forward: we repeat this in the Sprint 2 review: a reason for every design choice and how the design holds up against a change.

OR2 - above level 3 (strong point named by the client, without a rating; our own assumption)
Client: strong point of the team: "using the development tools"; under points to improve for the team he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: the client named this our strong point and saw nothing to improve, and the teacher gave no point to improve on it. The review shows the methods we chose: every merge needs a teammate's approval and a green pipeline, secrets are refused, and packages with a known high or critical risk are stopped; we presented them as a foundation every next screen can land on and as the safe way of working underneath that was half of our Sprint 1 goal. Our own reasoning adds why they fit our context: a second pair of eyes fits a team where most of us are learning the stack, and refusing secrets and risky packages fits a platform that will hold personal data of members under 18.
Feed-up: at level 3, relevant standards and methods from the field are selected independently, and why they fit the context is substantiated.
Feed-forward: we keep every change going through a merge request with a teammate's approval and a green pipeline as the code grows in Sprint 2.

OR3 - above level 3 (no rating; the client saw nothing to improve and the teacher gave no point to improve; our own assumption)
Client: no rating for this in the Sprint Review form; under points to improve for the team he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on this.
Why this level: the client saw nothing to improve and the teacher gave no point to improve on it, and the review shows that we involve the client directly and systematically (his choices of 25 September in our plan and our rules, a list of 134 questions only the foundation can answer, a request for one meeting to go through them, a Figma account so he can open every page himself, and the form), that we acted on a teacher's request, and that we weighed the interests of the young people, parents, organisations and the HvA in our rules and plan and brought them into our questions to the client.
Feed-up: at level 3, multiple stakeholders with diverse interests are involved proactively, the coordination with them is systematic, and their feedback is weighed consciously in the choices.
Feed-forward: we repeat this in the Sprint 2 review: the client's choices in our plan, the open questions for him, a request for what we need from the foundation, and questions for his feedback.
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
Feedback on our team Sprint 1 review of 9 October 2026, from two sources: our client Rachid Kherrazi, on the Sprint Review form behind this link, and our teacher Rob Kiebert. This evidence holds the feedback on our process and teamwork, in one comment with a section per sub-goal, each linked to its sub-goal. The client's feedback on our idea of the project needs, our presentation and our development tools is in the comment on the evidence "Sprint 1 review: Young Amstelland Connect". The form is in our team repository in docs/sprintReviews (merge request !43, issue #1519), which is how DLO asks us to share it with the team coach.

ABOUT THE LEVELS
Levels are measured against BoKSA Full-Stack Developer level 3 (Advanced). A client rating of "beyond expectation" means above level 3, and "as expected" means at level 3. Under points to improve for the team the client wrote "n.a.": he saw nothing to improve. Where the teacher gave a point to improve, the sub-goal is at level 3, also when the client rated the same topic beyond expectation. If a teacher disagrees with a level, we would like to know on what basis. If there is extra feedback, we would also like to know why it was not given during the review, so that next time we can ask for it at the review itself.

MS1 - above level 3 (client feedback)
Client: "Clear on what will be delivered in next sprint (user stories)": beyond expectation.

MS2 - at level 3 (client and teacher feedback)
Client: "User stories delivered in line with planning": beyond expectation. Teacher: check how much weight we finished in Sprint 1 and plan to that, so we do not take on too much; match the planning to earlier deliveries. The teacher's point keeps MS2 at level 3.

MS4 - at level 3 (client and teacher feedback)
Client: "Leading the meeting - clear structure and roles": beyond expectation; "Each team member actively participated": as expected; individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Teacher: prepare better beforehand who says what. The client's "as expected" and the teacher's point both put MS4 at level 3.
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
Feedback on our team Sprint 1 review of 9 October 2026, from our client Rachid Kherrazi (WailSalutem Foundation, on the Sprint Review form) and our teacher Rob Kiebert. Each section below is about one sub-goal and is linked to it. Levels are measured against BoKSA Full-Stack Developer level 3 (Advanced). If a teacher disagrees with a level below, we would like to know on what basis, and why that feedback was not given during the review, so that next time we can ask for it at the review itself.

MS1 - above level 3 (based on the client's rating)
Client: "Clear on what will be delivered in next sprint (user stories)": beyond expectation; under points to improve for the team he wrote "n.a.", so he saw nothing to improve.
Teacher: no point to improve on how clear the next sprint is; we read his point on not taking on too much as being about the amount of weight we plan.
Why this level: the client rated this beyond expectation, and we read the teacher's point as being about the amount of work, not about how clear the next sprint is. In the review we showed the Sprint 2 goal, its 30 items and who is Scrum Master and Product Owner, and we asked the client for the DNS record that our test address waits for and for one meeting on the order and the open questions. Every part of Connect is on one board and the risks and open points are in one place, so the work stays transparent for anyone who joins.
Feed-up: at level 3, risks and bottlenecks are anticipated before they become problems, and the work stays transparent and transferable.
Feed-forward: we keep showing the next sprint goal and what we wait on at every review, and we ask the client in time for what we need from the foundation, before it holds up the sprint.

MS2 - at level 3 (the teacher gave a point to improve)
Client: "User stories delivered in line with planning": beyond expectation; under points to improve for the team he wrote "n.a.", so he saw nothing to improve.
Teacher: check how much weight we finished in Sprint 1 and plan to that, so we do not take on too much; match the planning to earlier deliveries.
Why this level: the client rated this beyond expectation, but the teacher's point to improve keeps it at level 3.
Feed-up: at level 3, progress and collaboration are evaluated critically, patterns are recognised, and the process is adjusted proactively where necessary.
Feed-forward: in Sprint 1, 25 of 44 items with 46 of 82 weight were done, counting work waiting in a merge request (36 weight closed), and the screens part of the sprint goal moved to Sprint 2. Sprint 2 is planned at 63 weight, because Sprint 1 was preparation and we expect to go faster now. As we said in the review, we check the plan again after the Sprint 2 review. Then we compare the weight we finished with the 63 we planned and use that difference when we check the plan for Sprint 3.

MS4 - at level 3 (the teacher gave a point to improve)
Client: "Leading the meeting - clear structure and roles": beyond expectation. "Each team member actively participated": as expected. Individual level: "I see that all the team members are motivated to participate so for all the students a complement, so I will skip next details". Under points to improve for the team he wrote "n.a.".
Teacher: prepare better beforehand who says what.
Why this level: the client rated participation as expected, which is level 3, and the teacher's point to improve on who says what also keeps it at level 3, although the client rated leading the meeting beyond expectation.
Feed-up: at level 3, each member makes sure their work aligns with the work of the others and contributes to the joint result.
Feed-forward: before the next review every speaker knows their part and who answers which kind of question, and we rehearse once together.
```

## 6. Validation

On comment 1 and on comment 2: **⋮**, then **Request comment validation**, then choose Rob Kiebert. The client cannot validate, because he is not a teacher; each comment holds his feedback too, which Rob can check against the client's form behind evidence 2.

## 7. Check

Open **Goals & Progress**. Each of these sub-goals should show its evidence and a highlighted section of a comment: AA3, AA4, AA5, OR1, OR2, OR3, MS1, MS2, MS4. A sub-goal without both means a link is missing. No comment section should be linked to a goal name, only to a sub-goal code.

## 8. Team

- Bayon, Nick, Gurpreet and Kelvin add the same two GitLab links, the same two descriptions and the same two comments to their own Portflow, with the same links and highlights per section.
- Validation works per portfolio, so each of you sends the two validation requests from your own Portflow. Rob Kiebert then gets ten requests in total, so tell him beforehand in Teams.
- Kelvin's profile is Security Specialist: he replaces "Full-Stack Developer" with "Security Specialist" everywhere and rewrites each feed-up line to his own level 3 criterion, in the same style ("at level 3, ...") and without "you".
- Bayon is assessed on level 2: he first replaces "level 3 (Advanced)" with "level 2", then every other "level 3" with "level 2", and rewrites each feed-up line to the level 2 criterion of his profile, in the same style ("at level 2, ...") and without "you".

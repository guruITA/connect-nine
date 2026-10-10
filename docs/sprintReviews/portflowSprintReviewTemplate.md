# Template: Sprint Review in Portflow

For every sprint review of Young Amstelland Connect. Replace everything between `<` and `>`. Text inside the grey blocks gets pasted into Portflow: keep one line per paragraph, never break a sentence over two lines.

## Rules (read first)

1. **Team voice.** "We" and "our", never "I" or "my", except inside a quote from the client. All five paste the same texts. **Plus one personal block per student** at the end of the evidence 1 description, in third person without "he" or "she": role, the part presented in the review (from the agenda), and own items on the current board (closed, waiting in an open merge request, next sprint), next to the team issues of all five.
2. **Two pieces of evidence, both in Project Group, both "Add link".** Evidence 1 is the deck, evidence 2 is the client's Sprint Review form. Both are GitLab permalinks to the merge commit, so the version stays fixed.
3. **Evidence 1 holds what we showed, evidence 2 holds only feedback.** On evidence 1, the Analysing & Advising and Designing & Realising sub-goals (the Sprint Review assesses "Analyse, Design"). On evidence 2, the Managing & Collaborating sub-goals, and only what the client and the teacher said.
4. **No Personal Leadership in these team texts.** PL is about one person, and all five paste the same text.
5. **One comment per evidence, with the feedback of the client and the teacher together.** Only the teacher can validate, so he validates both comments. The client's own form is the proof for his part.
6. **One section per sub-goal in that comment.** Each section is highlighted and linked to its own sub-goal. A section never mentions another code.
7. **Every sub-goal linked to an evidence has three things:** a heading in that evidence's description, a section in its comment, and its code in the link step. The check list (step 7) has the same codes.
8. **The level rule, measured against BoKSA Full-Stack Developer level 3 (Advanced):**
   - the client rated it "beyond expectation", and the teacher gave no point to improve on that topic: above level 3
   - the client rated it "as expected": at level 3
   - the teacher gave a point to improve on that topic: at level 3, even if the client rated it beyond
   - "n.a." under points to improve on the form means the client saw nothing to improve. Write it in every section as "so he saw nothing to improve"
   - no rating and no point to improve from the teacher, but proof in the presentation, the GitLab board, the prototype or our work shown or linked in the review: above level 3, called "our own assumption". The section gives that proof, and the feed-forward is to repeat what we did
   - no proof at all in any of those: "no level this time", even with "n.a.", and the feed-forward says what we show next time
   - a strong point without a rating: above level 3, also called "our own assumption"
   - Before writing "nobody gave a point to improve", check that no teacher point touches that topic. If one seems to, say in the section why it is about something else (for example: his point on who says what is about dividing roles, not about how we communicated).
9. **Every "above level 3" without a rating needs proof from the review:** the presentation, the GitLab board, the prototype, or work we made that was shown or linked in it. The section names that proof and ties it to the parts of the criterion. Only when there is no proof at all, write "no level this time" and what you show next time. The slides of a review that is done cannot change: build the proof from what is in the deck, the speaker notes and what was shown.
10. **Facts only from the presentation** (deck and speaker notes), the GitLab board and Figma shown in it, the client's form, the teacher's feedback, and GitLab numbers. **Weights come from the board as the team corrected it after the review**, and the text names it with the date of the review ("on the board of <date>"), never "the current board", because that turns false when a merge request is merged. Say "done, counting work waiting in a merge request" and give the closed number too. Never "finished" for work that is still open. Never claim something the deck doesn't say (for example "we prepared it together", "all merge requests had approval"). **Only proof:** every sentence in a section proves a part of the criterion of that sub-goal. Context, backlog detail and numbers that prove nothing stay out (for example "one of 103 should and could items", "Sprint 1 had 31 merge requests").
11. **The disagree question appears once per text, never per section:** once in each description (ABOUT THE LEVELS) and once in the opening paragraph of each comment.
12. **All feedback in English**, also a teacher's Dutch feedback, so every teacher can read it later. **No "you" outside a quote** (paraphrase the BoKSA criterion), no AI mentions, no "retrospective" (the retro is a different datapoint). **No semicolons and no em-dashes:** a new thought gets a full stop, the rest a comma.

## Which client form row goes to which sub-goal

| Form row | Sub-goal |
|---|---|
| Clear idea of project needs | AA3 |
| Presentation skills | AA5 |
| Strong (team level), when it is about tools or methods | OR2 |
| Clear on what will be delivered in next sprint | MS1 |
| User stories delivered in line with planning | MS2 |
| Leading the meeting, clear structure and roles | MS4 |
| Each team member actively participated, and the individual feedback | MS4 |
| Teacher point about planning or weight | MS2 |
| Teacher point about roles or who says what | MS4 |

A new kind of point gets the sub-goal whose BoKSA level 3 text fits it best. Write the choice down in this table.

## Before Portflow: the files in the team repo

DLO says the form is shared with the team coach "by putting a copy on the team repo".
1. Create a closed issue logged afterwards (rule A8 in docs/team/issueRules.md) in the milestone of the sprint, assigned to all five, without weight, so the sprint numbers stay as they are.
2. Branch `docs/<issue>-sprint<n>-review-documents` from the newest dev.
3. Add `docs/sprintReviews/sprint<n>ReviewDeck.pptx` and `docs/sprintReviews/sprint<n>ReviewForm.pdf`, unchanged, plus a line in docs/README.md and a proof file in docs/<assignee>/portfolioProof/.
4. Open a merge request into dev with all five as reviewers. After the merge, take the merge commit for the permalinks.
5. Count the weight in GitLab for the milestone: all items, done (closed plus waiting for review), closed only, and the plan for the next sprint.

## 1. DLO

Open the folder "Sprint Review <n>" and upload the deck and the form, before the deadline of <date>.

## 2. Evidence 1: the Sprint Review

**Add it:** Collections, then **Project Group**, then **Add evidence**, then **Add link**.
**Name:** `Sprint <n> review: Young Amstelland Connect`
**Link:**
```
https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/blob/<merge-commit>/docs/sprintReviews/sprint<n>ReviewDeck.pptx
```

**Description:** pencil next to **Evidence description**, and paste:
```
Sprint <n> review of Young Amstelland Connect, <date>, for our client <client name> (<organisation>), by Team 9 (Connect Nine). <one sentence on how the review was set up, only what the deck shows>. In the review we showed <what was shown: deck, board, prototype, working software>. The deck and the client's form are in our team repository in docs/sprintReviews (merge request !<mr>, issue #<issue>). The feedback of the client and the teacher is in one comment on this evidence, with a section per sub-goal, and the feedback on how we worked is in the evidence "Sprint <n> review: feedback from the client".

ABOUT THE LEVELS
Levels are measured against BoKSA Full-Stack Developer level 3 (Advanced). Where the client gave a rating, the level follows that rating. Where nobody gave feedback to improve, we assess the sub-goal as above level 3. That is our own assumption, based on that lack of feedback. If a teacher disagrees with a rating, we would like to know on what basis. If there is extra feedback, we would also like to know why it was not given during the review, so that next time we can ask for it at the review itself.

<CODE> <short name of the sub-goal> - <above level 3 | at level 3> (<client: "<form row>" <rating> | no feedback to improve, our own assumption>)
<the proof from the review, in one paragraph, every fact from the deck, board, Figma or GitLab>
```
Repeat the last two lines for every AA and OR sub-goal of this evidence.

**Link the evidence:** **⋯**, then **Manage (sub-)goals**. Goals: Analysing & Advising, Designing & Realising. Sub-goals: <the codes of the headings>. **Save**.

## 3. Comment 1, on evidence 1

**Add it:** under **Evidence feedback**, **Add comment**, paste the whole text, save.

**Link each section**, once per section (<the codes>): **⋮**, **Link to sub-goal(s)**, leave **Link to goal(s)** empty, pick the code of the section, under **Highlight feedback** mark only that section (from its code line to its Feed-forward line), **Save**.

```
Feedback on our team Sprint <n> review of <date>, from our client <client name> (<organisation>, on the Sprint Review form) and our teacher <teacher name>. Each section below is about one sub-goal and is linked to it. Levels are measured against BoKSA Full-Stack Developer level 3 (Advanced). If a teacher disagrees with a level below, we would like to know on what basis, and why that feedback was not given during the review, so that next time we can ask for it at the review itself.

<CODE> - <above level 3 | at level 3 | no level this time> (<based on the client's rating | no feedback to improve, our own assumption | the teacher gave a point to improve>)
Client: <"<form row>": <rating> | no point on this in the Sprint Review form>.
Teacher: <his point, in English | no point to improve on this>.
Why this level: <the reason from the level rule, and with no feedback also the proof from the review>.
Feed-up: level 3 asks <the BoKSA level 3 criterion in own words, without "you">.
Feed-forward: <what we do next, concrete, in the next sprint>.
```
Repeat the section (from the code line to the Feed-forward line) for every code of evidence 1, with an empty line between sections.

## 4. Evidence 2: the client's feedback form

**Add it:** Collections, then **Project Group**, then **Add evidence**, then **Add link**.
**Name:** `Sprint <n> review: feedback from the client`
**Link:**
```
https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/blob/<merge-commit>/docs/sprintReviews/sprint<n>ReviewForm.pdf
```

**Description:**
```
Feedback on our team Sprint <n> review of <date>, from two sources: our client <client name>, on the Sprint Review form behind this link, and our teacher <teacher name>. The feedback of both is in one comment on this evidence, with a section per sub-goal, each linked to its sub-goal. The form is in our team repository in docs/sprintReviews (merge request !<mr>, issue #<issue>), which is how DLO asks to share it with the team coach. The review itself is in the evidence "Sprint <n> review: Young Amstelland Connect".

ABOUT THE LEVELS
Levels are measured against BoKSA Full-Stack Developer level 3 (Advanced). Where the client gave a rating, the level follows that rating. Where the teacher gave a point to improve, the level is at level 3. If a teacher disagrees with a rating, we would like to know on what basis. If there is extra feedback, we would also like to know why it was not given during the review, so that next time we can ask for it at the review itself.

<CODE> <short name of the sub-goal> - <above level 3 | at level 3> (<client feedback | client and teacher feedback | teacher feedback>)
Client: <"<form row>": <rating>>. Teacher: <his point>. <one sentence with numbers if they matter>. <The teacher's point keeps <CODE> at level 3.>
```
Only MS sub-goals that the client or the teacher gave feedback on.

**Link the evidence:** **⋯**, **Manage (sub-)goals**. Goals: Managing & Collaborating. Sub-goals: <the codes of the headings>. **Save**.

## 5. Comment 2, on evidence 2

Same as step 3: one comment, one section per MS code, each section highlighted and linked to its own code.

## 6. Validation

On comment 1 and comment 2: **⋮**, **Request comment validation**, choose <teacher name>.

## 7. Check before you're done

- [ ] Per evidence: the codes of the headings, the comment sections and the link step are exactly the same.
- [ ] No section mentions another code.
- [ ] Every level follows the level rule, and a heading and its section have the same level.
- [ ] Every "nobody gave a point to improve" is true: no teacher point touches that topic.
- [ ] Every "above level 3" without a rating has proof of every part of its criterion.
- [ ] Every fact can be found in the deck, the speaker notes, the board, Figma, the form, the teacher's feedback or GitLab.
- [ ] No "I", "my" or "you" outside a quote, no "retrospective", one line per paragraph.
- [ ] In Portflow: **Goals & Progress** shows every code with its evidence and a highlighted section.

## 8. Team

The other four paste the same texts and links and make the same highlights.

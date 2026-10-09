# Proof: task 178.1 The page documents point to the prototype screens (#1497)

Merge request: MR_NUMBER, branch `docs/1496-prototype-in-page-docs`. Issue: 178 The page descriptions point to the screens of the prototype (#1496).
Profile: Full Stack Software Developer, level 3. Portflow: Managing and Collaborating, MS1 and MS4. Link the merge request and this file.

Every point the task asks for is below. A point that needs someone else is marked TODO with who and what.

## MS1 Seeing risks early and keeping the work transferable

School asks: "You anticipate risks and bottlenecks in the process before they become problems and ensure that the work remains transparent and transferable."

1. **The risk this removes.** Before this change the page documents in `docs/pages` named a frame on the Figma page Design directions by its name only, without a link. A teammate who picks up a page had to search the Figma board for the right drawing. Now 28 page documents link the screens of the clickable prototype Prototype sprint 1 and 2 (desktop) directly, 82 links to 79 screens, right under the design line the builder already reads.

2. **Why a link per page document and not one link in the README.** One link in the README asks every builder to find their own screens among 79. A link per page document puts the screens where the rules of that page already are. The README of `docs/pages` explains once how to read the prototype: the rows A to J, the issue numbers above each screen, the screens with a message that are not on the click path, and that the prototype has no phone screens, so for a phone the page documents still point to Design directions.

3. **What the team does when a screen and a page document disagree.**
   - TODO Wesley and team: decide at the stand up what wins when a screen of the prototype and a page document disagree. Then I write the decision here.

## MS4 Making your work fit with the work of the team

School asks: "You ensure that your work aligns with that of others and contributes to the joint result."

1. **How the design, the issues and the page documents point to each other.** The line above each screen in the prototype names the GitLab issues it shows. Each page document links its screens, and already lists the issues that build the page under Built by. A teammate can go from an issue to its page document to its screen and back.

2. **How I checked that no page was missed.** All 64 slice issues of sprint 1 and 2 (milestones Sprint 1 and Sprint 2 in GitLab, label type::slice) are named in at least one page document that now has a prototype part. Every one of the 82 links was checked against the list of screens on the Figma page: none points to a missing screen or to a screen with another name.

3. **Where a screen went when the obvious page document was the wrong one.** `LogInTries.md` describes a later page where a member sees their own log in tries, so the screens with a wait or a lock went to `LogIn.md`, where those messages appear. `MyOrganisation.md` describes the opportunities of an organisation, a later sprint, so the My account screens of an organisation went to `MyAccount.md`. The screen of a member who does not exist is linked from both `MemberProfile.md` and `NotFound.md`, because both pages describe that message.

4. **The board number.** The issue got board number 178, the next free number, and `docs/team/boardNumbers.md` has its row.

## TODO

What is still needed before this proof is complete. Tick a line when it is done.

- [ ] Every team member: open the screens from one page document you will build and confirm in #1496 that the links open the right screens (Definition of done of the issue).
- [ ] Reviewer of this merge request: write your name and the date in #1496.
- [ ] Wesley and team: decide what wins when a screen and a page document disagree, then fill in MS1.3.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.
